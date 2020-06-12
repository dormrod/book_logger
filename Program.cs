using System;

namespace BookLogger
{

    class Program
    {
        //App to store user information about books they have read

        static void Main(string[] args)
        {
            //Initialise logfile
            Logfile logfile = new Logfile();
            logfile.WriteLine("Book Logger begun");

            //Display welcome message
            Console.WriteLine("Welcome to your Book Logger");

            //Initialise menus
            Menu mainMenu = new Menu(new string[] { "Add Book", "Edit Book", "Search Books", "Hard Reset", "Quit" }, new string[] { "add", "edit", "search", "reset", "quit" });
            logfile.WriteLine("Menus initialised");

            //Initialise DB
            BookDB bookDB = new BookDB(logfile);

            //Run until user quits
            bool quit = false;
            string option;
            do
            {
                //Main menu
                option = mainMenu.GetUserOptionCL();
                switch (option)
                {
                    case "add":
                        {
                            Book book = new Book();
                            book.UserInputCL();
                            bookDB.AddBook(book,logfile);
                            Console.WriteLine("\nYou added the book: \n{0}", book);
                            break;
                        }
                    case "edit":
						{

                            break;
						}
                    case "search":
                        {
                            bookDB.SearchCL(logfile);
                            break;
						}
                    case "reset":
                        {
                            bookDB.ResetTable(logfile);
                            break;
						}
                    case "quit":
                        {
                            Console.WriteLine("See you next time!");
                            quit = true;
                            break;
                        }
                }
            } while (!quit);
        }
    
    }

}
