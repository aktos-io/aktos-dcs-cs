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
            System.Threading.Thread.Sleep(3000);
            send(@"
            {""PingMessage"": {""text"": ""this is ponger in csharp, hello pinger!""}}
            "); 
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
