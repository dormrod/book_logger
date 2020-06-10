using System;

namespace BookLogger
{

    public class Book
    {
        //Stores book information

        private string title = "";
        private string author = "";
        private string language = "";
        private string date = "";
        private int rating = -1;

        public Book() { }

        public void UserInputCL()
        {
            //User input from command line

            Console.WriteLine("\nEnter the details of your book:");
            title = StringUserInputCL("Title");
            author = StringUserInputCL("Author");
            language = StringUserInputCL("Language");
            date = DateUserInputCL("Date (yyyy-mm-dd)");
            rating  = IntUserInputCL("Rating");
			
			
		}

        public string StringUserInputCL(string text)
        {
            //Get user string from command line

            bool error;
            string userInput="";
            do
            {
                Console.Write(text + ": ");
                try
                {
                    userInput = Console.ReadLine();
                    if (userInput == "") 
					{
						Console.WriteLine("Invalid entry!");
                        error = true;
					}
                    else error = false;
				}
                catch
                {
                    Console.WriteLine("Invalid entry!");
                    error = true;
				}

            } while (error);

            return userInput;
		}
        
		public string DateUserInputCL(string text)
        {
            //Get user string from command line

            bool error;
            string userInput="";
            do
            {
                Console.Write(text + ": ");
                try
                {
                    userInput = Console.ReadLine();
                    if (userInput.Length!=10) 
					{
						Console.WriteLine("Invalid entry!");
                        error = true;
					}
                    else if (userInput[4]!='-' || userInput[7] != '-') 
					{ 
						Console.WriteLine("Invalid entry!");
                        error = true;
					}
                    else error = false;
				}
                catch
                {
                    Console.WriteLine("Invalid entry!");
                    error = true;
				}

            } while (error);

            return userInput;
		}
        
		public int IntUserInputCL(string text)
        {
            //Get user string from command line

            bool error;
            int userInput=-1;
            do
            {
                Console.Write(text + ": ");
                try
                {
                    userInput = Convert.ToInt32(Console.ReadLine());
                    if (userInput < 0) 
					{
						Console.WriteLine("Invalid entry!");
                        error = true;
					}
                    else error = false;
				}
                catch
                {
                    Console.WriteLine("Invalid entry!");
                    error = true;
				}

            } while (error);

            return userInput;
		}

        public override string ToString()
        {
            //Provide book details

            string book = "";
            book += "Title: " + title + "\n";
            book += "Author: " + author + "\n";
            book += "Language: " + language + "\n";
            book += "Date: " + date + "\n";
            book += "Rating: " + rating + "\n";

            return book;
        }
    }

}