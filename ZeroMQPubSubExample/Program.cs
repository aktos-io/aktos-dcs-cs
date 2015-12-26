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
                    string raw_msg = "{ \"timestamp\": 1451168495.814, \"msg_id\": \"MZ9YV.6\", \"sender\": [\"MZ9YV\", \"cKsNg\"], \"payload\": {\"PongMessage\": {\"text\": \"Hello ponger, this is pinger 1!\"}}}";
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
