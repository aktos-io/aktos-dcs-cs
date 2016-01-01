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
            while (true)
            {
                Console.WriteLine("Pinger is in action! ");
                System.Threading.Thread.Sleep(2000);

                string msg_ser = @"
                    {""PongMessage"": 
                        {""text"": ""this is proper message from csharp implementation""}
                    }
                "; 

                send(msg_ser);
            }
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
