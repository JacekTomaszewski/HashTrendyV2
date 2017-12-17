using LinqToTwitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
            ViewBag.data = postSources.ToArray();
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
            GooglePlusController.GetGooglePlusPosts(hashtagname);
            TwitterController.GetTwitterPosts(hashtagname); 
            
            return Redirect("http://localhost:50707/post/SpecificPostsView?hashtagname=" + hashtagname);
        }

        public static void DeserializertoDB(string PostSource, string Avatar, DateTime Date, string Username,
           string ContentDescription, string ContentImageUrl, string UrlAddress, List<string> listOfHashtags)
        {
            HashContext db = new HashContext();
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            Post post = new Post() { Hashtags = new List<Hashtag>() };

            post.PostSource = PostSource;
            post.Avatar = Avatar;
            post.Date = Date;
            post.Username = Username;
            post.ContentDescription = ContentDescription;
            if (ContentImageUrl != "")
            {
                post.ContentImageUrl = ContentImageUrl;
            }

            for (int x = 0; x < listOfHashtags.Count; x++)
            {
                string hashtagnamefor = listOfHashtags.ElementAt(x);
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
            db.Posts.Add(post);
            db.SaveChanges();
        }


    }
}