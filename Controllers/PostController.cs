using LinqToTwitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class PostController : Controller
    {
        static HashContext db = new HashContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostsView()
        {
            var Posts = db.Posts.ToList();
            return View(Posts);
        }

       public ActionResult AsyncUpdateDB()
        {

            while (true)
            {
                TwitterController.TwitterTrendstoDB();
                GetPostsFromTrendsToDb();
                Thread.Sleep(3600000);
            }

            return RedirectToAction("SpecificPostsView", "Post");
        }

        public static string RemoveDiacricts(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
        public ActionResult SpecificPostsView(string hashtagname)
        {
            if(hashtagname!=null)
            GetPostsFromSocialMedia(hashtagname);
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

        public static void GetPostsFromTrendsToDb()
        {
            HashController hash = new HashController();
            DateTime dt1 = DateTime.Now.AddHours(-1.1);
            var result = (from m in db.Trends
                          where m.DateCreated > dt1
                          select m).ToList();
            
            
            for (int i = 0; i < result.Count-1; i++)
            {
                Thread thr = new Thread(() => GetPostsFromSocialMedia(result.ElementAt(i).TrendName));
                Thread.Sleep(120);
                thr.Start();
            }
        }

        public ActionResult Search(string term)
        {
            Hashtag hashtag = new Hashtag();
            return Json(hashtag.HashtagListForAutoComplete(term), JsonRequestBehavior.AllowGet);
        }


        public static void GetPostsFromSocialMedia(string hashtagname)
        {

            hashtagname = RemoveDiacricts(hashtagname);
            Thread thr = new Thread(() => TwitterController.GetTwitterPosts(hashtagname));
            Thread thr2 = new Thread(() => GooglePlusController.GetGooglePlusPosts(hashtagname));
            Thread thr3 = new Thread(() => WykopController.GetWykopPosts(hashtagname));
            thr.Start();
            Thread.Sleep(100);
            thr2.Start();
            Thread.Sleep(100);
            thr3.Start();
            Thread.Sleep(100);
        }

        public static void DeserializertoDB(string PostSource, string Avatar, DateTime Date, string Username,
           string ContentDescription, string ContentImageUrl, string UrlAddress, List<string> listOfHashtags)
        {
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            Post post = new Post() { Hashtags = new List<Hashtag>() };
            try
            {
                var postquery = (from z in db.Posts where z.ContentDescription == ContentDescription select z).First(); //exception wyskakuje jak nic nie ma
            }
            catch
            { 
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
                        string hashtagnamefor = RemoveDiacricts(listOfHashtags.ElementAt(x));
                        if (hashtagnamefor.Substring(0, 1) == "#")
                        {
                            hashtagnamefor = hashtagnamefor.Remove(0, 1);
                    }

                    var query = (from z in db.Hashtags where z.HashtagName == hashtagnamefor select z).FirstOrDefault();
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



    }