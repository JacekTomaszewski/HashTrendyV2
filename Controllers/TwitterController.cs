using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSharp;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class TwitterController : Controller
    {
        HashContext db = new HashContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TwitterTrends()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = "O5YRKrovfS42vADDPv8NdC4ZS",
                    ConsumerSecret = "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF",
                    AccessToken = "859793491941093376-kqRIYWY9bWyS10ATfqAVdwk1ZaxloEJ",
                    AccessTokenSecret = "hbOXipioFNcyOUyWbGdVAXvoVquETMl57AZUTcbMh3WRv"
                }
            };
            List<String> listTwitterStatus = new List<String>();
            TwitterContext twitterctx = new TwitterContext(auth);
            Models.Trend twittrend = new Models.Trend();
            var trends = (from trend in twitterctx.Trends
                          where trend.Type == TrendType.Place
                                && trend.WoeID == 23424923
                                // POLAND 23424923 , WOLRDWIDE 1;
                                && trend.SearchUrl.Substring(28, 3).Equals("%23")
                          select trend).ToList();
            if (trends != null &&
                trends.Any() &&
                trends.First().Locations != null
                )
            {
                ViewData["Lokacja"] = "Trendy wyszukiwane dla: " + trends.First().Locations.First().Name;
                trends.ForEach(trnd =>
                    listTwitterStatus.Add(trnd.Name));
            }
            string replaceHashtag;
            for (int i = 0; i < listTwitterStatus.Count; i++)
            {
                ViewData["MyList" + i] = listTwitterStatus[i].ToString();
                replaceHashtag = trends.ElementAt(i).Name.Replace("#", "%23");
                ViewData["MyLink" + i] = "/Hash/GetHashtag?hashtagname=" + replaceHashtag;
            }
            for (int i = 0; i < trends.Count; i++)
            {
                twittrend.TrendName = trends.ElementAt(i).Name;
                twittrend.DateCreated = DateTime.Now;
                db.Trends.Add(twittrend);
                db.SaveChanges();
            }
            return View(ViewData);
        }

        public ActionResult TwitterAuth()
        {

            string Key = "O5YRKrovfS42vADDPv8NdC4ZS";
            string Secret = "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF";

            TwitterService service = new TwitterService(Key, Secret);

            OAuthRequestToken requestToken = service.GetRequestToken("http://localhost:51577/Twitter/TwitterCallback");

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
            TempData["Name"] = user.Name;
            TempData["Userpic"] = user.ProfileImageUrl;
            TempData["Date"] = user.CreatedDate;
            TempData["Status"] = user;
            TempData["access"] = accessToken.Token;
            TempData["access secret"] = accessToken.TokenSecret;
            return View(TempData);

        }

        public void GetTwitterPosts(string hashtagname)
        {
            HashContext db = new HashContext();
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            Post twitterPost = new Post() { Hashtags = new List<Hashtag>() };
            List<TwitterStatus> listTwitterStatus = new List<TwitterStatus>();
            var service = new TwitterService("O5YRKrovfS42vADDPv8NdC4ZS", "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF");
            service.AuthenticateWith("859793491941093376-kqRIYWY9bWyS10ATfqAVdwk1ZaxloEJ", "hbOXipioFNcyOUyWbGdVAXvoVquETMl57AZUTcbMh3WRv");
            var twitterSearchResult = service.Search(new SearchOptions { Q = hashtagname, Count = 100, Resulttype = TwitterSearchResultType.Recent });
            if (twitterSearchResult != null)
            {
                listTwitterStatus = ((List<TwitterStatus>)twitterSearchResult.Statuses);
            }
            for (int i = 0; i < listTwitterStatus.Count; i++)
            {
                twitterPost.PostSource = "Twitter";
                twitterPost.Avatar = listTwitterStatus[i].User.ProfileImageUrl;
                twitterPost.Date = listTwitterStatus[i].User.CreatedDate;
                twitterPost.Username = listTwitterStatus[i].User.Name;
                twitterPost.ContentDescription = listTwitterStatus[i].Text;
                if (listTwitterStatus[i].Entities.Media.Count > 0)
                {
                    twitterPost.ContentImageUrl = listTwitterStatus[i].Entities.Media[0].MediaUrl;
                }
                //tutaj się wywala - powodem tego jest dodawanie do tabeli po raz drugi hashtaga o takiej samej nazwie. 
                //Nalezy wyszukać tego hashtaga w bazie danych, a następnie przypisać go do twitterPosta. 
                for (int x = 0; x < listTwitterStatus[i].Entities.HashTags.Count; x++)
                {
                        hashtag.HashtagName = listTwitterStatus[i].Entities.HashTags[x].Text;
                        hashtag.Posts.Add(twitterPost);
                        db.Hashtags.Add(hashtag);
                        db.SaveChanges();   
                }
                    db.Posts.Add(twitterPost);
                    db.SaveChanges();
            }
        }

    }
}
