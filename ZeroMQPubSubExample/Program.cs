using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroMQPubSubExample
{
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

            using (var context = NetMQContext.Create())
            using (var pub = context.CreatePublisherSocket())
            using (var sub = context.CreateSubscriberSocket())
            {
                pub.Connect("tcp://localhost:5012");
                sub.Connect("tcp://localhost:5013");
                while(true)
                {
                    
                    System.Threading.Thread.Sleep(1000);
                    string timestamp = unix_timestamp_now().ToString().Replace(',', '.'); 
                    string raw_msg = "{ \"timestamp\": " + timestamp + ", \"msg_id\": \"MZ9YV.6\", \"sender\": [\"MZ9YV\", \"cKsNg\"], \"payload\": {\"PongMessage\": {\"text\": \"Hello ponger, this is pinger 1!\"}}}";
                    //byte[] bytes = Encoding.Default.GetBytes(raw_msg);
                    //raw_msg = Encoding.UTF8.GetString(bytes);

                    pub.SendFrame(raw_msg);
                    //break;
                }

                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }

        }
    }
}
