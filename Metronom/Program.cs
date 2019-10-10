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

        // Do toho, jak funguje Tikej nikomu nic není - úmyslně
        // publikujeme jenom Tick event a typ delegáta
        private void Tikej()
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
            // Tikej(); 
            // Spuštění metody Tikej v dalším vlákně
            Thread t = new Thread(Tikej);
            t.Start();
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

        public void Unsubscribe(Metronom m)
        {
            m.Tick -= HeardIt;
            m.Tick -= HeardItAndProcessed;
        }
    }

    class Program
    {
        static void Main()
        {
            Metronom m1 = new Metronom();
            Listener l = new Listener(1); l.Subscribe(m1);
            Listener n = new Listener(2); n.Subscribe(m1);
            m1.Start();
            // Tohle se nikdy nespustí - pokud s tím něco neuděláme
            // Pokud m.Start() otevře nové vlákno, program pojede dál...
            Console.WriteLine("Jsem zpátky!");
            Thread.Sleep(6000);
            Console.WriteLine("Odhlašuji id {0} z odběrů metronomu", l.ListID);
            l.Unsubscribe(m1);

            Metronom m2 = new Metronom();
            m2.Start();
            Thread.Sleep(4000);
            // l už existuje, jen ho přihlásíme k 2. metronomu
            l.Subscribe(m2);
        }
    }
}
