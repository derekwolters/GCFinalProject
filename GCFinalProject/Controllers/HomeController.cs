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
    }
}