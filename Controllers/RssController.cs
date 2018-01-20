using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml.Linq;
using WebApiHash.Context;

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
                    DateFormatChangeTVN(feeds[i].Element("pubDate").Value), "TVN24", Regex.Replace(feeds[i].Element("description").Value, @"(<img\/?[^>]+>)", @"", RegexOptions.IgnoreCase),
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
            {
                PostController.DeserializertoDB("Wyborcza", "http://bi.im-g.pl/im/4/16968/m16968574,OG-FACEBOOK-WYBORCZAPL.jpg",
                    DateFormatChange(feeds[i].Element("pubDate").Value), "Wyborcza", feeds[i].Element("title").Value, "",
                    feeds[i].Element("link").Value, null);

            }
        }


        public static void GetRssFeedRMF24Swiat()
        {
            string _blogURL = "http://www.rmf24.pl/fakty/swiat/feed";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = (from feed in feedXml.Descendants("item") select feed).ToList();
            for (int i = 0; i < feeds.Count; i++)
            {
                PostController.DeserializertoDB("RMF24 Swiat", "http://www.gruparmf.pl/_files/Upload/Files/Presspack/RMF-FM-logo.jpg",
                    DateFormatChange(feeds[i].Element("pubDate").Value), "RMF24 Swiat", feeds[i].Element("title").Value, "",
                    feeds[i].Element("link").Value, null);
            }
        }

        public static void GetRssFeedRMF24Sport()
        {
            string _blogURL = "http://www.rmf24.pl/sport/feed";
            XDocument feedXml = XDocument.Load(_blogURL);
            var feeds = (from feed in feedXml.Descendants("item") select feed).ToList();
            for (int i = 0; i < feeds.Count; i++)
            {
                PostController.DeserializertoDB("RMF24 Sport", "http://www.gruparmf.pl/_files/Upload/Files/Presspack/RMF-FM-logo.jpg",
                    DateFormatChange(feeds[i].Element("pubDate").Value), "RMF24 Sport", feeds[i].Element("title").Value, "",
                    feeds[i].Element("link").Value, null);
            }
        }


        public static void GetRss()
        {
            GetRssFeedTVN24();
            GetRssFeedWyborcza();
            GetRssFeedRMF24Swiat();
            GetRssFeedRMF24Sport();
        }

        public static DateTime DateFormatChange(String s)
        {
            string day;
            string month;
            string year;
            string hour;
            string min;
            string sec;
            string dateString;
            string time;

            DateTime date;

            day = s.Substring(5, 2);
            day = abc(day);
            month = s.Substring(8, 3);
            month = abc(month);
            year = s.Substring(12, 4);


            time = s.Substring(17, 8);
            string[] words;

            words = time.Split(':');
            hour = words[0];
            hour = abc(hour);
            min = words[1];
            min = abc(min);
            sec = words[2];
            sec = abc(sec);
            if (sec == "") sec = "00";

            dateString = day + "-" + month + "-" + year + " " + hour + ":" + min + ":" + sec + ".000";

            date = System.Convert.ToDateTime(dateString);

            return (date);

        }

        public static DateTime DateFormatChangeTVN(String s)
        {
            string day;
            string month;
            string year;
            string hour;
            string min;
            string sec;
            string dateString;
            string time;

            DateTime date;

            day = s.Substring(5, 2);
            day = abc(day);
            month = s.Substring(8, 3);
            month = abc(month);
            year = "20" + s.Substring(12, 2);


            time = s.Substring(15, 8);
            string[] words;

            words = time.Split(':');
            hour = words[0];
            hour = abc(hour);
            min = words[1];
            min = abc(min);
            sec = words[2];
            sec = abc(sec);

            if (sec == "") sec = "00";

            dateString = day + "-" + month + "-" + year + " " + hour + ":" + min + ":" + sec + ".000";

            date = System.Convert.ToDateTime(dateString);

            return (date);

        }

        public static string abc(string s)
        {
            string a = "";
            if (s.Length == 1)
            {
                a = "0" + s;

                return a;
            }
            else return s;
        }



    }
}