using System;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;

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

        public void AddBook(Book book, string updateType, Logfile logfile)
        {
            //Add book to books table

            var command = new SQLiteCommand(connection);
            command.CommandText = string.Format("INSERT OR {0} INTO books(title, author, language, date, rating, missing_info, goodreads_id) VALUES(@title, @author, @language, @date, @rating, @missing_info, @goodreads_id);", updateType);
            command.Parameters.AddWithValue("@title", book.title);
            command.Parameters.AddWithValue("@author", book.author);
            command.Parameters.AddWithValue("@language", book.language);
            command.Parameters.AddWithValue("@date", book.date);
            command.Parameters.AddWithValue("@rating", book.rating);
            command.Parameters.AddWithValue("@missing_info", book.missing_info);
            command.Parameters.AddWithValue("@goodreads_id", book.goodreads_id);
            ExecuteSQLiteNonQuery(command, logfile);
		}

        public void DeleteBook(Book book, Logfile logfile)
        {
            //Delete book from books table

            var command = new SQLiteCommand(connection);
            command.CommandText = string.Format("DELETE FROM books WHERE id={0};",book.dbId);
            ExecuteSQLiteNonQuery(command, logfile);
		}

        public List<Book> SearchCL(Logfile logfile)
        {
            //Search books table using command line query

            //Get query from user
            Console.WriteLine("\nChoose keyword to search by");
            Menu queryMenu = new Menu(new string[] { "Title", "Author", "Language", "Rating", "Missing Info" }, new string[] { "title", "author", "language", "rating", "missing_info"});
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
                case "missing_info":
                    {
						query = string.Format("SELECT * FROM books WHERE {0}={1};", column, searchTerm);
                        break;
					}
			}

            //Search book DB
            List<Book> bookRecords = ExecuteSQLiteReader(query, logfile);
            return bookRecords;    
		}

        public List<Book> GetAllBooks(Logfile logfile)
        {
            //Get all books from books table

			string query = "SELECT * FROM books;";
            List<Book> bookRecords = ExecuteSQLiteReader(query, logfile);
            return bookRecords;
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
                ExecuteSQLiteNonQuery("CREATE TABLE books(id INTEGER PRIMARY KEY, title TEXT, author TEXT, language TEXT, date DATE, rating INTEGER, missing_info BIT, goodreads_id TEXT);", logfile);
                ExecuteSQLiteNonQuery("CREATE UNIQUE INDEX title_author on books(title, author);", logfile);
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

        public List<Book> ExecuteSQLiteReader(string commandText, Logfile logfile)
        { 
			//Execture SQL read command using string

            var command = new SQLiteCommand(connection);
            command.CommandText = commandText;
            return ExecuteSQLiteReader(command, logfile);
		}

        public List<Book> ExecuteSQLiteReader(SQLiteCommand command, Logfile logfile) 
		{
            //Execture SQL read command using command object

            logfile.WriteLine("SQL reader: ", command.CommandText);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Book> bookRecords = new List<Book>();
            while (reader.Read())
            {
                Book book = new Book();
                book.dbId = reader.GetInt32(0);
                book.title = reader.GetString(1);
                book.author = reader.GetString(2);
                book.language = reader.GetString(3);
                book.date = reader.GetString(4);
                book.rating = reader.GetInt32(5);
                book.missing_info = reader.GetBoolean(6);
                bookRecords.Add(book);
            }

            return bookRecords;
        }
    }
}
