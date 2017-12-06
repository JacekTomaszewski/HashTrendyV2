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

        public static void GooglePlusResultDeserializerToDB(GooglePlusPost post)
        {
            HashContext db = new HashContext();
            IEnumerable<string> tags;
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            Post googlePost = new Post() { Hashtags = new List<Hashtag>() };
            googlePost.PostSource = "Google";
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