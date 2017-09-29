using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApiHash.Controllers
{
    public partial class InstagramRegister : System.Web.UI.Page
    {
       public String ClientId = "cf180dec33df49569e5a04bbbf919179";
       public  String ClientSecret = "8d2d6bb610dd4ce4a106087a7f309b49";
       public String RedirectURI = "http://hashtrend.hostingasp.pl/controllers/instagramcallback.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void ButtonRegisterApp_Click(object sender, EventArgs e)
        {

            Response.Redirect("https://api.instagram.com/oauth/authorize/?client_id=" + ClientId + "&redirect_uri=" + RedirectURI + "&response_type=code");
        }
    }
}