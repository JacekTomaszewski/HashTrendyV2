using LinqToTwitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml.Linq;
using TweetSharp;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{

    public class HashController : Controller
    {
        HashContext db = new HashContext();

        public object DeviceId { get; private set; }



        public ActionResult Index()
        {

            return View(db.Devices.ToList());
        }



        public ActionResult GetCR7Hashtag()
        {
            string hashtagname = "#Cr7";
            string replaceHashtag = hashtagname.Replace("#", "%23");
            GetTwitterCR7Posts();
            GetGooglePlusPosts(replaceHashtag);
            RssReaderTVNandWYBORCZAtoDB(); //wszystkie najnowsze z tvn24&wyborcza
            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }


        public ActionResult GooglePlusAndTwitterView()
        {
            var PostsCount = (from x in db.Posts select x).Count();
            var AvatarList = (from x in db.Posts select x.Avatar).ToList();
            var DateList = (from x in db.Posts select x.Date).ToList();
            var UsernameList = (from x in db.Posts select x.Username).ToList();
            var ContentList = (from x in db.Posts select x.ContentDescription).ToList();
            var ContentImageUrlList = (from x in db.Posts select x.ContentImageUrl).ToList();
            var PostSourceList = (from x in db.Posts select x.PostSource).ToList();
            ViewData["PostsCount"] = PostsCount;
            if (PostsCount > 0)
            {
                for (int i = 0; i < PostsCount; i++)
                {
                    ViewData["Avatar" + i + 0] = AvatarList[i];
                    ViewData["Date" + i + 1] = DateList[i];
                    ViewData["Username" + i + 2] = UsernameList[i];
                    ViewData["Content" + i + 3] = ContentList[i];
                    ViewData["Image" + i + 4] = ContentImageUrlList[i];
                    ViewData["PostSource" + i + 5] = "Via: " + PostSourceList[i];
                }
            }



            return View(ViewData);
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
                    listTwitterStatus.Add("Name: " + trnd.Name));
            }
            string replaceHashtag;
            for (int i = 0; i < listTwitterStatus.Count; i++)
            {
                ViewData["MyList" + i] = listTwitterStatus[i].ToString();
                replaceHashtag = trends.ElementAt(i).Name.Replace("#", "%23");
                ViewData["MyLink" + i] = "/hash/twitterli?hashtagname=" + replaceHashtag;
            }
            for (int i = 0; i < trends.Count; i++)
            {
                GetTwitterPosts(trends.ElementAt(i).Name);
                GetGooglePlusPosts(trends.ElementAt(i).Name.Replace("#", "%23"));
                var duplicate = (from x in db.Trends where x.TrendName == trends.ElementAt(i).Name select x);
                if (duplicate == null)
                {
                    // GetTwitterPosts(trends.ElementAt(i).Name);       //TU PYTANIE - czy to ma tak działać? :)
                    twittrend.TrendName = trends.ElementAt(i).Name;
                    db.Trends.Add(twittrend);
                    db.SaveChanges();
                }
            }
            return View(ViewData);
        }

        public ActionResult Twitterli(string hashtagname)
        {
            //localhost:50707/hash/twitterli?hashtagname=%23nazwahashtaga
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            IEnumerable<string> tags;
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
                ViewData["Avatar" + i + 0] = listTwitterStatus[i].User.ProfileImageUrl;
                twitterPost.Avatar = listTwitterStatus[i].User.ProfileImageUrl;
                ViewData["Date" + i + 1] = listTwitterStatus[i].User.CreatedDate;
                twitterPost.Date = listTwitterStatus[i].User.CreatedDate;
                ViewData["Username" + i + 2] = listTwitterStatus[i].User.Name;
                twitterPost.Username = listTwitterStatus[i].User.Name;
                ViewData["Content" + i + 3] = listTwitterStatus[i].Text;
                twitterPost.ContentDescription = listTwitterStatus[i].Text;
                tags = Regex.Split(listTwitterStatus[i].Text, @"\s+").Where(b => b.StartsWith("#"));
                for (int x = 0; x < tags.Count(); x++)
                {
                    string selectedNameOfElement = tags.ElementAt(x);
                    var querySearchForHashtag = (from p in db.Hashtags where p.HashtagName == selectedNameOfElement select p);
                    if (querySearchForHashtag.Count() == 0)
                    {
                        hashtag.HashtagName = selectedNameOfElement;
                        hashtag.Posts.Add(twitterPost);
                        db.Hashtags.Add(hashtag);
                        db.SaveChanges();
                    }
                    else
                        twitterPost.Hashtags.Add(querySearchForHashtag.FirstOrDefault());
                }
                // nie moge znalezc direct linka twitterPost.DirectLinkToStatus = listTwitterStatus[i].
                // twitterPost.DirectLinkToStatus=listTwitterStatus[i].Entities.Urls.Value;  
                if (i < listTwitterStatus.Count)
                {
                    db.Posts.Add(twitterPost);
                    db.SaveChanges();
                }

            }


            return View(ViewData);
        }




        public void GetTwitterCR7Posts()
        {
            Post twitterPost = new Post();
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
            using (var context = new TwitterContext(auth))
            {
                var tweets =
                (from search in context.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "#cr7" && search.Count == 30
                 select search)
                .ToList();
                for (int i = 0; i < tweets.ElementAt(0).Statuses.Count; i++)
                {
                    twitterPost.PostSource = "Twitter";
                    if (tweets[0].Statuses[i].Entities.MediaEntities.Count > 0)
                    {
                        twitterPost.ContentImageUrl = tweets[0].Statuses[i].Entities.MediaEntities[0].MediaUrl;
                    }
                    twitterPost.Avatar = tweets[0].Statuses[i].User.ProfileImageUrl;
                    twitterPost.Date = tweets[0].Statuses[i].CreatedAt;
                    twitterPost.Username = tweets[0].Statuses[i].User.Name;
                    twitterPost.ContentDescription = tweets[0].Statuses[i].Text;
                    db.Posts.Add(twitterPost);
                    db.SaveChanges();
                }

            }
        }

        public void GetTwitterPosts(string hashtagname)
        {
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            IEnumerable<string> tags;
            Post twitterPost = new Post() { Hashtags = new List<Hashtag>() };
            twitterPost.PostSource = "Twitter";
            List<TwitterStatus> listTwitterStatus = new List<TwitterStatus>();
            var service = new TwitterService("O5YRKrovfS42vADDPv8NdC4ZS", "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF");
            service.AuthenticateWith("859793491941093376-kqRIYWY9bWyS10ATfqAVdwk1ZaxloEJ", "hbOXipioFNcyOUyWbGdVAXvoVquETMl57AZUTcbMh3WRv");
            var twitterSearchResult = service.Search(new SearchOptions { Q = "#cr7", Count = 100, Resulttype = TwitterSearchResultType.Recent });
            if (twitterSearchResult != null)

            {
                listTwitterStatus = ((List<TwitterStatus>)twitterSearchResult.Statuses);
            }
            for (int i = 0; i < listTwitterStatus.Count; i++)
            {
                twitterPost.Avatar = listTwitterStatus[i].User.ProfileImageUrl;
                twitterPost.Date = listTwitterStatus[i].User.CreatedDate;
                twitterPost.Username = listTwitterStatus[i].User.Name;
                twitterPost.ContentDescription = listTwitterStatus[i].Text;
                tags = Regex.Split(listTwitterStatus[i].Text, @"\s+").Where(b => b.StartsWith("#"));
                for (int x = 0; x < tags.Count(); x++)
                {
                    string selectedNameOfElement = tags.ElementAt(x);
                    var querySearchForHashtag = (from p in db.Hashtags where p.HashtagName == selectedNameOfElement select p);
                    if (querySearchForHashtag.Count() == 0)
                    {
                        hashtag.HashtagName = selectedNameOfElement;
                        hashtag.Posts.Add(twitterPost);
                        db.Hashtags.Add(hashtag);
                        db.SaveChanges();
                    }
                    else
                        twitterPost.Hashtags.Add(querySearchForHashtag.FirstOrDefault());
                }
                if (i < listTwitterStatus.Count)
                {
                    db.Posts.Add(twitterPost);
                    db.SaveChanges();
                }

            }
        }




        public void GetGooglePlusPosts(string hashtagname)
        {
            string result;
            string hashtagquery = hashtagname;
            string requestString = "https://www.googleapis.com/plus/v1/activities?query=" + hashtagquery + "&key=AIzaSyCXR0gFpvOpB0QmZs7qxHB7waGBFywchdA" + "&maxResults=20";
            WebRequest objWebRequest = WebRequest.Create(requestString);
            WebResponse objWebResponse = objWebRequest.GetResponse();
            Stream objWebStream = objWebResponse.GetResponseStream();
            using (StreamReader objStreamReader = new StreamReader(objWebStream))
            {
                result = objStreamReader.ReadToEnd();
            }

            GooglePlusPost post = JsonConvert.DeserializeObject<GooglePlusPost>(result);

            Post googlePost = new Post();
            googlePost.PostSource = "Google";
            //Hashtag hashtag = new Hashtag();
            ViewData["rozmiar"] = post.items.Count;
            for (int i = 0; i < post.items.Count; i++)
            {
                googlePost.Avatar = post.items[i].actor.image.url;
                googlePost.Date = System.DateTime.Parse(post.items[i].published);
                googlePost.Username = post.items[i].actor.displayName;
                googlePost.DirectLinkToStatus = post.items[i].url;
                //tags = regex.split(post.items[i].@object.attachments[0].content, @"\s+").where(b => b.startswith("#"));
                //for (int x = 0; x < tags.count(); x++)
                //{
                //    hashtag.hashtagname = tags.elementat(x);
                //    db.hashtags.add(hashtag);
                //    db.savechanges();
                //    pomocy z foreign key !!! - duplikaty
                //}
                googlePost.ContentDescription = post.items[i].title;
                //if (post.items[i].@object.attachments[0].image != null)
                //{
                //   googlePost.ContentImageUrl = post.items[i].@object.attachments[0].image.url;
                //}
                db.Posts.Add(googlePost);
                db.SaveChanges();
            }
        }


        public ActionResult TwitterGooglePlusView()
        {
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            IEnumerable<string> tags;
            Post twitterPost = new Post() { Hashtags = new List<Hashtag>() };
            List<TwitterStatus> listTwitterStatus = new List<TwitterStatus>();
            var service = new TwitterService("O5YRKrovfS42vADDPv8NdC4ZS", "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF");
            service.AuthenticateWith("859793491941093376-kqRIYWY9bWyS10ATfqAVdwk1ZaxloEJ", "hbOXipioFNcyOUyWbGdVAXvoVquETMl57AZUTcbMh3WRv");
            var twitterSearchResult = service.Search(new SearchOptions { Q = (from x in db.Trends where x.DateCreated > DateTime.Now.AddHours(-1) select x.TrendName).ToString(), Count = 10, Resulttype = TwitterSearchResultType.Recent });
            if (twitterSearchResult != null)
            {
                listTwitterStatus = ((List<TwitterStatus>)twitterSearchResult.Statuses);
            }
            for (int i = 0; i < listTwitterStatus.Count; i++)
            {
                ViewData["Avatar" + i + 0] = listTwitterStatus[i].User.ProfileImageUrl;
                twitterPost.Avatar = listTwitterStatus[i].User.ProfileImageUrl;
                ViewData["Date" + i + 1] = listTwitterStatus[i].User.CreatedDate;
                twitterPost.Date = listTwitterStatus[i].User.CreatedDate;
                ViewData["Username" + i + 2] = listTwitterStatus[i].User.Name;
                twitterPost.Username = listTwitterStatus[i].User.Name;
                ViewData["Content" + i + 3] = listTwitterStatus[i].Text;
                twitterPost.ContentDescription = listTwitterStatus[i].Text;
                tags = Regex.Split(listTwitterStatus[i].Text, @"\s+").Where(b => b.StartsWith("#"));
                for (int x = 0; x < tags.Count(); x++)
                {
                    string selectedNameOfElement = tags.ElementAt(x);
                    var querySearchForHashtag = (from p in db.Hashtags where p.HashtagName == selectedNameOfElement select p);
                    if (querySearchForHashtag.Count() == 0)
                    {
                        hashtag.HashtagName = selectedNameOfElement;
                        hashtag.Posts.Add(twitterPost);
                        db.Hashtags.Add(hashtag);
                        db.SaveChanges();
                    }
                    else
                        twitterPost.Hashtags.Add(querySearchForHashtag.FirstOrDefault());
                }
                // nie moge znalezc direct linka twitterPost.DirectLinkToStatus = listTwitterStatus[i].
                // twitterPost.DirectLinkToStatus=listTwitterStatus[i].Entities.Urls.Value;  
                if (i < listTwitterStatus.Count)
                {
                    db.Posts.Add(twitterPost);
                    db.SaveChanges();
                }

            }

            return View(ViewData);
        }






        public ActionResult TwitterAuth()
        {

            string Key = "O5YRKrovfS42vADDPv8NdC4ZS";
            string Secret = "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF";

            TwitterService service = new TwitterService(Key, Secret);

            OAuthRequestToken requestToken = service.GetRequestToken("http://localhost:51577/Hash/TwitterCallback");

            Uri uri = service.GetAuthenticationUrl(requestToken);

            return Redirect(uri.ToString());
        }
        public ActionResult TwitterExample(string hashtagname)
        {
            //localhost:50707/hash/twitterli?hashtagname=%23nazwahashtaga
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            IEnumerable<string> tags;
            Post twitterPost = new Post() { Hashtags = new List<Hashtag>() };
            List<TwitterStatus> listTwitterStatus = new List<TwitterStatus>();
            var service = new TwitterService("O5YRKrovfS42vADDPv8NdC4ZS", "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF");
            service.AuthenticateWith("859793491941093376-kqRIYWY9bWyS10ATfqAVdwk1ZaxloEJ", "hbOXipioFNcyOUyWbGdVAXvoVquETMl57AZUTcbMh3WRv");
            var twitterSearchResult = service.Search(new SearchOptions { Q = hashtagname, Count = 5, Resulttype = TwitterSearchResultType.Recent });
            if (twitterSearchResult != null)

            {
                listTwitterStatus = ((List<TwitterStatus>)twitterSearchResult.Statuses);
            }
            for (int i = 0; i < listTwitterStatus.Count; i++)
            {
                ViewData["Content" + i + 3] = listTwitterStatus[i].Text;
                tags = Regex.Split(listTwitterStatus[i].Text, @"\s+").Where(b => b.StartsWith("#"));
                for (int x = 0; x < tags.Count(); x++)
                {
                    string selectedNameOfElement = tags.ElementAt(x);
                    var querySearchForHashtag = (from p in db.Hashtags where p.HashtagName == selectedNameOfElement select p);
                    foreach (var item in querySearchForHashtag)
                    { ViewData["Content" + x + 3] = item.HashtagName; }
                }

            }


            return View(ViewData);
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

        public ActionResult RssReaderTVN()
        {
            IEnumerable<Post> TVN24RssFeeds = Operation.RssOperation.RssReaderTVN24.GetRssFeed();
            for (int i = 0; i < TVN24RssFeeds.Count(); i++)
            {
                db.Posts.Add(TVN24RssFeeds.ElementAt(i));
            }
            db.SaveChanges();
            return View(Operation.RssOperation.RssReaderTVN24.GetRssFeed());
        }
        public ActionResult RssReaderWyborcza()
        {
            IEnumerable<Post> WyborczaRssFeeds = Operation.RssOperation.RssReaderWyborcza.GetRssFeed();
            for (int i = 0; i < WyborczaRssFeeds.Count(); i++)
            {
                db.Posts.Add(WyborczaRssFeeds.ElementAt(i));
            }
            db.SaveChanges();
            return View(Operation.RssOperation.RssReaderWyborcza.GetRssFeed());
        }

        public void RssReaderTVNandWYBORCZAtoDB()
            {
            IEnumerable<Post> WyborczaRssFeeds = Operation.RssOperation.RssReaderWyborcza.GetRssFeed();
            for (int i = 0; i < WyborczaRssFeeds.Count(); i++)
            {
                db.Posts.Add(WyborczaRssFeeds.ElementAt(i));
            }
            IEnumerable<Post> TVN24RssFeeds = Operation.RssOperation.RssReaderTVN24.GetRssFeed();
            for (int i = 0; i < TVN24RssFeeds.Count(); i++)
            {
                db.Posts.Add(TVN24RssFeeds.ElementAt(i));
            }
            db.SaveChanges();
        }

        public ActionResult Create(String imei)
        {
            Device device = new Device();
            ViewBag.imei = imei;
            device.DeviceUniqueId = imei;
            db.Devices.Add(device);
            db.SaveChanges();
            return View();
        }

        public ActionResult Delete(int id = 0)
        {
            Device DeviceId = db.Devices.Find(id);
            if (DeviceId == null)
            {
                return HttpNotFound();
            }
            return View(DeviceId);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id = 0)
        {
            Device DeviceId = db.Devices.Find(id);
            db.Devices.Remove(DeviceId);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }

}