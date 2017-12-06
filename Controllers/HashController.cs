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

    }
}
    //TODO: DUZY PROBLEM Z ZAPISYWANIEM POSTOW DO BAZY DANYCH (!!!)
    //TODO: Logika TwitterTrends - tzn jakie kiedy i dlaczego?
    //TODO: Ujednolicenie bazy danych w tabeli Hashtagi - z '#' czy bez?
    //TODO: Duplikaty w bazie danych w tabeli Posts
    //TODO: Wykop API
    //TODO: Adres localhost w JS przy wyszukiwaniu SpecificPostsView :) - zmienić przy wejściu na serwer
    //TODO: Bootstrap przystosowany do urządzeń mobilnych
    //TODO: Zrobic layout do apki
    //TODO: Twitter uciete posty - inaczej nie mozna - ale moznaby ogarnac inny sposob pobierania z API i wtedy raczej bedzie sie dalo