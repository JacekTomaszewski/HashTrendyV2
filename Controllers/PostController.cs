using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApiHash.Context;

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
            // var Posts = (from p in db.Hashtags where p.HashtagName == hashtagname select p.Posts);
            // var Posts = db.Posts.Where(w => w.Hashtags.Any(s => s.HashtagName == hashtagname)).ToList();
            // var Posts = (from g in db.Posts join m in db.Hashtags on g.PostId equals m.Posts where m.HashtagName == hashtagname select g).ToList();
            //var Posts = from e in db.Posts.Include("hashta") 
            //            where e.EmployeeId == someId
            //            select e;
            return View();
        }

    }
}