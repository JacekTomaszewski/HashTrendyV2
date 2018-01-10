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
using static WebApiHash.Models.Post;

namespace WebApiHash.Controllers
{
    public class PostController : Controller
    {
        static HashContext db = new HashContext();
        PostRepository messageRepository = new PostRepository();
        public static class DataManager
        {

            public static List<Post> GetPosts(int BlockNumber, int BlockSize)
            {

                int startIndex = (BlockNumber - 1) * BlockSize;
                var posts = (from p in db.Posts
                             orderby p.Date descending
                             select p).Skip(startIndex).Take(BlockSize).ToList();

                return posts;
            }
        }
        public ActionResult Index()
        {
            return View();
        }
        [OutputCache(Duration = 0)]
        public ActionResult GetMessages()
        {
            
            return PartialView("PostList", messageRepository.GetAllMessages());
        }
        [ChildActionOnly]
        public ActionResult PostList(List<Post> Model)
        {
            return PartialView(Model);
        }
        public ActionResult PostsView()
        {
           
            int BlockSize = 10;
            var posts = DataManager.GetPosts(1, BlockSize);
            return View(posts);
        }

        [HttpPost]
        public ActionResult InfinateScroll(int BlockNumber)
        {
           
            int BlockSize = 10;
            var posts = DataManager.GetPosts(BlockNumber, BlockSize);

            JsonModel jsonModel = new JsonModel();
            jsonModel.NoMoreData = posts.Count < BlockSize;
            jsonModel.HTMLString = RenderPartialViewToString("PostList", posts);
            return Json(jsonModel);
        }
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        public ActionResult AsyncUpdateDB()
        {
            GetPostsFromTrendsToDb();
            //Thread thr = new Thread(() => GetPostsFromTrendsToDb());
          //  thr.Start();
            //Thread.Sleep(3600000);
          
            return View(PostsView());
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

        public static void GetPostsFromTrendsToDb()
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
            TwitterController.GetTwitterPosts(hashtagname);
            GooglePlusController.GetGooglePlusPosts(hashtagname);
            WykopController.GetWykopPosts(hashtagname);
            //Thread thr = new Thread(()=>TwitterController.GetTwitterPosts(hashtagname));
            //Thread thr2 = new Thread(() => GooglePlusController.GetGooglePlusPosts(hashtagname));
            // Thread thr3 = new Thread(() => WykopController.GetWykopPosts(hashtagname));
            // thr.Start();
            //System.Diagnostics.Debug.WriteLine("Jestem wątkiem"+thr.Name);
            //  thr2.Start();
            //thr3.Start();
            //System.Diagnostics.Debug.WriteLine("Jestem wątkiem" + thr3.Name);
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
                if (hashtagnamefor.Substring(0, 1) == "#")
                {
                    hashtagnamefor.Remove(0, 1);
                }
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