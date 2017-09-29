using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiHash.Models
{
    public class Image
    {
        public string url { get; set; }
    }

    public class Image2
    {
        public string url { get; set; }
    }

    public class Actor
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Image image { get; set; }
    }

    public class Replies
    {
        public int totalItems { get; set; }
        public string selfLink { get; set; }
    }

    public class Plusoners
    {
        public int totalItems { get; set; }
        public string selfLink { get; set; }
    }

    public class Resharers
    {
        public int totalItems { get; set; }
        public string selfLink { get; set; }
    }


    public class Attachment
    {
        public string displayName { get; set; }
        public string content { get; set; }
        public string url { get; set; }
        public Image2 image { get; set; }
    }

    public class Object
    {
        public string content { get; set; }
        public string url { get; set; }
        public Replies replies { get; set; }
        public Plusoners plusoners { get; set; }
        public Resharers resharers { get; set; }
        public List<Attachment> attachments { get; set; }
    }

    public class Item
    {

        public string title { get; set; }
        public string published { get; set; }
        public string url { get; set; }
        public Actor actor { get; set; }
        public string verb { get; set; }
        public Object @object { get; set; }
    }

    public class GooglePlusPost
    {
        public List<Item> items { get; set; }
    }
}