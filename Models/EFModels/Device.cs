using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApiHash.Models
{
    public class Device
    {
        
        public int DeviceId { get; set; }

        [Index("Imei", IsUnique = false)]
        [StringLength(20)]
        public string DeviceUniqueId { get; set; }

        [Index("IX_HashAndDevice", 1, IsUnique =true)]
        [StringLength(450)]
        public virtual ICollection<Hashtag> Hashtags { get; set; }

    }
}