using LinqToTwitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApiHash.Context;
using WebApiHash.Models;
using static WebApiHash.Models.Post;

namespace WebApiHash.Controllers
{
    public class PostController : Controller
    {
        static HashContext db = new HashContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetMessages(string hashtagname)
        {
            var messageRepository = new PostRepository();
            return PartialView("_Partial", messageRepository.GetAllMessages(hashtagname));
        }

        public ActionResult PostsView()
        {
            return View();
        }

        public ActionResult AsyncUpdateDB()
        {
            //to dziala
            RssController.GetRss();
            GetPostsFromTrendsToDb();
            
            //i to tez
            //while (true)
            //{ WykopController.GetWykopPosts("#duda"); }


            return View("PostsView");
        }

        public ActionResult GetSpecificPosts(string hashtagname)
        {
            GetPostsFromSocialMedia(hashtagname);
            return View();
        }

        public ActionResult SpecificPostsView(string hashtagname)
        {
            return View();
        }

        public ActionResult SearchPostByHashtag()
        {
            return View();
        }

        public static void GetPostsFromTrendsToDb()
        {
            var result = (from trend in TwitterController.twitterctx.Trends
                          where trend.Type == TrendType.Place
                                && trend.WoeID == 23424923
                                && trend.SearchUrl.Substring(28, 3).Equals("%23")
                          select trend.Name).ToList();
                for (int i = 0; i < result.Count; i++)
            {
                GetPostsFromSocialMedia(RemoveDiacricts(result.ElementAt(i)));
            }
        }

        public ActionResult Search(string term)
        {
            Hashtag hashtag = new Hashtag();
            return Json(hashtag.HashtagListForAutoComplete(term), JsonRequestBehavior.AllowGet);
        }
        public ActionResult RefreshPage(string name)
        {
            Post post = new Post();
            string json = JsonConvert.SerializeObject(post.RefreshPost(name));
            return Json(json, JsonRequestBehavior.AllowGet);
        }



        public static void GetPostsFromSocialMedia(string hashtagname)
        {
            WykopController.GetWykopPosts(hashtagname);
            GooglePlusController.GetGooglePlusPosts(hashtagname);
            TwitterController.GetTwitterPosts(hashtagname);
        }

        public static string RemoveDiacricts(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private static bool IsDuplicate(string urlAddress)
        {
            var urlAddresses = PostRepository.GetPostUrls();
            return urlAddresses.Contains(urlAddress.ToLower());
        }

        public static void DeserializertoDB(string PostSource, string Avatar, DateTime Date, string Username,
           string ContentDescription, string ContentImageUrl, string UrlAddress, List<string> listOfHashtags)
        {
            if (UrlAddress != null && !IsDuplicate(UrlAddress))
            {
                HashContext db = new HashContext();
                Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
                Post post = new Post() { Hashtags = new List<Hashtag>() };
                post.PostSource = PostSource;
                post.Avatar = Avatar;
                post.Date = Date;
                post.Username = Username;
                post.ContentDescription = ContentDescription;
                post.DirectLinkToStatus = UrlAddress;
                if (ContentImageUrl != "")
                {
                    post.ContentImageUrl = ContentImageUrl;
                }
                if(listOfHashtags!=null)
                { 
                listOfHashtags = listOfHashtags.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
                for (int x = 0; x < listOfHashtags.Count; x++)
                {
                    string hashtagnamefor = listOfHashtags.ElementAt(x);
                    if (hashtagnamefor.Substring(0, 1) == "#")
                    {
                        hashtagnamefor = hashtagnamefor.Remove(0, 1);
                    }
                    hashtagnamefor = RemoveDiacricts(hashtagnamefor);
                    var query = (from z in db.Hashtags where z.HashtagName == hashtagnamefor select z).SingleOrDefault();
                    if (query == null)
                    {
                        hashtag.HashtagName = hashtagnamefor;
                        hashtag.Posts.Add(post);
                        db.Hashtags.Add(hashtag);
                    }
                    else
                    {
                        post.Hashtags.Add(query);
                    }
                }
                }
                db.Posts.Add(post);
                db.SaveChanges();
            }
        }



    }
}
