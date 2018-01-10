using Microsoft.Owin;
using Owin;

namespace WebApiHash
{

   public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}