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
using WebApiHash.Operation;

namespace WebApiHash.Controllers
{

    public class HashController : Controller
    {
        HashContext db = new HashContext();
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult GetHashtag(string hashtagname)
        {
            GooglePlusGetHashTagOperation googlePlusOperation = new GooglePlusGetHashTagOperation();
            TwitterGetHashTagOperation twitterGetHashTagOperation = new TwitterGetHashTagOperation();
            RssOperation rssOperation = new RssOperation();
            twitterGetHashTagOperation.GetTwitterPosts(hashtagname);
            rssOperation.RssReadertoDB(); // zasilanie bazy danych RSS'ami
            googlePlusOperation.GetGooglePlusPosts(hashtagname.Replace("#", "%23"));    
          

            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult RssReaderListHashtag(string hashtagname)
        {
            var postRss = db.Posts.Where(po => po.PostSource == "Wyborcza" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "TVN24" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Swiat" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Sport" && po.ContentDescription.Contains(hashtagname)).ToList();
            return View(postRss);
        }



    

    

     
    }

}