using LinqToTwitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class GooglePlusController : Controller
    {

        public static void GetGooglePlusPosts(string hashtagname)
        {
            string result;
            string requestString = "https://www.googleapis.com/plus/v1/activities?query=" + hashtagname + "&key=AIzaSyCXR0gFpvOpB0QmZs7qxHB7waGBFywchdA" + "&maxResults=20";
            WebRequest objWebRequest = WebRequest.Create(requestString);
            WebResponse objWebResponse = objWebRequest.GetResponse();
            Stream objWebStream = objWebResponse.GetResponseStream();
            using (StreamReader objStreamReader = new StreamReader(objWebStream))
            {
                result = objStreamReader.ReadToEnd();
            }
            GooglePlusPost jsonResult = JsonConvert.DeserializeObject<GooglePlusPost>(result);

            for (int i = 0; i < jsonResult.items.Count; i++)
            {
                PostController.DeserializertoDB("Google Plus", jsonResult.items[i].actor.image.url, System.DateTime.Parse(jsonResult.items[i].published),
                   jsonResult.items[i].actor.displayName, jsonResult.items[i].@object.content,
                   jsonResult.items[i].@object.attachments[0].image.url != null ? jsonResult.items[i].@object.attachments[0].image.url
                   : null, jsonResult.items[i].@object.url, Regex.Split(jsonResult.items[i].@object.content, @"\W(\#[a-zA-Z]+\b)(?!;)").Where(b => b.StartsWith("#")).ToList());
            }
        }

        }
    }