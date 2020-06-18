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

        public IRestResponse<T> ExecuteGetRequest<T>(RestRequest request, Logfile logfile)
        { 
            logfile.WriteLine("Goodreads GET request:", client.BuildUri(request).ToString());
            request.XmlSerializer = new RestSharp.Serializers.DotNetXmlSerializer();
            var response = client.Execute<T>(request);
            return response;
		}

    }

    [XmlRoot("GoodreadsResponse")]
    public class AuthResponse
    {
        [XmlElement("user")]
        public User user { get; set; }
    }

    public class User
    {
        [XmlElement("id")]
        public string id { get; set; }
        [XmlElement("name")]
        public string name { get; set; }
    }
    
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
        [XmlIgnore]
        public DateTime date;
        [XmlElement("read_at")]
        public string dateString
        {
            get { return this.date.ToString("yyyy-MM-dd"); }
            set 
			{
                Console.WriteLine("XXXXXXX");
                Console.WriteLine(value);
				DateTime.TryParseExact(value, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out this.date); 
			}
        }

        //        if (DateTime.TryParseExact(date, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
    }

    public class ReviewBook
    {
        [XmlElement("title_without_series")]
        public string title { get; set; }
        [XmlElement("authors")]
        public List<ReviewAuthor> authors { get; set; }
    }

    public class ReviewAuthor
    {
        [XmlElement("author")]
        public string name { get; set; }	
    }

}

