using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using RestSharp;
using RestSharp.Authenticators;

namespace BookLogger
{
    public class GoodReadsInterface
    {
        //Class to interface with Good Reads via api

        RestClient client = new RestClient("https://www.goodreads.com/");
        string apiKey, apiSecret;
        string userId, userName;

        public GoodReadsInterface()
        {
            //Set up authentication with client keys and authentication

            //Set up OAuth1 with developer api tokens
            apiKey = Environment.GetEnvironmentVariable("goodreads_key");
            apiSecret = Environment.GetEnvironmentVariable("goodreads_secret");
            client.Authenticator = OAuth1Authenticator.ForRequestToken(apiKey, apiSecret);
            LogIn();

        }

        public bool LogIn()
        {
            //Log in to GoodReads to authenticate user

            //Get OAuth tokens
			var oauthRequestToken = new RestRequest("oauth/request_token");
            var response = client.Execute(oauthRequestToken);
            var qs = HttpUtility.ParseQueryString(response.Content);
            var oauthToken = qs["oauth_token"];
            var oauthTokenSecret = qs["oauth_token_secret"];

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

            //Get user id
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(apiKey, apiSecret, accessToken, accessTokenSecret);
            var authUser = new RestRequest("api/auth_user");
            response = client.Execute(authUser);
            qs = HttpUtility.ParseQueryString(response.Content);
            userId = qs["user id"];
            userName = qs["name"];
            Console.WriteLine(qs);

            Console.WriteLine(userName);
            Console.WriteLine(userId);


            return true;
		}



    }
}


