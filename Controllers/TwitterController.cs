using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TweetSharp;
using WebApiHash.Context; 
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class TwitterController : Controller
    {
       static HashContext db = new HashContext();
        public static SingleUserAuthorizer auth = new SingleUserAuthorizer
        {
            CredentialStore = new SingleUserInMemoryCredentialStore
            {
                ConsumerKey = "O5YRKrovfS42vADDPv8NdC4ZS",
                ConsumerSecret = "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF",
                AccessToken = "859793491941093376-kqRIYWY9bWyS10ATfqAVdwk1ZaxloEJ",
                AccessTokenSecret = "hbOXipioFNcyOUyWbGdVAXvoVquETMl57AZUTcbMh3WRv"
            }
        };

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult TwitterTrends()
        {
            DateTime now = DateTime.Now.AddHours(-1.1);
            var result = (from m in db.Trends
                                      where m.DateCreated > now
                                      select m).ToList();
            return View(result);
        }


        public static void TwitterTrendstoDB()
        {
            Models.Trend trendModel = new Models.Trend();
            TwitterContext twitterctx = new TwitterContext(auth);
            var trends = (from trend in twitterctx.Trends
                          where trend.Type == TrendType.Place
                                && trend.WoeID == 23424923
                                && trend.SearchUrl.Substring(28, 3).Equals("%23")
                          select trend).ToList();
            if (trends != null &&
                trends.Any() &&
                trends.First().Locations != null
                )
            for (int i = 0; i < trends.Count; i++)
            {
                trendModel.TrendName = PostController.RemoveDiacricts(trends.ElementAt(i).Name);
                trendModel.DateCreated = DateTime.Now;
                db.Trends.Add(trendModel);
                db.SaveChanges();
            }
        }


        public ActionResult TwitterAuth()
        {
            TwitterService service = new TwitterService(auth.CredentialStore.ConsumerKey, auth.CredentialStore.ConsumerSecret);

            OAuthRequestToken requestToken = service.GetRequestToken("http://localhost:50707/Twitter/TwitterCallback");

            Uri uri = service.GetAuthenticationUrl(requestToken);

            return Redirect(uri.ToString());
        }


        public ActionResult TwitterCallback(string oauth_token, string oauth_verifier)
        {
            var requestToken = new OAuthRequestToken { Token = oauth_token };
            string Key = "O5YRKrovfS42vADDPv8NdC4ZS";
            string Secret = "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF";
            TwitterService service = new TwitterService(Key, Secret);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);
            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            VerifyCredentialsOptions option = new VerifyCredentialsOptions();
            TwitterUser user = service.VerifyCredentials(option);
            ViewBag.AccessToken = accessToken.Token;
            ViewBag.AccessTokenSecret = accessToken.TokenSecret;
            return View();
        }

       public static void GetTwitterPosts(string hashtagname)
        {
            TwitterContext twitterCtx = new TwitterContext(TwitterController.auth);
            var searchResponse =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == hashtagname &&
                       search.ResultType == ResultType.Recent &&
                       search.TweetMode == TweetMode.Extended &&
                       search.Count == 20
                 select search).ToList();

            List<string> ListOfHashtags = new List<string>();

            for (int i = 0; i < searchResponse[0].Statuses.Count; i++)
            {
                for (int x = 0; x < searchResponse[0].Statuses[i].Entities.HashTagEntities.Count; x++)
                    ListOfHashtags.Add(searchResponse[0].Statuses[i].Entities.HashTagEntities[x].Tag);

                PostController.DeserializertoDB("Twitter", searchResponse[0].Statuses[i].User.ProfileImageUrl, searchResponse[0].Statuses[i].CreatedAt,
                    searchResponse[0].Statuses[i].User.Name, searchResponse[0].Statuses[i].FullText,
                    searchResponse[0].Statuses[i].Entities.MediaEntities.Count > 0 ?
                searchResponse[0].Statuses[i].Entities.MediaEntities[0].MediaUrl : "", "https://twitter.com/statuses/"+searchResponse[0].Statuses[i].StatusID, ListOfHashtags);
            };
        }

    }

    }