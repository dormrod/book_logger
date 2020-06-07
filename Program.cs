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

            //Present menu options until user quits
            bool quit = false;
            do
            {
                int option = UserOption();

                switch (option)
                {
                    case 0:
						{ 
							break;
						}
                    case 1:
						{
                            break;
                        }
                    default:
                        {
                            quit = true;
                            Console.WriteLine("See you next time");
							break;
						}
                }
            } while (!quit); 

        }

        static public int UserOption()
        {
            //Get user option from menu

            //Display menu until valid option selected
            bool validOption = false;
            int option=-1;
            do
            {
                int nOptions = DisplayOptions();
                try
                {
                    option = Convert.ToInt32(Console.ReadLine());
                    if (option > 0 && option <= nOptions) validOption = true;
                    else
                    {
                        Console.WriteLine("Invalid option");
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid option");
                }
            } while (!validOption);

            return option;
		}

        static public int DisplayOptions()
        {
            //Display menu options in command line 

            string[] options = { "Add book", "Quit" };
            int optionNumber = 1;

            Console.WriteLine("What would you like to do?");
            foreach (string option in options)
            {
                Console.WriteLine("{0}) {1}", optionNumber, option);
                ++optionNumber;
			}

            return 2;
		}


    }

}
