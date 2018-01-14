using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApiHash.Context;

namespace WebApiHash.Models
{
    public class Post
    {
        HashContext abc = new HashContext();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }

        public DateTime Date { get; set; }

        public string Avatar { get; set; }

        public string Username { get; set; }

        public string ContentDescription { get; set; }

        public string ContentImageUrl { get; set; }

        public string DirectLinkToStatus { get; set; }

        public string PostSource { get; set; }

        public virtual ICollection<Hashtag> Hashtags { get; set; }

        public List<Post> RefreshPost(string name)
        {
           var result = (from m in abc.Posts
             from b in m.Hashtags
             where b.HashtagName.Contains(name)
             select m).ToList();
            var postRss = abc.Posts.Where(po => po.PostSource == "Wyborcza" && po.ContentDescription.Contains(name) ||
            po.PostSource == "TVN24" && po.ContentDescription.Contains(name) ||
            po.PostSource == "RMF24 Swiat" && po.ContentDescription.Contains(name) ||
            po.PostSource == "RMF24 Sport" && po.ContentDescription.Contains(name)).ToList();
            result.AddRange(postRss);
            return result;
        }
    }
}