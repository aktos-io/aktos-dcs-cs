using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private static double unix_timestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (double)span.TotalSeconds + 7200;
        }

        private static double unix_timestamp_now()
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

        string random_sender_id;
        int i = 0;
        PublisherSocket pub;
        public ActorPublisher()
        {
            random_sender_id = random_string(5);
            var context = NetMQContext.Create();
            pub = context.CreatePublisherSocket();
            pub.Connect("tcp://localhost:5012");
        }
        public void send(object msg)
        {
            Dictionary<string, object> telegram = new Dictionary<string, object>();
            telegram.Add("timestamp", unix_timestamp_now());
            telegram.Add("msg_id", random_sender_id + "." + i++);
            List<string> sender_list = new List<string>();
            sender_list.Add(random_sender_id);
            telegram.Add("sender", sender_list);
            Dictionary<string, object> topic = new Dictionary<string, object>();
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("text", "hello from new implementation....");
            topic.Add("PongMessage", payload);
            telegram.Add("payload", topic);
            string json = JsonConvert.SerializeObject(telegram);
            //System.Console.WriteLine("serialized object: {0}", json); 


            pub.SendFrame(json);
            System.Threading.Thread.Sleep(5000);
        }
    }
    public class Actor
    {
        ActorSubscriber sub = new ActorSubscriber();
        ActorPublisher pub = new ActorPublisher();
        BackgroundWorker action_worker;

        public Actor()
        {
            sub.event_receive += on_receive_event;
            action_worker = new BackgroundWorker();
            action_worker.DoWork += Action_DoWork;
            action_worker.RunWorkerAsync(); 
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
            receive(arg);
        }
        public virtual void receive(object msg)
        {
            Console.WriteLine("Actor has written to the console... {0}", msg);


        }
        public void send(object msg)
        {
            pub.send(msg);
        }
    }

        class Test : Actor
        {
            public override void receive(object msg)
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

