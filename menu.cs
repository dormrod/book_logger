using System;

namespace BookLogger
{

    public class Menu
    {
        //Base menu class

        //Menu options
        int nOptions; //number of options
        string[] options = { }; //options to display

        //Constructors
        public Menu() { }
        public Menu(string[] menuOptions)
        {
            options = menuOptions;
            nOptions = menuOptions.Length;
		}

        //Get user menu selection in command line
        public int GetUserOptionCL()
        {

            //Display menu until valid option selected
            bool error;
            int option = -1;
            do
            {
                DisplayCL();
                var res = ReadOptionCL();
                error = res.error;
                if (error) {
                    Console.WriteLine("Invalid input");
				}
                else option = res.option;
            } while (error);

            return option;
		}

		//Display menu options to command line
        public void DisplayCL() 
		{
            int i = 1;
            foreach (string option in options)
            {
                Console.WriteLine("{0}) {1}", i, option);
                ++i;
            }
        }

        //Read user option from command line
        public (bool error, int option) ReadOptionCL()
        {

            bool error; //option is invalid or not
            int option = -1; //placeholder for invalid option
            try
            {
                option = Convert.ToInt32(Console.ReadLine());
                if (option > 0 && option <= nOptions) error = false;
                else error = true;
            }
            catch
            {
                error = true;
            }

            return (error, option);
        }

    }

}



