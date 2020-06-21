using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Xml.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using System.Globalization;

namespace BookLogger
{
    public class GoodReadsInterface
    {
        //Class to interface with Good Reads via api

        RestClient client = new RestClient("https://www.goodreads.com/");
        string apiKey, apiSecret;
        string userId, userName;

        public GoodReadsInterface(Logfile logfile)
        {
            //Set up authentication with client keys and authentication

            //Set up OAuth1 with developer api tokens
            apiKey = Environment.GetEnvironmentVariable("goodreads_key");
            apiSecret = Environment.GetEnvironmentVariable("goodreads_secret");
            client.Authenticator = OAuth1Authenticator.ForRequestToken(apiKey, apiSecret);
            logfile.WriteLine("Developer key and token loaded");
            LogIn(logfile);
            Console.WriteLine("Welcome {0}!", userName);
        }

        public bool LogIn(Logfile logfile)
        {
            //Log in to GoodReads to authenticate user

            //Get OAuth tokens
            var oauthRequestToken = new RestRequest("oauth/request_token");
            var response = client.Execute(oauthRequestToken);
            var qs = HttpUtility.ParseQueryString(response.Content);
            var oauthToken = qs["oauth_token"];
            var oauthTokenSecret = qs["oauth_token_secret"];
            logfile.WriteLine("Goodreads API request:", oauthRequestToken.Body);

            //Construct url for user to login to
            var oauthAuthorise = new RestRequest("oauth/authorize");
            oauthAuthorise.AddParameter("oauth_token", oauthToken);
            oauthAuthorise.AddParameter("oauth_token_secret", oauthTokenSecret);
            response = client.Execute(oauthAuthorise);
            var url = client.BuildUri(oauthAuthorise).ToString();

            //Get user to login online
            Console.WriteLine("Please log in at: {0}\nPress any key to continue.", url);
            Console.ReadLine();
            client.Authenticator = OAuth1Authenticator.ForAccessToken(apiKey, apiSecret, oauthToken, oauthTokenSecret);
            var oauthAccessToken = new RestRequest("oauth/access_token");
            response = client.Execute(oauthAccessToken);
            if ((int)response.StatusCode != 200) return false;
            qs = HttpUtility.ParseQueryString(response.Content);
            var accessToken = qs["oauth_token"];
            var accessTokenSecret = qs["oauth_token_secret"];

            //Get user information
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(apiKey, apiSecret, accessToken, accessTokenSecret);
            var authUser = new RestRequest("api/auth_user", DataFormat.Xml);
            var authResponse = ExecuteGetRequest<AuthResponse>(authUser, logfile);
            userId = authResponse.Data.user.id;
            userName = authResponse.Data.user.name;

            return true;
        }

        public List<Book> GetAllBooks(Logfile logfile)
        {
            //Get all user books from goodreads account

            //Loop through record pages and extract books
            var books = new List<Book>();
            int page = 1;
            string endRecord, totalRecord;
            do
            {
                var request = new RestRequest(string.Format("review/list/{0}.xml", userId), DataFormat.Xml);
                request.AddParameter("v", 2);
                request.AddParameter("id", userId);
                request.AddParameter("shelf", "read");
                request.AddParameter("page", page);
                request.AddParameter("per_page", 200);
                request.AddParameter("key", apiKey);
                var response = ExecuteGetRequest<ShelfResponse>(request, logfile);

				foreach (Review review in response.Data.reviews.reviews)
				{
                    var book = new Book(review);
                    books.Add(book);
				}
                endRecord = response.Data.reviews.end;
                totalRecord = response.Data.reviews.end;
                ++page;
            } while (endRecord != totalRecord);

            return books;
		}

        public (List<bool> synced, List<Book> syncBooks) Sync(List<Book> syncBooks, Logfile logfile)
        {
            //Add books to goodreads which are not already present

            //Get all goodreads books to compare against
            List<Book> goodReadsBooks = GetAllBooks(logfile);
            var currentIds = new List<string>();
            foreach(Book book in goodReadsBooks) currentIds.Add(book.goodreads_id);

            //Add books sequentially
            var synced = new List<bool>();
            foreach(Book book in syncBooks)
            {
                //Search for book in goodreads to get id
                string bookId = Search(book, logfile);

                //Add book if not already synced from goodreads
                if (!currentIds.Contains(bookId))
                {
                    var request = new RestRequest("review.xml", Method.POST);
                    request.AddParameter("book_id", bookId);
                    if (book.rating != 0) request.AddParameter("review[rating]", book.rating);
                    if (book.date != "") request.AddParameter("review[read_at]", book.date);
                    request.AddParameter("shelf", "read");
                    var response = ExecutPostRequest(request, logfile);
                    if (response.IsSuccessful) Console.WriteLine("{0} synced with Goodreads", book.title);
                    book.goodreads_id = bookId; //update goodread ids
                    synced.Add(true);
                }
                else synced.Add(false);
			}
            return (synced, syncBooks);
		}

