using NetMQ;
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
            while (true)
            {
                Console.WriteLine("naber");
                System.Threading.Thread.Sleep(1000);
            }
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

        static void Main(string[] args)
        {
            ActorSubscriber a = new ActorSubscriber();
            using (var context = NetMQContext.Create())
            using (var pub = context.CreatePublisherSocket())
            using (var sub = context.CreateSubscriberSocket())
            {
                pub.Connect("tcp://localhost:5012");
                sub.Connect("tcp://localhost:5013");
                sub.Subscribe(""); 
                int i = 0; 
                while(true)
                {
                    
                    string timestamp = unix_timestamp_now().ToString().Replace(',', '.'); 
                    string raw_msg = "{ \"timestamp\": " + timestamp + ", \"msg_id\": \"MZ9YV."+ i++ +"\", \"sender\": [\"MZ9YV\", \"cKsNg\"], \"payload\": {\"PongMessage\": {\"text\": \"Hello ponger, this is pinger 1!\"}}}";
                    //byte[] bytes = Encoding.Default.GetBytes(raw_msg);
                    //raw_msg = Encoding.UTF8.GetString(bytes);

                    pub.SendFrame(raw_msg);
                    Console.WriteLine("From Server: {0}", sub.ReceiveFrameString());
                    System.Threading.Thread.Sleep(10000);

                    //break;
                }

                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }

        }
    }
}
