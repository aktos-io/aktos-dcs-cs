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
    class Program
    {
        static void Main(string[] args)
        {
            Actor pinger = new Pinger();
            Actor.wait_all(); 
        }
    }
}
