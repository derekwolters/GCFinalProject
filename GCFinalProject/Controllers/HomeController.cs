using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using GCFinalProject.Models;

namespace GCFinalProject.Controllers
{

    public class HomeController : Controller
    {
        const string clientID = "4e30bc62";
        const string clientKey = "ec86a3101ba85f41ca22c992867e8d10";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Craving()
        {
            return View();
        }
        public ActionResult Results()
        {
            string searchTerm = "chicken";
            int firstResultIndex = 0;
            int lastRestultIndex = 10;

            var url = "https://api.edamam.com";
            var strPostData = "/search?q=" + searchTerm;
            strPostData += "&app_id=" + clientID;
            strPostData += "&app_key=" + clientKey;
            strPostData += "&from=" + firstResultIndex + "&to=" + lastRestultIndex;
            Console.WriteLine(url + strPostData);

            HttpWebRequest request = WebRequest.CreateHttp(url + strPostData);

            // actually grabs the request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //gets a stream of text
            StreamReader rd = new StreamReader(response.GetResponseStream());

            //reads to the end of file
            string ApiText = rd.ReadToEnd();

            //Converts that text into JSON
            JObject foodData = JObject.Parse(ApiText);

            //serialize data into usable format
            JavaScriptSerializer oJS = new JavaScriptSerializer();
            RootObject oRootObject = new RootObject();
            oRootObject = oJS.Deserialize<RootObject>(ApiText);

            for (int i = 0; i < oRootObject.hits.Count; i++)
            {
                Console.WriteLine(oRootObject.hits[i]);
            }

            var list = oRootObject.hits.ToList();

            return View(list);
        }
    }
}