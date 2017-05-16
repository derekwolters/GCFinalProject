﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GCFinalProject.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web.Script.Serialization;

namespace GCFinalProject.Controllers
{
    public class NutrientsController : Controller
    {
        private HealthyCravingsEntities db = new HealthyCravingsEntities();
        //static int userChoice =2;
        static List<string> foodSuggestions = new List<string>();
        static string foodRestriction = "";

        public ActionResult Selection(int userChoice, string dietChoice)
        {
            ViewBag.Message = "Your craving could be related to a deficiency in the following nutrients: ";
            var nutrientIDList = new List<int>();
            foodRestriction = dietChoice;
            var nutrientList = db.CravingNutrients.ToArray();
            for (int i = 0; i < nutrientList.Length; i++)
            {
                if (userChoice == nutrientList[i].CravingID)
                {
                    nutrientIDList.Add(nutrientList[i].NutrientID);
                }
            }
            ViewBag.Data = NutrientNames(db, nutrientIDList);
            ViewBag.Selection = SuggestedFoods(db, GetSuggestionID(db, nutrientIDList));
            return View(Results());
        }

        public static List<string> NutrientNames(HealthyCravingsEntities db, List<int> nutrientIDList)
        {
            var nutrientNamesList = new List<string>();
            var namesList = db.Nutrients.ToArray();
            foreach (var NutrientID in nutrientIDList)
            {
                for (int i = 0; i < namesList.Length; i++)
                {
                    if (NutrientID == namesList[i].NutrientID)
                    {
                        nutrientNamesList.Add(namesList[i].NutrientName);
                    }
                }
            }
            return nutrientNamesList;
        }

        public static List<int> GetSuggestionID(HealthyCravingsEntities db, List<int> nutrientIDList)
        {
            var suggestedFoodsID = new List<int>();
            var suggestionList = db.NutrientSuggestions.ToArray();
            foreach (var nutrientID in nutrientIDList)
            {
                for (int i = 0; i < suggestionList.Length; i++)
                {
                    if (nutrientID == suggestionList[i].NutrientID)
                    {
                        suggestedFoodsID.Add(suggestionList[i].SuggestionID);
                    }
                }
            }
            return suggestedFoodsID;
        }
        public static List<string> SuggestedFoods(HealthyCravingsEntities db, List<int> GetSuggestionID)
        {
            var suggestionNames = db.Suggestions.ToArray();
            foreach (var suggestionID in GetSuggestionID)
            {
                for (int i = 0; i < suggestionNames.Length; i++)
                {
                    if (suggestionID == suggestionNames[i].SuggestionID)
                    {
                        foodSuggestions.Add(suggestionNames[i].SuggestionName);
                    }
                }
            }
            return foodSuggestions;
        }
        public static List<string> getFoodSuggestions()
        {
            return foodSuggestions;
        }

        //Grab a list of Hits, contains the recipe results
        public List<Hit> Results()
        {
            const string clientID = "4e30bc62";
            const string clientKey = "ec86a3101ba85f41ca22c992867e8d10";            
            int firstResultIndex = 0;
            int lastRestultIndex = 6;

            //generate a random index to grab a random search term
            Random rand = new Random();
            int randIndex = rand.Next(firstResultIndex, lastRestultIndex);
            string searchTerm = getFoodSuggestions()[randIndex];

            //build query string
            var url = "https://api.edamam.com";
            var strPostData = "/search?q=" + searchTerm;
            strPostData += "&app_id=" + clientID;
            strPostData += "&app_key=" + clientKey;
            strPostData += "&from=" + firstResultIndex + "&to=" + lastRestultIndex;
            
            //check if a diet restriction exists, if yes add it
            if (foodRestriction != "")
            {
                strPostData += "&health=" + foodRestriction;
            }

            //build HTTP request
            HttpWebRequest request = WebRequest.CreateHttp(url + strPostData);

            //actually grabs the request response
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

            //convert JSON list of recipes to a usable format
            var list = oRootObject.hits.ToList();

            return list;
        }

        //GET: Nutrients
        public ActionResult Index()
        {
            return View(db.Nutrients.ToList());
        }

        // GET: Nutrients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nutrient nutrient = db.Nutrients.Find(id);
            if (nutrient == null)
            {
                return HttpNotFound();
            }
            return View(nutrient);
        }

        // GET: Nutrients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Nutrients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NutrientID,NutrientName")] Nutrient nutrient)
        {
            if (ModelState.IsValid)
            {
                db.Nutrients.Add(nutrient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nutrient);
        }

        // GET: Nutrients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nutrient nutrient = db.Nutrients.Find(id);
            if (nutrient == null)
            {
                return HttpNotFound();
            }
            return View(nutrient);
        }

        // POST: Nutrients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NutrientID,NutrientName")] Nutrient nutrient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nutrient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nutrient);
        }

        // GET: Nutrients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nutrient nutrient = db.Nutrients.Find(id);
            if (nutrient == null)
            {
                return HttpNotFound();
            }
            return View(nutrient);
        }

        // POST: Nutrients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nutrient nutrient = db.Nutrients.Find(id);
            db.Nutrients.Remove(nutrient);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