        public string Search(Book book, Logfile logfile)
        { 
			//Search goodreads and return book id

            var request = new RestRequest("search/index.xml", DataFormat.Xml);
            request.AddParameter("q", string.Format("{0} {1}", book.author, book.title));
            request.AddParameter("page", 1);
            request.AddParameter("key", apiKey);
            var response = ExecuteGetRequest<CatalogueSearch>(request, logfile);
            //Use custom deserialiser to handle non-escapted characters
            var deserialiser = new CustomXmlDeserialiser();
            CatalogueSearch data = deserialiser.DeserializeRegEx<CatalogueSearch>(response);
            return data.search.results.work[0].bestBook.id;
		}

        public IRestResponse<T> ExecuteGetRequest<T>(RestRequest request, Logfile logfile)
        { 
            logfile.WriteLine("Goodreads GET request:", client.BuildUri(request).ToString());
            request.XmlSerializer = new RestSharp.Serializers.DotNetXmlSerializer();
            var response = client.Execute<T>(request);
            return response;
		}
        
		public IRestResponse ExecutPostRequest(RestRequest request, Logfile logfile)
        { 
            logfile.WriteLine("Goodreads POST request:", client.BuildUri(request).ToString());
            var response = client.Execute(request);
            return response;
		}

    }

    //#### CUSTOM DESEREALISER AS TO REMOVE ESCAPE CHARACTERS ####
    class CustomXmlDeserialiser : DotNetXmlDeserializer
    {
        public T DeserializeRegEx<T>(IRestResponse response)
        {
            string pattern = @"&#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|7F|8[0-46-9A-F]9[0-9A-F])"; // XML 1.0
            //string pattern = @"#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|[19][0-9A-F]|7F|8[0-46-9A-F]|0?[1-8BCEF])"; // XML 1.1
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (regex.IsMatch(response.Content))
            {
                response.Content = regex.Replace(response.Content, String.Empty);
            }
            response.Content = response.Content.Replace("&;", string.Empty);

            return base.Deserialize<T>(response);
        }
    }

    //#### AUTH XML DESEREALISATION ####
    [XmlRoot("GoodreadsResponse")]
    public class AuthResponse
    {
        [XmlElement("user")]
        public AuthUser user { get; set; }
    }

    public class AuthUser
    {
        [XmlElement("id")]
        public string id { get; set; }
        [XmlElement("name")]
        public string name { get; set; }
    }
    
    //#### SHELF XML DESEREALISATION ####
    [XmlRoot("GoodreadsResponse")]
    public class ShelfResponse
    {
        [XmlElement("reviews")]
        public Reviews reviews { get; set; }
    }

    public class Reviews
    {
        [XmlElement("start")]
        public string start { get; set; }
        [XmlElement("end")]
        public string end { get; set; }
        [XmlElement("total")]
        public string total { get; set; }
        [XmlElement("review")]
        public List<Review> reviews { get; set; }
    }

    public class Review
    { 
		[XmlElement("book")]
        public ReviewBook book { get; set; }
        [XmlElement("rating")]
        public int rating { get; set; }
        [DeserializeAs(Name = "readat")]
        public string readAt { get; set; }
    }

    public class ReviewBook
    {
        [XmlElement("id")]
        public string id { get; set; }
        [DeserializeAs(Name = "titlewithoutseries")]
        public string titleWithoutSeries { get; set; }
        [XmlElement("authors")]
        public List<ReviewAuthor> authors { get; set; }
    }

    public class ReviewAuthor
    {
        [XmlElement("author")]
        public string name { get; set; }	
    }

    //#### SEARCH XML DESEREALISATION ####
    [XmlRoot("GoodreadsResponse")]
    public class CatalogueSearch
    { 
		[XmlElement("search")]
        public SearchBook search { get; set; }
    }

    public class SearchBook
    {
        [XmlElement("results")]
        public SearchResults results { get; set; }
    }

	public class SearchResults
	{
		[XmlElement("work")]
		public List<SearchWork> work { get; set; }
	}

	public class SearchWork
    {
        [XmlElement("best_book")]
        public SearchBookMatch bestBook { get; set; }
    }

    public class SearchBookMatch
    {
		[XmlElement("id")]
        public string id { get; set; }
    }


}

