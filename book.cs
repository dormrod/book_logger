﻿using System;
using System.Globalization;

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
        public bool missing_info = true;
        public string goodreads_id = "";

        public Book() { }

        public Book(Review review) 
		{
            //Make book record from GoodReads book
            title = review.book.titleWithoutSeries;
            rating = review.rating;
            author = "";
            date = "";
            foreach(ReviewAuthor reviewAuthor in review.book.authors)
            {
                author += reviewAuthor.name;
            }
			try
            {
                string dateTmp = "";
                for(int i=0; i<review.readAt.Length; ++i)
                {
                    if (i < 20 || i > 25) dateTmp += review.readAt[i];
				}
                DateTime dt = DateTime.ParseExact(dateTmp,"ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
                date = dt.ToString("yyyy-MM-dd");
            }
            catch
            {
                date = ""; 
			}
            goodreads_id = review.book.id;
            CheckInfo();
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
            CheckInfo();
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

        public void CheckInfo()
        {
            //Check book has no missing information

            missing_info = false;
            if (title == "") missing_info = true;
            if (author == "") missing_info = true;
            if (language == "") missing_info = true;
            if (date == "") missing_info = true;
            if (rating == 0) missing_info = true;
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