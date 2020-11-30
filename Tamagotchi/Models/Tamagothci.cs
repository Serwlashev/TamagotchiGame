using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Tamagotchi.Exceptions;
using Timer = System.Timers.Timer;

namespace TamagotchiGame.Models
{
    class Tamagotchi
    {
        private readonly string name;                  // Character name

        private Timer wholeLifeTimer;        // Timer count life of the Tamagotchi
        private Timer eventTimer;            // Timer for life's events and desires
        private Random number;

        private MessageBoxButtons buttons;           // Button on the message

        private event Action LifeEvent;       //  Events during the life 

        private int countUnacceptedDesires;  // Counter increases when the user didn't accept a desire of the Tamagotchi, describes it's mood in the console
        private enum Desires                 
        {
            Feed,
            Walk,
            Sleep,
            Treat,
            Play
        }
        public Tamagotchi(string name)
        {
            number = new Random();

            countUnacceptedDesires = 0;
            this.name = name;

            buttons = MessageBoxButtons.OKCancel;
            InitTimers();
        }

        private void InitTimers()       // Method initiate timers in the program
        {
            wholeLifeTimer = new Timer();
            eventTimer = new Timer();
            wholeLifeTimer.Interval = GetLifeTime();
            eventTimer.Interval = 10000;
        }

        private int GetLifeTime()       // Method generates a random duration of the Tamagotchi's life from 1 to 2 minutes
        {
            return number.Next(60000, 120000);
        }

        public void StartLife()         // Method begins the life of the Tamagotchi and catches events 
        {
            StartTimers();

            Draw();

            while (true)
            {
                LifeEvent?.Invoke();
                Thread.Sleep(1000);
            }
        }

        private void Draw()
        {
            Console.Clear();

            // Draw Tamagothci according to it's mood
            switch(countUnacceptedDesires)
            {
                case 0:
                    Smile();
                    break;
                case 1:
                    Serious();
                    break;
                case 2:
                    Sad();
                    break;
                default:        // If Tamagitchi dies we shouldn't draw him
                    break;
            }
        }

        private void Sad()
        {
            Console.WriteLine("\n\n\n\t\t\t" + name);

            Console.WriteLine("\t\t/-------------\\");
            Console.WriteLine("\t\t|  *        * |");
            Console.WriteLine("\t\t|      /\\     |");
            Console.WriteLine("\t\t|   /------\\  |");
            Console.WriteLine("\t\t---------------");
        }

        private void Serious()
        {
            Console.WriteLine("\n\n\n\t\t\t" + name);

            Console.WriteLine("\t\t/-------------\\");
            Console.WriteLine("\t\t|  O        O |");
            Console.WriteLine("\t\t|      /\\     |");
            Console.WriteLine("\t\t|   -------   |");
            Console.WriteLine("\t\t---------------");
        }

        private void Smile()
        {
            Console.WriteLine("\n\n\n\t\t\t" + name);

            Console.WriteLine("\t\t/-------------\\");
            Console.WriteLine("\t\t|  O        O |");
            Console.WriteLine("\t\t|      /\\     |");
            Console.WriteLine("\t\t|  \\=======/  |");
            Console.WriteLine("\t\t---------------");
        }

        public void StartTimers()
        {
            // When Tamagotchi's life ends it died
            wholeLifeTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate (object sender, ElapsedEventArgs e)
            {
                Death();
            }
            );

            // Every 10 seconds happens random event in the Tamagotchi's life
            eventTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate (object sender, ElapsedEventArgs e)
            {
                AddEvent();
            }
            );

            // Start for timers
            wholeLifeTimer.Start();
            eventTimer.Start();
        }

        private void Death(bool isDeadFromOldAge = true)        // When Tamagotchi die we throw an Exception and catch it in the class Application 
        {
            if(isDeadFromOldAge)
            {
                LifeEvent += () =>
                {
                    throw new DeathException("Your Tamagotchi died, but he lived a long life");
                };
            }
            else
            {
                LifeEvent += () =>
                {
                    throw new DeathException("You were a bad master and your Tamagotchi died");
                };
            }
        }

        public void AddEvent()      // Method add new event according to random desire
        {
            switch(GetDesire())
            {
                case Desires.Feed:
                    LifeEvent += TimeToFeed;
                    break;
                case Desires.Walk:
                    LifeEvent += TimeToWalk;
                    break;
                case Desires.Sleep:
                    LifeEvent += TimeToSleep;
                    break;
                case Desires.Treat:
                    LifeEvent += TimeToTreat;
                    break;
                case Desires.Play:
                    LifeEvent += TimeToPlay;
                    break;
            }
        }

        private Desires GetDesire()     // Method provides a new random desire for AddEvent
        {
            int randNum = number.Next(0, 5);
            Desires desire = Desires.Feed;

            switch(randNum)
            {
                case 0:
                    desire = Desires.Feed;
                    break;
                case 1:
                    desire = Desires.Walk;
                    break;
                case 2:
                    desire = Desires.Sleep;
                    break;
                case 3:
                    desire = Desires.Treat;
                    break;
                case 4:
                    desire = Desires.Play;
                    break;
            }
            return desire;
        }

        // Group of method which describes desires of the Tamagotchi
        private void TimeToFeed()
        {
            StartEvent("I want to eat, feed me!", Desires.Feed);
        }

        private void TimeToWalk()
        {
            StartEvent("I want to walk, let's go outside!", Desires.Walk);
        }

        private void TimeToSleep()
        {
            StartEvent("I want to sleep, put me in bed!", Desires.Sleep);
        }

        private void TimeToTreat()
        {
            StartEvent("I am ill, please, treat me.", Desires.Treat);
        }

        private void TimeToPlay()
        {
            StartEvent("I feel boring, let's play with me!", Desires.Play);
        }

        // Method shows a message to the user and checks if the user didn't accept it 3 times
        private void StartEvent(string text, Desires desire)
        {
            if (MessageBox.Show(text, name, buttons) == System.Windows.Forms.DialogResult.Cancel)   // If user didn't accept Tamagotchis desire it's become more sad
            {
                countUnacceptedDesires++;
            }
            else
            {
                // If user accepted a desire than Tamagotci is smiling
                if(countUnacceptedDesires > 0)
                {
                    countUnacceptedDesires = 0;
                }
            }

            Draw();

            CheckDesires();

            DeleteDesire(desire);
        }

        private void CheckDesires()
        {
            // If user refused desires 3 times, Tamagotchi died from bad careness
            if (countUnacceptedDesires == 3)
            {
                Death(false);
            }
        }

        // Method deletes a desire from the events
        private void DeleteDesire(Desires desire)
        {
            switch (desire)
            {
                case Desires.Feed:
                    LifeEvent -= TimeToFeed;
                    break;
                case Desires.Walk:
                    LifeEvent -= TimeToWalk;
                    break;
                case Desires.Sleep:
                    LifeEvent -= TimeToSleep;
                    break;
                case Desires.Treat:
                    LifeEvent -= TimeToTreat;
                    break;
                case Desires.Play:
                    LifeEvent -= TimeToPlay;
                    break;
            }
        }
    }
}
