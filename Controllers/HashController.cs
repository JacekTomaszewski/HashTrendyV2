using System.Text;
using System.Web.Mvc;

namespace WebApiHash.Controllers
{

    public class HashController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //public string UTF8Encoding(string stringToEncode)
        //{
        //    byte[] bytes = Encoding.Default.GetBytes(stringToEncode);
        //    stringToEncode = Encoding.UTF8.GetString(bytes);
        //    return stringToEncode;
        //}


        public void GetHashtagPosts(string hashtagname)
        {
            GooglePlusController googlePlusController = new GooglePlusController();
            TwitterController twitterController = new TwitterController();
            twitterController.GetTwitterPosts(hashtagname);
            googlePlusController.GetGooglePlusPosts(hashtagname.Replace("#", "%23"));
        }

        public ActionResult GetHashtagPostsWithView(string hashtagname)
        {
            GooglePlusController googlePlusController = new GooglePlusController();
            TwitterController twitterController = new TwitterController();
            twitterController.GetTwitterPosts(hashtagname);
            googlePlusController.GetGooglePlusPosts(hashtagname.Replace("#", "%23"));
            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }

        public void GetRssPosts()
        {
            RssController rssController = new RssController();
            rssController.RssReadertoDB();
        }
    }
}
    //TODO: DUZY PROBLEM Z ZAPISYWANIEM POSTOW DO BAZY DANYCH (!!!)
    //TODO: Logika TwitterTrends - tzn jakie kiedy i dlaczego?
    //TODO: Ujednolicenie bazy danych w tabeli Hashtagi - z '#' czy bez?
    //TODO: 
    //TODO: Duplikaty w bazie danych
    //TODO: Wykop API
    //TODO: Adres localhost w JS przy wyszukiwaniu SpecificPostsView :) - zmienić przy wejściu na serwer
    //TODO: Bootstrap przystosowany do urządzeń mobilnych
    //TODO: Zrobic layout do apki
    //TODO: Twitter uciete posty - inaczej nie mozna - ale moznaby ogarnac inny sposob pobierania z API i wtedy raczej bedzie sie dalo