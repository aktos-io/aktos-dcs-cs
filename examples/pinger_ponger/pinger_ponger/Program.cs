using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aktos_dcs_cs;

namespace pinger_ponger
{
    class Pinger : Actor
    {
        public override void action()
        {
            while (true)
            {
                Console.WriteLine("Pinger is running...");
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
    class Ponger : Actor
    {
        public override void action()
        {
            while (true)
            {
                Console.WriteLine("Ponger is running...");
                System.Threading.Thread.Sleep(2000);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Actor pinger = new Pinger();
            Actor ponger = new Ponger();
            Actor.wait_all();
        }
    }
}
