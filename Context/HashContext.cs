using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApiHash.Models;

namespace WebApiHash.Context
{
    public class HashContext: DbContext
    {
        public DbSet<Trend> Trends { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<Device> Devices { get; set; }
    }
}