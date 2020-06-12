﻿using System;
using System.IO;
using System.Data.SQLite;
using System.Collections;

namespace BookLogger
{

    public class BookDB
    {
        //Manages SQLite DB for books

        SQLiteConnection connection;

        public BookDB(Logfile logfile, string dbPath = "")
        {
            //Constructor opens DB connection

            //Set default path as current directory 
            if (dbPath == "")
            {
                string pwd = Directory.GetCurrentDirectory();
                dbPath += pwd + "/.book_logger.db";
            }
            string connectionCommand = "Data Source=" + dbPath + ";";

            //Open connection
            connection = new SQLiteConnection(connectionCommand);
            connection.Open();
            logfile.WriteLine("Database connection opened at: ", dbPath);

            //Initialise table
            InitialiseTable(logfile);
        }

        public void AddBook(Book book, Logfile logfile)
        {
            //Add book to books table

            using var command = new SQLiteCommand(connection);
            command.CommandText = "INSERT INTO books(title, author, language, date, rating) VALUES(@title, @author, @language, @date, @rating);";
            command.Parameters.AddWithValue("@title", book.title);
            command.Parameters.AddWithValue("@author", book.author);
            command.Parameters.AddWithValue("@language", book.language);
            command.Parameters.AddWithValue("@date", book.date);
            command.Parameters.AddWithValue("@rating", book.rating);
            ExecuteSQLiteNonQuery(command, logfile);
		}

        public void SearchCL(Logfile logfile)
        {
            //Search books table using command line query

            //Get query from user
            Console.WriteLine("\nChoose keyword to search by");
            Menu queryMenu = new Menu(new string[] { "Title", "Author", "Language", "Rating" }, new string[] { "title", "author", "language", "rating" });
            string column = queryMenu.GetUserOptionCL();
            Console.Write("Enter {0}: ", column);
            string searchTerm = Console.ReadLine();
            string query="";
            switch (column)
            {
                case "title":
                case "author":
                case "language":
                    {
						query = string.Format("SELECT * FROM books WHERE {0}='{1}';", column, searchTerm);
                        break; 
					}
                case "rating":
                    {
						query = string.Format("SELECT * FROM books WHERE {0}={1};", column, searchTerm);
                        break;
					}
			}

            //Search book DB
            ArrayList bookRecords = ExecuteSQLiteReader(query, logfile);
            Console.WriteLine("{0} records found", bookRecords.Count);
            Console.WriteLine("Press enter to display");
            for(int i=0; i<bookRecords.Count; ++i)
            {
                Console.ReadLine();
                Console.WriteLine(bookRecords[i]);
			}
		}

        public void InitialiseTable(Logfile logfile)
        { 
            //Create empty book SQL table

			//Create table if it does not already exist
            var command = new SQLiteCommand(connection);
            command.CommandText = "SELECT name FROM sqlite_master WHERE name = 'books';";
            logfile.WriteLine("SQL command: ", command.CommandText);
            var name = command.ExecuteScalar();
			if (name == null)
            {
                ExecuteSQLiteNonQuery("CREATE TABLE books(id INTEGER PRIMARY KEY, title TEXT, author TEXT, language TEXT, date DATE, rating INTEGER);", logfile);
            }
		}

        public void ResetTable(Logfile logfile)
        {
            //Delete book table and reinitialise

            ExecuteSQLiteNonQuery("DROP TABLE IF EXISTS books;", logfile);
            InitialiseTable(logfile);
		}

		public void ExecuteSQLiteNonQuery(string commandText, Logfile logfile)
        {
            //Exectute SQL non-query command using string

            var command = new SQLiteCommand(connection);
            command.CommandText = commandText;
            ExecuteSQLiteNonQuery(command,logfile);
		}

        public void ExecuteSQLiteNonQuery(SQLiteCommand command, Logfile logfile)
        {
            //Exectute SQL non-query command using command object

            logfile.WriteLine("SQL non-query: ", command.CommandText);
            command.ExecuteNonQuery();		
		}

        public ArrayList ExecuteSQLiteReader(string commandText, Logfile logfile)
        { 
			//Execture SQL read command using string

            var command = new SQLiteCommand(connection);
            command.CommandText = commandText;
            return ExecuteSQLiteReader(command, logfile);
		}

        public ArrayList ExecuteSQLiteReader(SQLiteCommand command, Logfile logfile) 
		{
            //Execture SQL read command using command object

            logfile.WriteLine("SQL reader: ", command.CommandText);
            SQLiteDataReader reader = command.ExecuteReader();

            ArrayList bookRecords = new ArrayList();
            while (reader.Read())
            {
                Book book = new Book();
                book.title = reader.GetString(1);
                book.author = reader.GetString(2);
                book.language = reader.GetString(3);
                book.date = reader.GetString(4);
                book.rating = reader.GetInt32(5);
                bookRecords.Add(book);
            }

            return bookRecords;
        }
    }
}
