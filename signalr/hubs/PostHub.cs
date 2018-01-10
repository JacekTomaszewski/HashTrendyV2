using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Configuration;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebApiHash.hubs
{
    [HubName("postHub")]
    public class PostHub : Hub
    {
        private static string connString = ConfigurationManager.ConnectionStrings["HashContext"].ConnectionString;
        public void Hello()
        {
            Clients.All.hello();
        }

        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<PostHub>();
            context.Clients.All.updateMessages();
        }
    }
}