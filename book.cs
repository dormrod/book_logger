using System;

namespace BookLogger
{

    public class Book
    {
        //Stores book information

        public string title = "";
        public string author = "";
        public string language = "";
        public string date = "";
        public int rating = 0;
        public int dbId = -1;

        public Book() { }

        public Book(Review review) 
		{
            //Make book record from GoodReads book
            title = review.book.title;
            rating = review.rating;
		}

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
            string userInput;
            string userDate = "";
            do
            {
                Console.Write(text + ": ");
                try
                {
                    userInput = Console.ReadLine();
                    userDate = "";
                    error = false;
                    if (userInput.Length < 9) error = true;
                    for (int i = 0; i < userInput.Length; ++i)
                    {
                        if (i > 9) break;
						else if(i==4 || i==7)
                        {
                            if (userInput[i] != '-') error = true;
						}
                        userDate += userInput[i];
					}
				}
                catch
                {
                    Console.WriteLine("Invalid entry!");
                    error = true;
				}

            } while (error);

            return userDate;
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