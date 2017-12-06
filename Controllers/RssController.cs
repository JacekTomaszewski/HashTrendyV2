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

        public static IEnumerable<Post> GetRssFeedTVN24()
        {
            string _blogURL = "https://www.tvn24.pl/najnowsze.xml";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = from feed in feedXml.Descendants("item")
                        select new Post
                        {
                            Username = "TVN24",
                            DirectLinkToStatus = feed.Element("link").Value,
                            ContentDescription = Regex.Replace(feed.Element("description").Value, @"(<img\/?[^>]+>)", @"",
                            RegexOptions.IgnoreCase),
                            PostSource = "TVN24",
                            Date = System.Convert.ToDateTime(feed.Element("pubDate").Value),
                            ContentImageUrl = Regex.Match(feed.Element("description").Value, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value
                        };
            return feeds;
        }



        public static IEnumerable<Post> GetRssFeedWyborcza()
        {
            string _blogURL = "http://rss.gazeta.pl/pub/rss/najnowsze_wyborcza.xml";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = from feed in feedXml.Descendants("item")
                        select new Post
                        {
                            Username = "Wyborcza",
                            ContentDescription = feed.Element("title").Value,
                            DirectLinkToStatus = feed.Element("link").Value,
                            PostSource = "Wyborcza",
                            Avatar = "~/App_Data/RMF.jpg",
                            Date = System.Convert.ToDateTime(feed.Element("pubDate").Value)
                        };
            return feeds;
        }



        public static IEnumerable<Post> GetRssFeedRMF24Swiat()
        {
            string _blogURL = "http://www.rmf24.pl/fakty/swiat/feed";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = from feed in feedXml.Descendants("item")
                        select new Post
                        {
                            Username = "RMF24 Swiat",
                            ContentDescription = feed.Element("title").Value,
                            DirectLinkToStatus = feed.Element("link").Value,
                            PostSource = "RMF24 Swiat",
                            Date = System.Convert.ToDateTime(feed.Element("pubDate").Value)
                        };

            return feeds;
        }




        public static IEnumerable<Post> GetRssFeedRMF24Sport()
        {
            string _blogURL = "http://www.rmf24.pl/sport/feed";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = from feed in feedXml.Descendants("item")
                        select new Post
                        {
                            Avatar = "D:/ProgramowanieGIT/HashTrendyV2/App_Data/RMF.jpg",
                            Username = "RMF24 Sport",
                            ContentDescription = feed.Element("title").Value,
                            DirectLinkToStatus = feed.Element("link").Value,
                            PostSource = "RMF24 Sport",
                            Date = System.Convert.ToDateTime(feed.Element("pubDate").Value)
                        };
            return feeds;

        }


        public static void RssReadertoDB()
        {
            IEnumerable<Post> WyborczaRssFeeds = GetRssFeedWyborcza();
            for (int i = 0; i < WyborczaRssFeeds.Count(); i++)
            {
                db.Posts.Add(WyborczaRssFeeds.ElementAt(i));
            }
            IEnumerable<Post> TVN24RssFeeds = GetRssFeedTVN24();
            for (int i = 0; i < TVN24RssFeeds.Count(); i++)
            {
                db.Posts.Add(TVN24RssFeeds.ElementAt(i));
            }
            IEnumerable<Post> RMF24SwiatRssFeeds = GetRssFeedRMF24Swiat();
            for (int i = 0; i < RMF24SwiatRssFeeds.Count(); i++)
            {
                db.Posts.Add(RMF24SwiatRssFeeds.ElementAt(i));
            }
            IEnumerable<Post> RMF24SportRssFeeds = GetRssFeedRMF24Sport();
            for (int i = 0; i < RMF24SportRssFeeds.Count(); i++)
            {
                db.Posts.Add(RMF24SportRssFeeds.ElementAt(i));
            }
            db.SaveChanges();
        }


        public ActionResult RssReaderListHashtag(string hashtagname)
        {
            var postRss = db.Posts.Where(po => po.PostSource == "Wyborcza" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "TVN24" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Swiat" && po.ContentDescription.Contains(hashtagname) || po.PostSource == "RMF24 Sport" && po.ContentDescription.Contains(hashtagname)).ToList();
            return View(postRss);
        }
    }
}