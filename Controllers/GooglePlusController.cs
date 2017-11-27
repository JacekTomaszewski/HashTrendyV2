﻿using LinqToTwitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TweetSharp;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class GooglePlusController : Controller
    {
        public void GetGooglePlusPosts(string hashtagname)
        {
            HashContext db = new HashContext();
            string result;
            string requestString = "https://www.googleapis.com/plus/v1/activities?query=" + hashtagname + "&key=AIzaSyCXR0gFpvOpB0QmZs7qxHB7waGBFywchdA" + "&maxResults=20";
            WebRequest objWebRequest = WebRequest.Create(requestString);
            WebResponse objWebResponse = objWebRequest.GetResponse();
            Stream objWebStream = objWebResponse.GetResponseStream();
            using (StreamReader objStreamReader = new StreamReader(objWebStream))
            {
                result = objStreamReader.ReadToEnd();
            }
            GooglePlusPost post = JsonConvert.DeserializeObject<GooglePlusPost>(result);
            Post googlePost = new Post();
            googlePost.PostSource = "Google";
            Hashtag hashtag = new Hashtag();
            IEnumerable<string> tags;
            for (int i = 0; i < post.items.Count; i++)
            {
                googlePost.Avatar = post.items[i].actor.image.url;
                googlePost.Date = System.DateTime.Parse(post.items[i].published);
                googlePost.Username = post.items[i].actor.displayName;
                googlePost.DirectLinkToStatus = post.items[i].url;
                googlePost.ContentDescription = post.items[i].@object.content;
                if (post.items[i].@object.attachments != null)
                {
                    try
                    {
                        googlePost.ContentImageUrl = post.items[i].@object.attachments[0].image.url;
                    }
                    catch { }
                }
                // tags = Regex.Split(post.items[i].@object.content, @"\s+").Where(b => b.StartsWith("#"));
                tags = Regex.Split(post.items[i].@object.content, @"\W(\#[a-zA-Z]+\b)(?!;)").Where(b => b.StartsWith("#"));
                for (int x = 0; x < tags.Count(); x++)
                {
                    string hashtagnamefor = tags.ElementAt(x);
                    var query = (from z in db.Hashtags where z.HashtagName == hashtagnamefor select z).SingleOrDefault();
                    if (query == null)
                    {
                        hashtag.HashtagName = tags.ElementAt(x);
                        hashtag.Posts.Add(googlePost);
                        db.Hashtags.Add(hashtag);
                    }
                    else
                    {
                        googlePost.Hashtags.Add(query);
                    }
                }
                    db.Posts.Add(googlePost);
                    db.SaveChanges();
                }
            }
        }
    }