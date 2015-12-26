using System;
using NetMQ;

namespace HelloWorld
{
    internal static class Program1
    {
        private static void Main2()
        {
            Console.Title = "NetMQ HelloWorld";

            using (var context = NetMQContext.Create())
            using (var pub = context.CreatePublisherSocket())
            using (var sub = context.CreateSubscriberSocket())
            {
                pub.Connect("tcp://localhost:5012");
                sub.Connect("tcp://localhost:5013"); 
                for (int i = 0; i < 50; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                    string raw_msg = "{ u'timestamp': 1451168495.814, u'msg_id': u'MZ9YV.6', u'sender': [u'MZ9YV', u'cKsNg'], u'payload': {u'PongMessage': {u'text': u'Hello ponger, this is pinger 1!'}}}"; 
                    pub.SendFrame(raw_msg);
                    //break;
                }

                //Console.WriteLine("From Server: {0}", client.ReceiveFrameString());

                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}

