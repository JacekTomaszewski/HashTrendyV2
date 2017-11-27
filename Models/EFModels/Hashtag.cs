using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApiHash.Context;

namespace WebApiHash.Models
{
    public class Hashtag
    {
        HashContext abc = new HashContext();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HashtagId { get; set; }
        [Index("HashtagName", IsUnique = true)]
        [MaxLength(200)]
        public string HashtagName { get; set; }

        [Index("IX_HashAndDevice", 2, IsUnique = true)]
        [StringLength(450)]
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

        public List<string> HashtagListForAutoComplete(string name)
        {
            return abc.Hashtags.Where(p => p.HashtagName.StartsWith(name)).Select(x => x.HashtagName).Take(10).ToList();
        }
    }
}