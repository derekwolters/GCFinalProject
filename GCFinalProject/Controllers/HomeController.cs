﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
<<<<<<< HEAD
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Xml;
=======
using System.Web.Script.Serialization;
using GCFinalProject.Models;
>>>>>>> 8e43f1b1a072e8a0261dd64e67aefa20a178573b

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
        public ActionResult Success()
        {
            ViewBag.Message = "Sorry to hear that you're craving sweets. This could stem from a deficiency in: ";
            return View();
        }

        public ActionResult Results()
        {
<<<<<<< HEAD
            //var URL = "https://api.edamam.com";
            //var strPostData = "https://api.edamam.com/search?q=" + searchTerm;
            //strPostData += "&app_id=" + clientID;
            //strPostData += "&app_key=" + clientKey;
            ////NOvar strPostData = "https://api.edamam.com/search?q=chicken&app_id=4e30bc62&app_key=ec86a3101ba85f41ca22c992867e8d10&from=0&to=3&calories=gte%20591,%20lte%20722&health=alcohol-free";
            //WebClient WC = new WebClient();

            //ViewBag.testUrl = strPostData;
            //var strResponse = wc.UploadString(URL, strPostData);
            ////NOvar login = JavascriptDeserialize<OAuthLogin>(strResponse);
            ////NOvar strSearch = "https://api.edamam.com/search";
            ////NOwc.Headers.Add("Authorization", "Bearer " + login.access_token);
            ////NOvar strSearchResponse = wc.DownloadString(strSearch);
            //ViewBag.Result = "response:" + strResponse;

            //HttpWebRequest request =
            //   WebRequest.CreateHttp("https://api.edamam.com/search?q=chicken&app_id=4e30bc62&app_key=ec86a3101ba85f41ca22c992867e8d10&from=0&to=3&calories=gte%20591,%20lte%20722&health=alcohol-free");
            //request.UserAgent = "Foo";
            //request.Accept = "*/*";

            //// actually grabs the request
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            ////gets a stream of text
            //StreamReader rd = new StreamReader(response.GetResponseStream());

            ////reads to the end of file
            //string ApiText = rd.ReadToEnd();

            ////Converts that text into JSON
            //JObject foodData = JObject.Parse(ApiText);

            //ViewBag.ApiText = foodData;

=======
>>>>>>> 8e43f1b1a072e8a0261dd64e67aefa20a178573b
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