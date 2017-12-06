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
            return sBuilder.ToString();
        }

        private string CreateSign(string hashTagName)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string textToHash = secret + "http://a.wykop.pl/tag/index," + hashTagName + "/appkey," + appkey + "/page=1"; //json
                string hash = GetMd5Hash(md5Hash, textToHash);
                return hash;
            }
        }

        public ActionResult RequestView(string hashTagName)
        {
            HashContext db = new HashContext();
            HttpWebRequest request = WebRequest.CreateHttp("http://a.wykop.pl/tag/index," + hashTagName + "/appkey," + appkey + "/page=1");
            request.Headers.Add("apisign", CreateSign(hashTagName));
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
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
                if (response != null)
                {
                    result = response.StatusCode.ToString();
                }
            }
            List<Post> abc = new List<Post>();
            WykopDeserializer post = JsonConvert.DeserializeObject<WykopDeserializer>(result);
            Post wykopPost;
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            if (post.items.Count != 0)
                for (int i = 0; i < post.items.Count; i++)
                {
                    wykopPost = new Post() { Hashtags = new List<Hashtag>() };
                    wykopPost.PostSource = "Wykop";
                    wykopPost.Avatar = post.items[i].author_avatar;
                    wykopPost.Date = System.DateTime.Parse(post.items[i].date);
                    wykopPost.Username = post.items[i].author;

                    if (post.items[i].description != null)
                    {
                        wykopPost.ContentDescription = post.items[i].description;
                        var tags = Regex.Split(post.items[i].description, @"\W(\#[a-zA-Z]+\b)(?!;)").Where(b => b.StartsWith("#"));
                        for (int x = 0; x < tags.Count() - 1; x++)
                        {
                            string hashtagnamefor = tags.ElementAt(x);
                            var query = (from z in db.Hashtags where z.HashtagName == hashtagnamefor select z).SingleOrDefault();
                            if (query == null)
                            {
                                hashtag.HashtagName = tags.ElementAt(x);
                                hashtag.Posts.Add(wykopPost);
                                db.Hashtags.Add(hashtag);
                            }
                            else
                            {
                                wykopPost.Hashtags.Add(query);
                            }
                        }
                    }
                    else
                    {
                        wykopPost.ContentDescription = post.items[i].body;
                        var tags = Regex.Split(post.items[i].body, @"\W(\#[a-zA-Z]+\b)(?!;)").Where(b => b.StartsWith("#"));
                        for (int x = 0; x < tags.Count() - 1; x++)
                        {
                            string hashtagnamefor = tags.ElementAt(x);
                            var query = (from z in db.Hashtags where z.HashtagName == hashtagnamefor select z).SingleOrDefault();
                            if (query == null)
                            {
                                hashtag.HashtagName = tags.ElementAt(x);
                                hashtag.Posts.Add(wykopPost);
                                db.Hashtags.Add(hashtag);
                            }
                            else
                            {
                                wykopPost.Hashtags.Add(query);
                            }
                        }
                    }
                    db.Posts.Add(wykopPost);
                    db.SaveChanges();
                    abc.Add(wykopPost);
                }
            return View(abc.ToList());
        }
    }
}