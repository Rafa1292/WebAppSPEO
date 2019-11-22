using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HouseFeaturesController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: HouseFeatures
        public ActionResult Index()
        {
            var houseFeatures = db.HouseFeatures.Include(h => h.Content);
            return View(houseFeatures.ToList());
        }

        // GET: HouseFeatures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseFeature houseFeature = db.HouseFeatures.Find(id);
            if (houseFeature == null)
            {
                return HttpNotFound();
            }
            return View(houseFeature);
        }

        // GET: HouseFeatures/Create
        public ActionResult Create()
        {
            ViewBag.ContentTypeId = new SelectList(db.ContentTypes, "ContentTypeId", "Description");
            return View();
        }

        // POST: HouseFeatures/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HouseFeatureId,Description,ContentTypeId")] HouseFeature houseFeature)
        {
            if (ModelState.IsValid)
            {
                db.HouseFeatures.Add(houseFeature);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ContentTypeId = new SelectList(db.ContentTypes, "ContentTypeId", "Description", houseFeature.ContentTypeId);
            return View(houseFeature);
        }

        // GET: HouseFeatures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseFeature houseFeature = db.HouseFeatures.Find(id);
            if (houseFeature == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContentTypeId = new SelectList(db.ContentTypes, "ContentTypeId", "Description", houseFeature.ContentTypeId);
            return View(houseFeature);
        }

        // POST: HouseFeatures/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HouseFeatureId,Description,ContentTypeId")] HouseFeature houseFeature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(houseFeature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContentTypeId = new SelectList(db.ContentTypes, "ContentTypeId", "Description", houseFeature.ContentTypeId);
            return View(houseFeature);
        }

        // GET: HouseFeatures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseFeature houseFeature = db.HouseFeatures.Find(id);
            if (houseFeature == null)
            {
                return HttpNotFound();
            }
            return View(houseFeature);
        }

        // POST: HouseFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HouseFeature houseFeature = db.HouseFeatures.Find(id);
            db.HouseFeatures.Remove(houseFeature);
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
