using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TweetSharp;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Operation
{
    public class TwitterGetHashTagOperation
    {
        public void GetTwitterPosts(string hashtagname)
        {
            HashContext db = new HashContext();
            Hashtag hashtag = new Hashtag() { Posts = new List<Post>() };
            Post twitterPost = new Post() { Hashtags = new List<Hashtag>() };
            List<TwitterStatus> listTwitterStatus = new List<TwitterStatus>();
            var service = new TwitterService("O5YRKrovfS42vADDPv8NdC4ZS", "tDrCy3YypKhnIOBm0qgCipwGjoJVf7akHV6srkHnLHJm62WvMF");
            service.AuthenticateWith("859793491941093376-kqRIYWY9bWyS10ATfqAVdwk1ZaxloEJ", "hbOXipioFNcyOUyWbGdVAXvoVquETMl57AZUTcbMh3WRv");
            var twitterSearchResult = service.Search(new SearchOptions { Q = hashtagname, Count = 100, Resulttype = TwitterSearchResultType.Recent });
            if (twitterSearchResult != null)
            {
                listTwitterStatus = ((List<TwitterStatus>)twitterSearchResult.Statuses);
            }
            for (int i = 0; i < listTwitterStatus.Count; i++)
            {
                twitterPost.PostSource = "Twitter";
                twitterPost.Avatar = listTwitterStatus[i].User.ProfileImageUrl;
                twitterPost.Date = listTwitterStatus[i].User.CreatedDate;
                twitterPost.Username = listTwitterStatus[i].User.Name;
                twitterPost.ContentDescription = listTwitterStatus[i].Text;
                if (listTwitterStatus[i].Entities.Media.Count > 0)
                {
                    twitterPost.ContentImageUrl = listTwitterStatus[i].Entities.Media[0].MediaUrl;
                }
                for (int x = 0; x < listTwitterStatus[i].Entities.HashTags.Count; x++)
                {
                    try
                    {
                        hashtag.HashtagName = listTwitterStatus[i].Entities.HashTags[x].Text;
                        hashtag.Posts.Add(twitterPost);
                        db.Hashtags.Add(hashtag);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        // var 
                        //string helperString = listTwitterStatus[i].Entities.HashTags[0].Text;
                        //var findHashtag1 = db.Hashtags.Where(f => f.HashtagName == helperString).ToList();
                        //int z = findHashtag1.ElementAt(0).HashtagId;
                        //var findHashtag2 = db.Hashtags.Find(z);
                        //findHashtag2.Posts.Add(twitterPost);
                    }
                }

                try
                {
                    db.Posts.Add(twitterPost);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    //if (db.Posts.Local != null)
                    //{
                    //    db.Posts.Local.Clear();
                    //}
                }
            }
        }

    }
}