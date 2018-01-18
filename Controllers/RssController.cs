using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class RssController : Controller
    {
        public static HashContext db = new HashContext();

        public static void GetRssFeedTVN24()
        {
            string _blogURL = "https://www.tvn24.pl/najnowsze.xml";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = (from feed in feedXml.Descendants("item") select feed).ToList();
            for (int i = 0; i < feeds.Count; i++)
            {
                PostController.DeserializertoDB("TVN24", "https://lh6.ggpht.com/jyTQdDRrVgrhnyf0pbiTPJZEp2APQoS5z3pc1LveN76ZWBaz2UEdNJRiwOIHhG5cNQ",
                    System.Convert.ToDateTime(feeds[i].Element("pubDate").Value), "TVN24", Regex.Replace(feeds[i].Element("description").Value, @"(<img\/?[^>]+>)", @"", RegexOptions.IgnoreCase),
                    Regex.Match(feeds[i].Element("description").Value, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value,
                    feeds[i].Element("link").Value, null);
            }

        }



        public static void GetRssFeedWyborcza()
        {
            string _blogURL = "http://rss.gazeta.pl/pub/rss/najnowsze_wyborcza.xml";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = (from feed in feedXml.Descendants("item") select feed).ToList();
            for (int i = 0; i < feeds.Count; i++)
                PostController.DeserializertoDB("Wyborcza", "http://bi.im-g.pl/im/4/16968/m16968574,OG-FACEBOOK-WYBORCZAPL.jpg",
                    System.Convert.ToDateTime(feeds[i].Element("pubDate").Value), "Wyborcza", feeds[i].Element("title").Value, "",
                    feeds[i].Element("link").Value, null);
        }



        public static void GetRssFeedRMF24Swiat()
        {
            string _blogURL = "http://www.rmf24.pl/fakty/swiat/feed";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = (from feed in feedXml.Descendants("item") select feed).ToList();
            for (int i = 0; i < feeds.Count; i++)
                PostController.DeserializertoDB("RMF24 Swiat", "http://www.gruparmf.pl/_files/Upload/Files/Presspack/RMF-FM-logo.jpg",
                    System.Convert.ToDateTime(feeds[i].Element("pubDate").Value), "RMF24 Swiat", feeds[i].Element("title").Value,"",
                    feeds[i].Element("link").Value, null);
        }

        public static void GetRssFeedRMF24Sport()
        {
            string _blogURL = "http://www.rmf24.pl/sport/feed";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = (from feed in feedXml.Descendants("item") select feed).ToList();
            for (int i = 0; i < feeds.Count; i++)
                PostController.DeserializertoDB("RMF24 Sport", "http://www.gruparmf.pl/_files/Upload/Files/Presspack/RMF-FM-logo.jpg",
                    System.Convert.ToDateTime(feeds[i].Element("pubDate").Value), "RMF24 Sport", feeds[i].Element("title").Value,"",
                    feeds[i].Element("link").Value, null);
        }


        public ActionResult RssReaderListHashtag(string hashtagname)
        {
            var postRss = db.Posts.Where(po => po.PostSource == "Wyborcza" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "TVN24" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Swiat" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Sport" && po.ContentDescription.Contains(hashtagname)).ToList();
            return View(postRss);
        }

    }
}