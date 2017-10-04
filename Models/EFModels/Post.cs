using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApiHash.Models
{
    public class Post
    {
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
    }
}