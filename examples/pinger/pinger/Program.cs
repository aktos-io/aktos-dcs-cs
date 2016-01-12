using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aktos_dcs_cs;

namespace pinger
{
    class Pinger : Actor
    {
        public delegate void cb_PingMessage(Dictionary<string, object> msg);
        public event cb_PingMessage event_PingMessage;

        public override void action()
        {
            string msg_ser = @"
                    {""PongMessage"": 
                        {""text"": ""this is a periodic PongMessage message from csharp implementation""}
                    }
                ";
            send(msg_ser);

            while (true)
            {
                Console.WriteLine("Pinger is in action! ");
                System.Threading.Thread.Sleep(2000);

                msg_ser = @"
                    {""PeriodicPongMessage"": 
                        {""text"": ""this is a periodic PongMessage message from csharp implementation""}
                    }
                ";
                send(msg_ser);
            }
        }
        public void handle_PingMessage(Dictionary<string, object> msg)
        {
            if (event_PingMessage != null)
            {
                event_PingMessage(msg);
            }
            Console.WriteLine("Pinger handled PingMessage: {0} ", msg["text"]);

            string msg_ser = @"
                    {""PongMessage"": 
                        {""text"": ""this is proper message from csharp implementation""}
                    }
                ";
            System.Threading.Thread.Sleep(2000);
            send(msg_ser);
        }
        
    }

    class Test
    {
        // Think that this class is already inherited from another class, 
        // like Windows Form 
        // You can still define your communicator class above, then define 
        // events in order to handle them in this class. 
        public Test()
        {
            Pinger dcs = new Pinger();
            dcs.event_PingMessage += handle_PingMessage; 

        }

        public void handle_PingMessage(Dictionary<string, object> msg)
        {
            Console.WriteLine("Test got message: {0}", msg["text"]); 
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Test x = new Test(); 
            Actor.wait_all(); 
        }
    }
}
