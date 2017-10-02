using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Operation
{
    public class RssOperation
    {
        public class RssReader
        {
          
            private static string _blogURL = "https://www.tvn24.pl/najnowsze.xml";
            public static IEnumerable<Post> GetRssFeed()
            {
              
               
                XDocument feedXml = XDocument.Load(_blogURL);
                var feeds = from feed in feedXml.Descendants("item")
                            select new Post
                            {
                                Title = feed.Element("title").Value,
                                DirectLinkToStatus = feed.Element("link").Value,
                                ContentDescription = feed.Element("description").Value,
                                Date = System.Convert.ToDateTime(feed.Element("pubDate").Value)

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
                                Title = feed.Element("title").Value,
                                DirectLinkToStatus = feed.Element("link").Value,
                                Date = System.Convert.ToDateTime(feed.Element("pubDate").Value)

                            };

                return feeds;

            }
        }
    }
}