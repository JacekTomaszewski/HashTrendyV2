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
     
        public List<Post> GetAllMessages()
        {
            
           
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT contentdescription,avatar,date,contentimageurl,
                directlinktostatus, postsource,username from posts order by date desc", connection))
                {

                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                    //if (connection.State == ConnectionState.Closed)
                    //    connection.Open();
                   
                    // var reader = command.ExecuteReader();
                    using (var reader = command.ExecuteReader())
                        return reader.Cast<IDataRecord>()
                            .Select(x => new Post()
                            {

                                ContentDescription = x.IsDBNull(0) ? "" : x.GetString(0),
                                Avatar = x.IsDBNull(1) ? "" : x.GetString(1),
                                Date = x.GetDateTime(2),
                                ContentImageUrl = x.IsDBNull(3) ? "" : x.GetString(3),
                                DirectLinkToStatus = x.IsDBNull(4) ? "" : x.GetString(4),
                                PostSource = x.IsDBNull(5) ? "" : x.GetString(5),
                                Username = x.IsDBNull(6) ? "" : x.GetString(6)


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