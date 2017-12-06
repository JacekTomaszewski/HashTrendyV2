using LinqToTwitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class PostController : Controller
    {
        HashContext db = new HashContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostsView()
        {
            var Posts = db.Posts.ToList();
            return View(Posts);
        }

        public ActionResult SpecificPostsView(string hashtagname)
        {
            var result = (from m in db.Posts
                          from b in m.Hashtags
                          where b.HashtagName.Contains(hashtagname)
                          select m).ToList();
            var postRss = db.Posts.Where(po => po.PostSource == "Wyborcza" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "TVN24" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Swiat" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Sport" && po.ContentDescription.Contains(hashtagname)).ToList();
            result.AddRange(postRss);

            var postSources = db.Posts.Where(p => p.PostSource != null).Select(p => p.PostSource).Distinct().ToList();
            ViewBag.data= postSources.ToArray();
            return View(result);
        }

        public ActionResult GetPostsFromTrendsToDb()
        {
            HashController hash = new HashController();
            DateTime dt1 = DateTime.Now.AddHours(-1.1);
            var result = (from m in db.Trends
                          where m.DateCreated > dt1
                          select m).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                GetPostsFromSocialMedia(result.ElementAt(i).TrendName);
            }
            return View(result);
        }

        public ActionResult Search(string term)
        {
            Hashtag hashtag = new Hashtag();
            return Json(hashtag.HashtagListForAutoComplete(term), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPostsFromSocialMedia(string hashtagname)
        {
            #region Twitter
            TwitterContext twitterCtx = new TwitterContext(TwitterController.auth);
            var searchResponse =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == hashtagname &&
                       search.ResultType == ResultType.Recent &&
                       search.TweetMode == TweetMode.Extended &&
                       search.Count == 100
                 select search).ToList();
            List<Post> ListOfPosts = new List<Post>();
            for(int i = 0; i < searchResponse.Count; i++)
            { 
            Post TempPost = new Post()
            {
                Avatar = searchResponse[0].Statuses[i].User.ProfileImageUrl,
                PostSource = "Twitter",
                Date = searchResponse[0].Statuses[i].CreatedAt,
                Username = searchResponse[0].Statuses[i].User.Name,
                ContentDescription = searchResponse[0].Statuses[i].FullText
            };
                ListOfPosts.Add(post);
            }

            TwitterController.TwitterDeserializertoDB(searchResponse);

            #endregion
            #region GooglePlus
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

            GooglePlusController.GooglePlusResultDeserializerToDB(post);
            #endregion
            #region Rss
            RssController.RssReadertoDB();
            #endregion

            return Redirect("http://localhost:50707/post/SpecificPostsView?hashtagname=" + hashtagname);
        }

    }
}