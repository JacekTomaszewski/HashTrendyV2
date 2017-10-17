using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Operation
{
    public class RssOperation
    {
        HashContext db = new HashContext();
        public class RssReaderTVN24
        {
            
            private static string _blogURL = "https://www.tvn24.pl/najnowsze.xml";
            public static IEnumerable<Post> GetRssFeed()
            {
                XDocument feedXml = XDocument.Load(_blogURL);
                var feeds = from feed in feedXml.Descendants("item")
                            select new Post
                            {
                                Username = "TVN24",
                                DirectLinkToStatus = feed.Element("link").Value,
                                ContentDescription = Regex.Replace(feed.Element("description").Value, @"(<img\/?[^>]+>)", @"",
                                RegexOptions.IgnoreCase),
                                PostSource="TVN24",
                                Date = System.Convert.ToDateTime(feed.Element("pubDate").Value),
                                ContentImageUrl = Regex.Match(feed.Element("description").Value, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value
            };

                return feeds;
               
            }

        }

        public class RssReaderWyborcza
        {
            private static string _blogURL = "http://rss.gazeta.pl/pub/rss/najnowsze_wyborcza.xml";
            public static IEnumerable<Post> GetRssFeed()
            {
                XDocument feedXml = XDocument.Load(_blogURL);
                var feeds = from feed in feedXml.Descendants("item")
                            select new Post
                            {
                                Username="Wyborcza",
                                ContentDescription = feed.Element("title").Value,
                                DirectLinkToStatus = feed.Element("link").Value,
                                PostSource = "Wyborcza",
                                Avatar= "~/App_Data/RMF.jpg",
                                Date = System.Convert.ToDateTime(feed.Element("pubDate").Value)
            };
                
                return feeds;

            }
        }

        public class RssReaderRMF24Swiat
        {
            private static string _blogURL = "http://www.rmf24.pl/fakty/swiat/feed";
            public static IEnumerable<Post> GetRssFeed()
            {
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
        }

        public class RssReaderRMF24Sport
        {
            private static string _blogURL = "http://www.rmf24.pl/sport/feed";
            public static IEnumerable<Post> GetRssFeed()
            {
                XDocument feedXml = XDocument.Load(_blogURL);
                var feeds = from feed in feedXml.Descendants("item")
                            select new Post
                            {
                                Avatar= "D:/ProgramowanieGIT/HashTrendyV2/App_Data/RMF.jpg",
                                Username = "RMF24 Sport",
                                ContentDescription = feed.Element("title").Value,
                                DirectLinkToStatus = feed.Element("link").Value,
                                PostSource = "RMF24 Sport",
                                Date = System.Convert.ToDateTime(feed.Element("pubDate").Value)
                            };

                return feeds;

            }
        }
    }
}