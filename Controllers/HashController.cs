using System.Web.Mvc;

namespace WebApiHash.Controllers
{

    public class HashController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public void GetHashtagPosts(string hashtagname)
        {
            GooglePlusController googlePlusController = new GooglePlusController();
            TwitterController twitterController = new TwitterController();
            twitterController.GetTwitterPosts(hashtagname);
            // googlePlusController.GetGooglePlusPosts(hashtagname.Replace("#", "%23"));
            // return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }

        public void GetRssPosts()
        {
            RssController rssController = new RssController();
            rssController.RssReadertoDB();
        }
    }

}