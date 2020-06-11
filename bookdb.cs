using System;
using System.IO;
using System.Data.SQLite;
using System.Data.SqlClient;

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

            string command = "INSERT INTO books(title, author, language, date, rating) VALUES(";
            command += "'" + book.title + "', ";
            command += "'" + book.author + "', ";
            command += "'" + book.language + "', ";
            command += "'" + book.date + "', ";
            command += book.rating + ")";
            ExecuteSQLiteCommand(command, logfile);
		}

        public void InitialiseTable(Logfile logfile)
        { 
            //Create empty book SQL table

			//Create table if it does not already exist
            using var command = new SQLiteCommand(connection);
            command.CommandText = "SELECT name FROM sqlite_master WHERE name = 'books'";
            logfile.WriteLine("SQL command: ", command.CommandText);
            var name = command.ExecuteScalar();
			if (name == null)
            {
                ExecuteSQLiteCommand("CREATE TABLE books(id INTEGER PRIMARY KEY, title TEXT, author TEXT, language TEXT, date DATE, rating INTEGER)", logfile);
            }
		}

        public void ResetTable(Logfile logfile)
        {
            //Delete book table and reinitialise

            ExecuteSQLiteCommand("DROP TABLE IF EXISTS books", logfile);
            InitialiseTable(logfile);
		}

		public void ExecuteSQLiteCommand(string commandText, Logfile logfile)
        {
            //Exectute SQL command using string

            using var command = new SQLiteCommand(connection);
            command.CommandText = commandText;
            ExecuteSQLiteCommand(command,logfile);
		}

        public void ExecuteSQLiteCommand(SQLiteCommand command, Logfile logfile)
        {
            //Exectute SQL command using command object

            logfile.WriteLine("SQL command: ", command.CommandText);
            command.ExecuteNonQuery();		
		}

    }
}
