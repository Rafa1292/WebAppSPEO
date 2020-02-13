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
    public class UbicationFeaturesController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: UbicationFeatures
        public ActionResult Index()
        {
            return View(db.UbicationFeatures.ToList());
        }

        // GET: UbicationFeatures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UbicationFeature ubicationFeature = db.UbicationFeatures.Find(id);
            if (ubicationFeature == null)
            {
                return HttpNotFound();
            }
            return View(ubicationFeature);
        }

        // GET: UbicationFeatures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UbicationFeatures/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UbicationFeatureId,Description")] UbicationFeature ubicationFeature)
        {
            if (ModelState.IsValid)
            {
                db.UbicationFeatures.Add(ubicationFeature);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ubicationFeature);
        }

        // GET: UbicationFeatures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UbicationFeature ubicationFeature = db.UbicationFeatures.Find(id);
            if (ubicationFeature == null)
            {
                return HttpNotFound();
            }
            return View(ubicationFeature);
        }

        // POST: UbicationFeatures/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UbicationFeatureId,Description")] UbicationFeature ubicationFeature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ubicationFeature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ubicationFeature);
        }

        // GET: UbicationFeatures/Delete/5
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UbicationFeature ubicationFeature = db.UbicationFeatures.Find(id);
            if (ubicationFeature == null)
            {
                return HttpNotFound();
            }
            return View(ubicationFeature);
        }

        // POST: UbicationFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UbicationFeature ubicationFeature = db.UbicationFeatures.Find(id);
            db.UbicationFeatures.Remove(ubicationFeature);
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
