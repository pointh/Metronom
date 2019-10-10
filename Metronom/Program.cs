using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Metronom
{
    // TimeOfTick by nemusel dědit z EventArgs, ale takto je jasné, proč tu třídu vlastně máme
    public class TimeOfTick : EventArgs
    {
        public DateTime Time { get; set; }

        public TimeOfTick(DateTime dt)
        {
            Time = DateTime.Now;
        }
    }

    public class Metronom
    {
        // Jenom metody tohoto druhu mohou být pověšeny na Tick event
        public delegate void TickHandler(Metronom m, TimeOfTick e);
        public event TickHandler Tick;

        public void Tikej()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (Tick != null)
                {
                    TimeOfTick TOT = new TimeOfTick(DateTime.Now);
                    // Spusť všechny metody pověšené na Tick ...
                    Tick(this, TOT);
                }
            }
        }

        public void Start()
        {
            Console.WriteLine("Ukonči Ctrl-C ...");
            Tikej();
        }
    }

    public class Listener
    {
        public int ListID { set; get; }
        public Listener(int ID) { ListID = ID; }
        public void Subscribe(Metronom m)
        {
            // Tohle je multicasting
            m.Tick += HeardIt;
            m.Tick += HeardItAndProcessed;
        }
        private void HeardIt(Metronom m, TimeOfTick e)
        {
            Console.WriteLine("{0} HEARD IT AT {1}", ListID, e.Time);
        }

        private void HeardItAndProcessed(Metronom m, TimeOfTick e)
        {
            Console.WriteLine("{0} PROCESSED IT AT {1}", ListID, e.Time);
        }
    }

    class Program
    {
        static void Main()
        {
            Metronom m = new Metronom();
            Listener l = new Listener(1); l.Subscribe(m);
            Listener n = new Listener(2); n.Subscribe(m);
            m.Start();
            // Tohle se nikdy nespustí - pokud s tím něco neuděláme
            Console.WriteLine("Jsem zpátky!");
        }
    }
}
