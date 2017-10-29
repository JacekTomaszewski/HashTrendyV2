using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class WykopController : Controller
    {
        private string appkey = "0In5DAkznE";
        private string secret = "gtiMvSQTP2";


        // GET: Wykop
        public ActionResult Index()
        {
            return View();
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        private string CreateSign(string hashTagName)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                //string textToHash = secret + "http://a.wykop.pl/tag/index," + hashTagName + "/appkey," + appkey + "/format,xml" + "/page=1"; //xml
                string textToHash = secret + "http://a.wykop.pl/tag/index," + hashTagName + "/appkey," + appkey + "/page=1"; //json
                string hash = GetMd5Hash(md5Hash, textToHash);
                return hash;
            }
        }

        public ActionResult DoRequest(string hashTagName)
        {
            HashContext db = new HashContext();
            HttpWebRequest request = WebRequest.CreateHttp("http://a.wykop.pl/tag/index," + hashTagName + "/appkey," + appkey + "/page=1"); //json
            //HttpWebRequest request = WebRequest.CreateHttp("http://a.wykop.pl/tag/index," + hashTagName + "/appkey," + appkey + "/format,xml" + "/page=1"); //xml
            request.Headers.Add("apisign", CreateSign(hashTagName));
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/xml";
            string result = "";
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                result = reader.ReadToEnd();                
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                if (response != null) //timeout
                {
                    result = response.StatusCode.ToString(); //result = json string
                }
            }

            WykopPost post = JsonConvert.DeserializeObject<WykopPost>(result);
            Post wykopPost = new Post();
            wykopPost.PostSource = "Wykop";
            Hashtag hashtag = new Hashtag();
            IEnumerable<string> tags;

            for (int i = 0; i < post.items.Count; i++)
            {
                //wykopPost.Avatar = post.items[i].actor.image.url;
                wykopPost.Date = System.DateTime.Parse(post.items[i].published);
                wykopPost.Username = post.items[i].actor.displayName;
                wykopPost.DirectLinkToStatus = post.items[i].url;

                if (post.items[i].@object.attachments != null)
                {
                    try
                    {
                        wykopPost.ContentImageUrl = post.items[i].@object.attachments[0].image.url;
                    }
                    catch { }
                }

                tags = Regex.Split(post.items[i].@object.content, @"\s+").Where(b => b.StartsWith("#"));
                for (int x = 0; x < tags.Count(); x++)
                    try
                    {
                        hashtag.HashtagName = tags.ElementAt(x);
                        hashtag.Posts.Add(wykopPost);
                        db.Hashtags.Add(hashtag);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        db.Hashtags.Attach(hashtag);
                        wykopPost.Hashtags.Add(hashtag);
                    }

                wykopPost.ContentDescription = post.items[i].@object.content;
                try
                {
                    db.Posts.Add(wykopPost);
                    db.SaveChanges();
                }
                catch
                {
                    if (db.Posts.Local != null)
                    {
                        db.Posts.Local.Clear();
                    }
                }
            }

            ViewData["test"] = result;
            return View(ViewData);
        }
    }
}