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
            GetTwitterPosts("#cr7");
            GetGooglePlusPosts(replaceHashtag);
            RssReadertoDB(); //wszystkie najnowsze z tvn24&wyborcza
           return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }



        public ActionResult GetHashtag(string hashtagname)
        {
            GetGooglePlusPosts(hashtagname.Replace("#", "%23"));
            GetTwitterPosts(hashtagname);
            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }


        public ActionResult PostsView()
        {
            var Posts = db.Posts.ToList();
            return View(Posts);
        }

        public ActionResult SpecificPostsView(string hashtagname)
        {
            // var Posts = (from p in db.Hashtags where p.HashtagName == hashtagname select p.Posts);
            // var Posts = db.Posts.Where(w => w.Hashtags.Any(s => s.HashtagName == hashtagname)).ToList();
            // var Posts = (from g in db.Posts join m in db.Hashtags on g.PostId equals m.Posts where m.HashtagName == hashtagname select g).ToList();
            //var Posts = from e in db.Posts.Include("hashta") 
            //            where e.EmployeeId == someId
            //            select e;
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
                ViewData["MyLink" + i] = "/hash/GetHashtag?hashtagname=" + replaceHashtag;
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

        public void GetTwitterPosts(string hashtagname)
        {
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
                for (int x = 0; x < listTwitterStatus[i].Entities.HashTags.Count; x++)
                {
                    try
                    { 
                    hashtag.HashtagName = listTwitterStatus[i].Entities.HashTags[x].Text;
                    hashtag.Posts.Add(twitterPost);
                    db.Hashtags.Add(hashtag);
                    db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                       // var 
                        //string helperString = listTwitterStatus[i].Entities.HashTags[0].Text;
                        //var findHashtag1 = db.Hashtags.Where(f => f.HashtagName == helperString).ToList();
                        //int z = findHashtag1.ElementAt(0).HashtagId;
                        //var findHashtag2 = db.Hashtags.Find(z);
                        //findHashtag2.Posts.Add(twitterPost);
                    }
                }

                try
                { 
                    db.Posts.Add(twitterPost);
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    //if (db.Posts.Local != null)
                    //{
                    //    db.Posts.Local.Clear();
                    //}
                }
            }
        }




        public void GetGooglePlusPosts(string hashtagname)
        {
            string result;
            string requestString = "https://www.googleapis.com/plus/v1/activities?query=" + hashtagname + "&key=AIzaSyCXR0gFpvOpB0QmZs7qxHB7waGBFywchdA" + "&maxResults=20";
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
            Hashtag hashtag = new Hashtag();
            IEnumerable<string> tags;
            ViewData["rozmiar"] = post.items.Count;
            for (int i = 0; i < post.items.Count; i++)
            {
                googlePost.Avatar = post.items[i].actor.image.url;
                googlePost.Date = System.DateTime.Parse(post.items[i].published);
                googlePost.Username = post.items[i].actor.displayName;
                googlePost.DirectLinkToStatus = post.items[i].url;
                if(post.items[i].@object.attachments[0].image!=null)
                {
                    googlePost.ContentImageUrl = post.items[i].@object.attachments[0].image.url;
                }
                tags = Regex.Split(post.items[i].@object.content, @"\s+").Where(b => b.StartsWith("#"));
                for (int x = 0; x < tags.Count(); x++)
                    try
                    {
                        hashtag.HashtagName = tags.ElementAt(x);
                        hashtag.Posts.Add(googlePost);
                        db.Hashtags.Add(hashtag);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        db.Hashtags.Attach(hashtag);
                        googlePost.Hashtags.Add(hashtag);
                    }

                googlePost.ContentDescription = post.items[i].@object.content;
                try
                {

                    db.Posts.Add(googlePost);
                    db.SaveChanges();
                }
                catch
                {
                    if (db.Posts.Local != null)
                    {
                        db.Posts.Local.Clear();
                    }
                }
            }
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

        public ActionResult RssReaderListHashtag(string hashtagname)
        {
            var postRss = db.Posts.Where(po => po.PostSource == "Wyborcza" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "TVN24" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Swiat" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Sport" && po.ContentDescription.Contains(hashtagname)).ToList();
            return View(postRss);
        }


        public void RssReadertoDB()
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
            IEnumerable<Post> RMF24SwiatRssFeeds = Operation.RssOperation.RssReaderRMF24Swiat.GetRssFeed();
            for (int i = 0; i < RMF24SwiatRssFeeds.Count(); i++)
            {
                db.Posts.Add(RMF24SwiatRssFeeds.ElementAt(i));
            }
            IEnumerable<Post> RMF24SportRssFeeds = Operation.RssOperation.RssReaderRMF24Sport.GetRssFeed();
            for (int i = 0; i < RMF24SportRssFeeds.Count(); i++)
            {
                db.Posts.Add(RMF24SportRssFeeds.ElementAt(i));
            }
            db.SaveChanges();
        }

        public ActionResult Create(String imei)   //create imei dla urzadzenia - tez mysle osobny kontroler
        {
            Device device = new Device();
            ViewBag.imei = imei;
            device.DeviceUniqueId = imei;
            db.Devices.Add(device);
            db.SaveChanges();
            return View();
        }

        public ActionResult Delete(int id = 0)    //delete imei dla urzadzenia
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