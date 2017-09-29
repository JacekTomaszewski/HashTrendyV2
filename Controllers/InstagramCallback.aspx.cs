using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApiHash.Controllers
{
    public partial class InstagramCallback : System.Web.UI.Page
    {
        static String code = "";
        string token;
        InstagramRegister fromRegisterClass = new InstagramRegister();
        public void Page_Load(object sender, EventArgs e)
        {
           

            if (!String.IsNullOrEmpty(Request["code"]) && !Page.IsPostBack)
            {
                code = Request["code"].ToString();
                //  GetDataInstagramToken();
                Session["token"] = GetToken();
                
            }
        }

       public string GetToken()
        {
            try
            {
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("client_id", fromRegisterClass.ClientId);
                parameters.Add("client_secret", fromRegisterClass.ClientSecret);
                parameters.Add("grant_type", "authorization_code");
               // parameters.Add("grant_type", "client_credentials"); 
                parameters.Add("redirect_uri", fromRegisterClass.RedirectURI);
                parameters.Add("code", code);
                //https:api.instagram.com/oauth/authorize/?client_id=CLIENT-ID&redirect_uri=REDIRECT-URI&response_type=token
                
                WebClient client = new WebClient();
                var result = client.UploadValues("https://api.instagram.com/oauth/access_token", "POST", parameters);
                var response = System.Text.Encoding.Default.GetString(result);
                var jsResult = (JObject)JsonConvert.DeserializeObject(response);
                dynamic data = JObject.Parse(response);
                string token = data.access_token;
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void btn_Click(object sender, EventArgs e)
        {
            string json;
            string token = Session["token"].ToString();
            WebRequest request = WebRequest.Create("https://api.instagram.com/v1/users/self/media/recent/?access_token=" + token);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            RootObject obj = JsonConvert.DeserializeObject<RootObject>(json);
            for (int i = 0; i < 21; i++)
                Image1.ImageUrl = obj.data[1].images.thumbnail.url;
            Image1.ImageUrl = obj.data[0].images.thumbnail.url;
            Image2.ImageUrl = obj.data[1].images.thumbnail.url;
            Image3.ImageUrl = obj.data[2].images.thumbnail.url;
            Image4.ImageUrl = obj.data[3].images.thumbnail.url;
            Image5.ImageUrl = obj.data[4].images.thumbnail.url;
            Image6.ImageUrl = obj.data[5].images.thumbnail.url;
            Image7.ImageUrl = obj.data[6].images.thumbnail.url;
            Image8.ImageUrl = obj.data[7].images.thumbnail.url;
            Image9.ImageUrl = obj.data[8].images.thumbnail.url;
            Image10.ImageUrl = obj.data[9].images.thumbnail.url;
            Image11.ImageUrl = obj.data[10].images.thumbnail.url;
            Image12.ImageUrl = obj.data[11].images.thumbnail.url;
            Image13.ImageUrl = obj.data[12].images.thumbnail.url;
            Image14.ImageUrl = obj.data[13].images.thumbnail.url;
            Image15.ImageUrl = obj.data[14].images.thumbnail.url;
            Image16.ImageUrl = obj.data[15].images.thumbnail.url;
            Image17.ImageUrl = obj.data[16].images.thumbnail.url;
            Image18.ImageUrl = obj.data[17].images.thumbnail.url;



        }
    } 
    public class Pagination
    {
    }

    public class User
    {
        public string id { get; set; }
        public string full_name { get; set; }
        public string profile_picture { get; set; }
        public string username { get; set; }
    }

    public class Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }

    public class LowResolution
    {
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }

    public class StandardResolution
    {
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }

    public class Images
    {
        public Thumbnail thumbnail { get; set; }
        public LowResolution low_resolution { get; set; }
        public StandardResolution standard_resolution { get; set; }
    }

    public class From
    {
        public string id { get; set; }
        public string full_name { get; set; }
        public string profile_picture { get; set; }
        public string username { get; set; }
    }

    public class Caption
    {
        public string id { get; set; }
        public string text { get; set; }
        public string created_time { get; set; }
        public From from { get; set; }
    }

    public class Likes
    {
        public int count { get; set; }
    }

    public class Comments
    {
        public int count { get; set; }
    }

    public class Location
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class StandardResolution2
    {
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }

    public class LowBandwidth
    {
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }

    public class LowResolution2
    {
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }

    public class Videos
    {
        public StandardResolution2 standard_resolution { get; set; }
        public LowBandwidth low_bandwidth { get; set; }
        public LowResolution2 low_resolution { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public User user { get; set; }
        public Images images { get; set; }
        public string created_time { get; set; }
        public Caption caption { get; set; }
        public bool user_has_liked { get; set; }
        public Likes likes { get; set; }
        public List<object> tags { get; set; }
        public string filter { get; set; }
        public Comments comments { get; set; }
        public string type { get; set; }
        public string link { get; set; }
        public Location location { get; set; }
        public object attribution { get; set; }
        public List<object> users_in_photo { get; set; }
        public Videos videos { get; set; }
    }

    public class Meta
    {
        public int code { get; set; }
    }

    public class RootObject
    {
        public Pagination pagination { get; set; }
        public List<Datum> data { get; set; }
        public Meta meta { get; set; }
    }
}