using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApiHash.Startup))]
namespace WebApiHash
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
