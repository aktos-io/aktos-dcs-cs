using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace aktos_dcs_cs
{
    class ActorSubscriber
    {
        BackgroundWorker sub_worker;
        public delegate void callback(object sender, object arg);
        public event callback event_receive;
        public ActorSubscriber()
        {
            sub_worker = new BackgroundWorker();
            sub_worker.DoWork += Sub_worker_DoWork;
            sub_worker.RunWorkerAsync();
        }
        private void on_receive(object msg)
        {
            if (event_receive != null)
            {
                event_receive(this, msg);
            }
        }
        private void Sub_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var context = NetMQContext.Create())
            using (var sub = context.CreateSubscriberSocket())
            {
                sub.Connect("tcp://localhost:5013");
                sub.Subscribe("");
                while (true)
                {
                    string received = sub.ReceiveFrameString();
                    //Console.WriteLine("From Server: {0}", received);
                    on_receive(received);
                }
            }
        }


    }
    class ActorPublisher
    {
        private static string unix_timestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            double t = (double)span.TotalSeconds + 7200;
            return t.ToString().Replace(",", "."); 
        }

        private static string unix_timestamp_now()
        {
            return unix_timestamp(DateTime.UtcNow);
        }

        public static string random_string(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string actor_id;
        int i = 0;
        PublisherSocket pub;
        public ActorPublisher()
        {
            actor_id = random_string(5);
            var context = NetMQContext.Create();
            pub = context.CreatePublisherSocket();
            pub.Connect("tcp://localhost:5012");
        }
        public void send(object msg)
        {
            //string message = JsonConvert.SerializeObject(msg);
            //Console.WriteLine("Msg to send: {0}", message);
            if (true)
            {
                string msg_id = actor_id + "." + i++;
                string telegram = @"
                {{""timestamp"": {0}, ""msg_id"": ""{1}"", ""sender"": [""{2}""], ""payload"": {3} }}
            ";
                string json = string.Format(telegram, unix_timestamp_now(), msg_id, actor_id, msg);
                //string json = JsonConvert.SerializeObject(telegram);
                //System.Console.WriteLine("serialized object: {0}", json); 

                json = json.Replace("\r", "").Replace("\n", "").Trim();

                pub.SendFrame(json);
            }
        }
    }
    public class Actor
    {
        ActorSubscriber sub = new ActorSubscriber();
        ActorPublisher pub = new ActorPublisher();
        BackgroundWorker action_worker;
        List<List<string>> filter_history = new List<List<string>>();
        public delegate void msg_callback(Dictionary<string, object> msg);
        public string actor_id;
        private SynchronizationContext syncContext;

        public Actor()
        {
            sub.event_receive += on_receive_event;
            action_worker = new BackgroundWorker();
            action_worker.DoWork += Action_DoWork;
            action_worker.RunWorkerAsync();
            actor_id = pub.actor_id;
            syncContext = SynchronizationContext.Current ??
                new SynchronizationContext();
        }

        private void Action_DoWork(object sender, DoWorkEventArgs e)
        {
            action();
        }

        public virtual void action()
        {

        }

        private void on_receive_event(object sender, object arg)
        {
            Dictionary<string, object> msg_dict = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)arg);
            if (filter_msg(msg_dict))
            {
                Dictionary<string, object> payload_dict = ((JObject)msg_dict["payload"]).ToObject<Dictionary<string, object>>(); 
                List<string> keyList = new List<string>(payload_dict.Keys);
                foreach (string key in keyList)
                {
                    //Console.WriteLine("Topic of incoming message: {0}", key);

                    // if there is a topic handler (handle_SUBJECT function), 
                    // pass message to that function. else, pass msg to default receive function
                    try
                    {
                        MethodInfo handler_func = this.GetType().GetMethod("handle_" + key);
                        handler_func.Invoke(this, new object[] { ((JObject)payload_dict[key]).ToObject<Dictionary<string, object>>() });  
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            string event_name = "event_" + key;
                            EventInfo handler_event = this.GetType().GetEvent(event_name);
                            var event_delegate = (MulticastDelegate)this.GetType().GetField(event_name, 
                                BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
                            foreach (var handler in event_delegate.GetInvocationList())
                            {
                             
                                syncContext.Post(new SendOrPostCallback((o) => {
                                    handler.Method.Invoke(handler.Target,
                                     new object[] { ((JObject)payload_dict[key]).ToObject<Dictionary<string, object>>() });
                                }), null);
                            }
                        }
                        catch
                        {
                            //Console.WriteLine("exception: {0}", e); 
                            receive(msg_dict);
                        }
                    }
                }
            }
        }

        private bool filter_msg(Dictionary<string, object> msg_dict)
        {
            /* 

            drop message if:
                
                * this actor is in sender list (this is a short circuit message) 
                * that or a newer message is received already (message is duplicate or too old) 
                
            if message is not dropped, add the message into `message_history` 

            */
            
            string msg_id = (string) msg_dict["msg_id"];
            Newtonsoft.Json.Linq.JArray sender = (Newtonsoft.Json.Linq.JArray) msg_dict["sender"];


            // check if this is short circuit message: 
            foreach(string s in sender) {
                if(s == actor_id)
                {
                    //Console.WriteLine("dropping short circuit message... {0}", msg);
                    return false; 
                }
            }

            // check if this or newer message has been received already 
            for(int i = 0; i < filter_history.Count; i++)
            {
                // column order: `creator_id`, `sequence number`
                if ((string) filter_history[i][0] == msg_id.Split('.')[0])
                {
                    // a message has been received from this actor already. check sequence number. 
                    int sequence_number;
                    if(Int32.TryParse(msg_id.Split('.')[1], out sequence_number))
                    {
                        int tmp_seq_num;
                        Int32.TryParse(filter_history[i][1], out tmp_seq_num); 
                        if (sequence_number < tmp_seq_num)
                        {
                            Console.WriteLine("CAUTION: dropping old message... {0}", msg_dict);
                            return false; 
                        }else if (sequence_number == tmp_seq_num)
                        {
                            //Console.WriteLine("dropping duplicate message... {0}", msg);
                            return false; 
                        }
                        else
                        {
                            // message is valid. update last received message sequence number
                            filter_history[i][1] = sequence_number.ToString();
                            return true; 
                        }
                    }
                }
            }

            // if code reaches here, this sender is new, so append to filter history. 
            filter_history.Add(msg_id.Split('.').ToList());
            return true; 
        }
        public virtual void receive(Dictionary<string, object> msg)
        {
            //Console.WriteLine("Actor received a message: {0}", msg);
        }
        public void send(object msg)
        {
            pub.send(msg);
        }
        public static void wait_all()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(999999);
            }
        }

    }

    class Test : Actor
        {
            public override void receive(Dictionary<string, object> msg)
            {
                Console.WriteLine("RFID received message: {0}", msg); 
            }

            public override void action()
            {
                while (true)
                {
                    Console.WriteLine("RFID is in action!");
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

    }

