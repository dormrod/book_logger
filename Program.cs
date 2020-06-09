using System;

namespace BookLogger
{

    class Program
    {
        //App to store user information about books they have read

        static void Main(string[] args)
        {

            //Display welcome message
            Console.WriteLine("Welcome to your Book Logger");

            //Initialise menus
            Menu mainMenu = new Menu(new string[] { "Add Book", "Quit" });

            //Initialise logfile
            Logfile logfile = new Logfile();



            //Run until user quits
            bool quit = false;
            do
            {
                //Main menu
                int option = mainMenu.GetUserOptionCL();
                switch (option)
                {
                    case 1:
                        {
                            Console.WriteLine("XXX");
                            break;
                        }
                    case 2:
                        {
                            quit = true;
                            Console.WriteLine("See you next time!");
                            break;
                        }
                }
            } while (!quit);

        }
    
    }

}
