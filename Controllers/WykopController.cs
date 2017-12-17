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

        public void GetWykopPosts(string hashTagName)
        {
            string postDescription = "";
            List<string> ListOfHashtags = new List<string>();
            HashContext db = new HashContext();
            HttpWebResponse response;
            HttpWebRequest request = WebRequest.CreateHttp("http://a.wykop.pl/tag/index," + hashTagName + "/appkey," + appkey + "/page=1");
            request.Headers.Add("apisign", CreateSign(hashTagName));
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            string result = "";
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
            WykopDeserializer post = JsonConvert.DeserializeObject<WykopDeserializer>(result);

            for (int i = 0; i < post.items.Count; i++)
            {

                if (post.items[i].description != null)
                    postDescription = post.items[i].description;
                else
                    postDescription = post.items[i].body;

                    ListOfHashtags = Regex.Split(postDescription, @"\W(\#[a-zA-Z]+\b)(?!;)").Where(b => b.StartsWith("#")).ToList();

                PostController.DeserializertoDB("Wykop", post.items[i].author_avatar, System.DateTime.Parse(post.items[i].date),
                    post.items[i].author,postDescription , "ImageURL" ,
                    "Post URL", ListOfHashtags);
            };

        }
    }
}