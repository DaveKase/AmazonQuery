using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Web.Mvc;

namespace AmazonQuery.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JArray GetData(string searchString)
        {
            JArray jArray = MakeAmazonQuery(searchString);
            return jArray;
        }

        private JArray MakeAmazonQuery(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return new JArray();
            }

            JObject keysJObject = GetKeys();
            string accessKey = keysJObject.GetValue("accessKey").ToString();
            string secretKey = keysJObject.GetValue("secretKey").ToString();
            string destination = keysJObject.GetValue("destination").ToString();
            string associateTag = keysJObject.GetValue("associateTag").ToString();
            string currId = keysJObject.GetValue("currId").ToString();

            string requestString = "Service=AWSECommerceService"
                    + "&Version=2009-03-31"
                    + "&Operation=ItemSearch"
                    + "&SearchIndex=All"
                    + "&ResponseGroup=Medium"
                    + "&AssociateTag=" + associateTag
                    + "&Keywords=" + searchString
                    ;

            AmazonApi amazon = new AmazonApi(accessKey, secretKey, destination);
            string requestUrl = amazon.Sign(requestString);
            JArray jArray = amazon.GetData(requestUrl, currId);
            JArray more = amazon.GetData(requestUrl, currId);
            JArray preloaded = amazon.GetData(requestUrl, currId);

            foreach (JObject moreObj in more)
            {
                jArray.Add(moreObj);
            }

            foreach (JObject preObj in preloaded)
            {
                jArray.Add(preObj);
            }

            /* Can't test what happens when the pages run out since we always get the same page and
             * therefor we always have more data to show */

            return jArray;
        }

        private JObject GetKeys()
        {
            string root = Server.MapPath("~");
            string jsonString = System.IO.File.ReadAllText(root + "\\custjson\\amazon.json");
            JObject jObject = JObject.Parse(jsonString);

            return jObject;
        }

        public string ChangeCurrency(string resultString, string oldCurrency, string newCurrency)
        {
            JObject keysJObject = GetKeys();
            string currAppId = keysJObject.GetValue("currId").ToString();
            string url = "https://openexchangerates.org/api/latest.json?app_id=" + currAppId;
            WebClient client = new WebClient();
            string response = client.DownloadString(url);
            JObject jObject = JObject.Parse(response);

            JObject ratesObj = (JObject)jObject.GetValue("rates");
            double rate = 0.0;

            return currAppId;
        }
    }
}