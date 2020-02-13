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
    public class TerrainFeaturesController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: TerrainFeatures
        public ActionResult Index()
        {
            return View(db.TerrainFeatures.ToList());
        }

        // GET: TerrainFeatures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TerrainFeature terrainFeature = db.TerrainFeatures.Find(id);
            if (terrainFeature == null)
            {
                return HttpNotFound();
            }
            return View(terrainFeature);
        }

        // GET: TerrainFeatures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TerrainFeatures/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TerrainFeatureId,Description")] TerrainFeature terrainFeature)
        {
            if (ModelState.IsValid)
            {
                db.TerrainFeatures.Add(terrainFeature);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(terrainFeature);
        }

        // GET: TerrainFeatures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TerrainFeature terrainFeature = db.TerrainFeatures.Find(id);
            if (terrainFeature == null)
            {
                return HttpNotFound();
            }
            return View(terrainFeature);
        }

        // POST: TerrainFeatures/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TerrainFeatureId,Description")] TerrainFeature terrainFeature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(terrainFeature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(terrainFeature);
        }

        // GET: TerrainFeatures/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TerrainFeature terrainFeature = db.TerrainFeatures.Find(id);
            if (terrainFeature == null)
            {
                return HttpNotFound();
            }
            return View(terrainFeature);
        }

        // POST: TerrainFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TerrainFeature terrainFeature = db.TerrainFeatures.Find(id);
            db.TerrainFeatures.Remove(terrainFeature);
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
