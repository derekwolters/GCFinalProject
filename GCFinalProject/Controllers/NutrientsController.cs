using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GCFinalProject.Models;

namespace GCFinalProject.Controllers
{
    public class NutrientsController : Controller
    {
        private HealthyCravingsEntities db = new HealthyCravingsEntities();
        int userChoice = 2;

        public ActionResult Selection(HealthyCravingsEntities db)
        {
            ViewBag.Message = "Your craving could be related to a deficiency in the following nutrients: ";
            var nutrientIDList = new List<int>();
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
            return View();
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
            var foodSuggestions = new List<string>();
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
