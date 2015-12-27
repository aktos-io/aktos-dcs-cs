using NetMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroMQPubSubExample
{
    class ActorSubscriber
    {
        BackgroundWorker sub_worker;

        public ActorSubscriber()
        {
            sub_worker = new BackgroundWorker();
            sub_worker.DoWork += Sub_worker_DoWork;
            sub_worker.RunWorkerAsync();
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
                    Console.WriteLine("From Server: {0}", received);
                }
            }
        }
        
    }
    class ActorPublisher
    {
        private ActorPublisher()
        {

        }

    }
    class Program
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

        static void Main(string[] args)
        {
            ActorSubscriber a = new ActorSubscriber();
            using (var context = NetMQContext.Create())
            using (var pub = context.CreatePublisherSocket())
            {
                pub.Connect("tcp://localhost:5012");
                int i = 0; 
                while(true)
                {
                    
                    string timestamp = unix_timestamp_now().ToString().Replace(',', '.'); 
                    string raw_msg = "{ \"timestamp\": " + timestamp + ", \"msg_id\": \"MZ9YV."+ i++ +"\", \"sender\": [\"MZ9YV\", \"cKsNg\"], \"payload\": {\"PongMessage\": {\"text\": \"Hello ponger, this is pinger 1!\"}}}";
                    //byte[] bytes = Encoding.Default.GetBytes(raw_msg);
                    //raw_msg = Encoding.UTF8.GetString(bytes);
                    Dictionary<string, object> telegram = new Dictionary<string, object>();
                    telegram.Add("timestamp", unix_timestamp_now());
                    string random_sender_id = random_string(5);
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
                    System.Threading.Thread.Sleep(1000);

                    //break;
                }

                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }

        }
    }
}
