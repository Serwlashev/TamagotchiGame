using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tamagotchi.Exceptions;

namespace TamagotchiGame.Models
{
    class Application
    {

        public void Start()
        {
            string choice;          // User's choice in the menu
            do
            {
                Console.Clear();

                ShowMenu();

                choice = Console.ReadLine();

                if(choice.Equals("1"))
                {
                    CreateNewTamagotchi();
                }

            } while (!choice.Equals("0"));

            Console.WriteLine("Goodbay!");
        }

        private void CreateNewTamagotchi()
        {
            string name = EnterName();
            try
            {
                new Tamagotchi(name).StartLife();
            }
            catch(DeathException ex)
            {
                Console.Clear();
                Console.WriteLine(ex.Message);
                Thread.Sleep(5000);
            }
        }

        private string EnterName()
        {
            Console.WriteLine("Please, enter name of the tamagotchi");
            string name = Console.ReadLine();

            return name;
        }

        private void ShowMenu()
        {
            Console.WriteLine("Tamagotchi game");
            Console.WriteLine("1 - start new game");
            Console.WriteLine("0 - exit");
            Console.WriteLine("Choose your action: ");
        }


    }
}
