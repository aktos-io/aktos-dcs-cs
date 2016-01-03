using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aktos_dcs_cs; 

namespace ponger
{
    class Ponger : Actor
    {
        public override void action()
        {
            Console.WriteLine("Ponger is in action! ");
            System.Threading.Thread.Sleep(2000); 
        }
        public override void receive(Dictionary<string, object> msg)
        {
            Console.WriteLine("Received a message: {0}", msg);
        }
        public void handle_PongMessage(Dictionary<string, object> msg)
        {
            Console.WriteLine("Pong message has been received: {0}", msg["text"]);
            send(@"
                {""PingMessage"": {""text"": ""this is ponger in csharp, hello pinger!""}}
            ");
        }
        public void handle_PeriodicPongMessage(Dictionary<string, object> msg)
        {
            Console.WriteLine("Periodic Pong Message is received: {0}", msg["text"]); 
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Actor ponger = new Ponger();
            Actor.wait_all(); 
        }
    }
}
