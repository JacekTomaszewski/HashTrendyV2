using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiHash.Models
{
        public class Counters
        {
            public int total { get; set; }
            public int entries { get; set; }
            public int links { get; set; }
        }

        public class Meta
        {
            public string tag { get; set; }
            public bool is_observed { get; set; }
            public bool is_blocked { get; set; }
            public Counters counters { get; set; }
        }

        public class Embed
        {
            public string type { get; set; }
            public string preview { get; set; }
            public string url { get; set; }
            public bool plus18 { get; set; }
            public string source { get; set; }
        }

        public class ItemW
        {
            public int id { get; set; }
            public string author { get; set; }
            public string author_avatar { get; set; }
            public string author_avatar_big { get; set; }
            public string author_avatar_med { get; set; }
            public string author_avatar_lo { get; set; }
            public int author_group { get; set; }
            public string author_sex { get; set; }
            public string date { get; set; }
            public string body { get; set; }
            public object source { get; set; }
            public string url { get; set; }
            public object receiver { get; set; }
            public object receiver_avatar { get; set; }
            public object receiver_avatar_big { get; set; }
            public object receiver_avatar_med { get; set; }
            public object receiver_avatar_lo { get; set; }
            public object receiver_group { get; set; }
            public object receiver_sex { get; set; }
            public List<object> comments { get; set; }
            public bool blocked { get; set; }
            public int vote_count { get; set; }
            public object user_vote { get; set; }
            public bool user_favorite { get; set; }
            public List<object> voters { get; set; }
            public string type { get; set; }
            public Embed embed { get; set; }
            public bool deleted { get; set; }
            public object violation_url { get; set; }
            public object can_comment { get; set; }
            public string app { get; set; }
            public int comment_count { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string tags { get; set; }
            public string source_url { get; set; }
            public int? report_count { get; set; }
            public int? related_count { get; set; }
            public string group { get; set; }
            public string preview { get; set; }
            public bool? user_observe { get; set; }
            public List<object> user_lists { get; set; }
            public bool? plus18 { get; set; }
            public string status { get; set; }
            public bool? can_vote { get; set; }
            public bool? has_own_content { get; set; }
            public bool? is_hot { get; set; }
            public string category { get; set; }
            public string category_name { get; set; }
            public object info { get; set; }
            public bool? commented { get; set; }
        }

        public class WykopDeserializer
        {
            public Meta meta { get; set; }
            public List<ItemW> items { get; set; }
        }
    }