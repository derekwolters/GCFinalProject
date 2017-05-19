using System;
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
        
        //Takes userChoice from button input and creates list of nutrient IDs for missing nutrients.
        public ActionResult Selection(int userChoice, string dietChoice)
        {
            var nutrientList = db.CravingNutrients.ToArray();
            var foodSuggestions = new List<string>();
            var nutrientIDList = new List<int>();
            var suggestionIDList = new List<int>();
            var recipeList = new List<Hit>();
            foodSuggestions.Clear();

            //Iterates through database table, matching cravings to nutrient deficiencies.
            nutrientIDList = NutrientIds(nutrientList, nutrientIDList, userChoice);

            //create a viewbag of nutrient names from the nutrient IDs
            ViewBag.Nutrients = NutrientNames(nutrientIDList);

            //create a suggestion list of IDs from the nutirent IDs
            suggestionIDList = GetSuggestionID(nutrientIDList);

            //create a suggestion list of foods from the suggestion IDs
            foodSuggestions = SuggestedFoods(suggestionIDList, foodSuggestions);
            
            //create a viewbag of food suggestions with the dietary restrictions filtered out
            ViewBag.Selection = FilterFoods(foodSuggestions, dietChoice);
            
            //create a suggestion list of recipes
            recipeList = Results(dietChoice, foodSuggestions);

            return View(recipeList);
        }

        //This method iterates through database table, matching cravings to nutrient deficiencies.
        public List<int> NutrientIds(CravingNutrient[] nutrientList, List<int> nutrientIDList, int userChoice)
        {
            //Iterates through database table, matching cravings to nutrient deficiencies.
            for (int i = 0; i < nutrientList.Length; i++)
            {
                if (userChoice == nutrientList[i].CravingID)
                {
                    nutrientIDList.Add(nutrientList[i].NutrientID);
                }
            }

            return nutrientIDList;
        }

        
        //This method converts the list of nutrient IDs to actual nutrient names
        public List<string> NutrientNames(List<int> nutrientIDList)
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

        //This method maps nutrientIDs to suggested food IDs.
        public List<int> GetSuggestionID(List<int> nutrientIDList)
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
        
        //This method returns a list of suggested food names from the food ID list and overwrites foodSuggestions which was instantiated above.
        public List<string> SuggestedFoods(List<int> GetSuggestionID, List<string> foodSuggestions)
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

            foodSuggestions = foodSuggestions.Distinct().ToList();    

            return foodSuggestions;
        }

        //This method returns a list of suggested foods without the ones contained in a dietary restriction
        public List<string> FilterFoods(List<string> foodSuggestions, string dietChoice)
        {
            var nonveganFoods = new List<string>{ "Fish", "Chicken", "Liver", "Beef", "Poultry",  "Eggs", "Dairy", "Lamb", "Goat Milk"};
            var nonvegetarianFoods = new List<string> { "Fish", "Chicken", "Liver", "Beef", "Poultry", "Lamb" };
            var resultList = new List<string> { };
            ViewBag.dietChoice = dietChoice;
            ViewBag.inputlist = foodSuggestions;
            ViewBag.veganlist = nonveganFoods;
            

            for(int i = 0; i<foodSuggestions.Count(); i++)
            {
                for (int j = 0; j < nonveganFoods.Count(); j++)
                {
                    if (foodSuggestions[i] != nonveganFoods[j])
                    {
                        resultList.Add(foodSuggestions[i]);
                    }
                }
            }

            ViewBag.outputlist = resultList;
            return foodSuggestions;           
        }

        //return a list of food suggestions
        public static List<string> getFoodSuggestions(List<string> foodSuggestions)
        {
            return foodSuggestions;
        }

        public List<Hit> Results(string dietChoice, List<string> foodSuggestions)
        {
            string searchTerm;
            string searchRestriction = dietChoice;
            int firstResultIndex = 0;
            int lastRestultIndex = 9;
            Codes secret = new Codes();

            //generate a random index to grab a random search term
            Random rand = new Random();

            int randIndex = rand.Next(0, foodSuggestions.Count);

            searchTerm = getFoodSuggestions(foodSuggestions)[randIndex];

            //build the API call string request
            var url = "https://api.edamam.com";
            var strPostData = "/search?q=" + searchTerm;
            strPostData += "&app_id=" + secret.getID();
            strPostData += "&app_key=" + secret.getKey();
            strPostData += "&from=" + firstResultIndex + "&to=" + lastRestultIndex;

            //check if there is a health restriction
            if (searchRestriction != "")
            {
                strPostData += "&health=" + searchRestriction;
            }

            //combine the base URL and the post data
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

            //put the list of recipes that we can interface with
            var list = oRootObject.hits.ToList();

            return list;
        }
        //***********************END OF UTILIZED METHODS************************
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
