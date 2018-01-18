using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApiHash.hubs;
using WebApiHash.Models;
using System.Data;

namespace WebApiHash
{
    public class PostRepository
    {
        readonly string _connString = ConfigurationManager.ConnectionStrings["HashContext"].ConnectionString;
        WebApiHash.Context.HashContext db = new Context.HashContext();
        public List<Post> GetAllMessages(string hashtagname)
        {
            string SelectCommand = "SELECT top(10) [Extent1].[PostId] AS[PostId], [Extent1].[Date] AS[Date]," +
                " [Extent1].[Avatar] AS[Avatar], [Extent1].[Username] AS[Username], [Extent1]" +
                ".[ContentDescription] AS[ContentDescription], [Extent1].[ContentImageUrl] AS[ContentImageUrl], " +
                "[Extent1].[DirectLinkToStatus] AS[DirectLinkToStatus], [Extent1].[PostSource] AS[PostSource] " +
                "FROM[dbo].[Posts] AS[Extent1] INNER JOIN(SELECT[Extent2].[Post_PostId] AS[Post_PostId], [Extent3].[HashtagName]" +
                " AS[HashtagName] FROM  [dbo].[PostHashtags] AS[Extent2] INNER JOIN[dbo].[Hashtags]" +
                " AS[Extent3] ON [Extent3].[HashtagId] = [Extent2].[Hashtag_HashtagId]) AS[Join1] ON[Extent1].[PostId]" +
                " = [Join1].[Post_PostId] WHERE [Join1].[HashtagName] LIKE '"+hashtagname+"' order by date desc"; 

            if (hashtagname == "undefinedhashtagname6")
            {
                SelectCommand = @"SELECT top(10) [Extent1].[PostId] AS[PostId], [Extent1].[Date] AS[Date]," +
                " [Extent1].[Avatar] AS[Avatar], [Extent1].[Username] AS[Username], [Extent1]" +
                ".[ContentDescription] AS[ContentDescription], [Extent1].[ContentImageUrl] AS[ContentImageUrl], " +
                "[Extent1].[DirectLinkToStatus] AS[DirectLinkToStatus], [Extent1].[PostSource] AS[PostSource] " +
                "FROM[dbo].[Posts] as [Extent1] order by date desc";
            }
            //else
            //{
            //    SelectCommand = @"SELECT top (1) contentdescription,avatar,date,contentimageurl,
            //    directlinktostatus, postsource,username from posts order by date desc";
            //}
            var messages = new List<Post>();
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(SelectCommand, connection))
                {

                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                    //if (connection.State == ConnectionState.Closed)
                    //    connection.Open();

                    // var reader = command.ExecuteReader();
                    using (var reader = command.ExecuteReader())
                        return messages = reader.Cast<IDataRecord>()
                            .Select(x => new Post()
                            {
                  
                                ContentDescription = x.IsDBNull(4)?"":x.GetString(4),
                                Avatar = x.IsDBNull(2) ? "" : x.GetString(2),
                                Date = x.GetDateTime(1),
                                ContentImageUrl = x.IsDBNull(5) ? "" : x.GetString(5),
                                DirectLinkToStatus = x.IsDBNull(6) ? "" : x.GetString(6),
                                PostSource = x.IsDBNull(7) ? "" : x.GetString(7),
                                Username = x.IsDBNull(3) ? "" : x.GetString(3)


                            }).ToList();
                }

            }
        


        }


        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                PostHub.SendMessages();
            }
        }
    }
}