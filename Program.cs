using System;
using System.Collections.Generic;

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
            Menu mainMenu = new Menu(new string[] { "Add Book", "Delete Book", "Search Books", "Show All", "Hard Reset", "Quit" }, new string[] { "add", "delete", "search", "show", "reset", "quit" });
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
                    case "search":
                        {
                            List<Book> searchResults = bookDB.SearchCL(logfile);
							Console.WriteLine("{0} records found", searchResults.Count);
							Console.WriteLine("Press enter to display");
							for(int i=0; i<searchResults.Count; ++i)
							{
								Console.ReadLine();
								Console.WriteLine(searchResults[i]);
							}
                            break;
						}
                    case "delete":
                        {
                            List<Book> searchResults = bookDB.SearchCL(logfile);
							Console.WriteLine("{0} records found", searchResults.Count);
                            Console.WriteLine("Choose book to delete");
                            Book bookToDelete = new Book();
                            bool bookSelected = false;
							foreach (Book book in searchResults)
							{
								Console.WriteLine(book);
								Console.WriteLine("Would you like to delete this book?");
                                string response = Console.ReadLine();
                                if(response=="y" || response == "Y" || response == "yes" || response == "Yes") 
								{
                                    bookToDelete = book;
                                    bookSelected = true;
                                    break;
								}
							}
                            if (bookSelected)
                            {
                                bookDB.DeleteBook(bookToDelete,logfile);
                                Console.WriteLine("Book deleted.");
                            }
                            else Console.WriteLine("No book selected for deletion.");
                            break;
						}
                    case "show":
                        {
                            List<Book> searchResults = bookDB.GetAllBooks(logfile);
							Console.WriteLine("{0} records found", searchResults.Count);
							Console.WriteLine("Press enter to display");
							for(int i=0; i<searchResults.Count; ++i)
							{
								Console.ReadLine();
								Console.WriteLine(searchResults[i]);
							}
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
