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
    public class UbicationCategoriesController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: UbicationCategories
        public ActionResult Index()
        {
            return View(db.UbicationCategory.ToList());
        }

        // GET: UbicationCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UbicationCategory ubicationCategory = db.UbicationCategory.Find(id);
            if (ubicationCategory == null)
            {
                return HttpNotFound();
            }
            return View(ubicationCategory);
        }

        // GET: UbicationCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UbicationCategories/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UbicationCategoryId,Name")] UbicationCategory ubicationCategory)
        {
            if (ModelState.IsValid)
            {
                db.UbicationCategory.Add(ubicationCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ubicationCategory);
        }

        // GET: UbicationCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UbicationCategory ubicationCategory = db.UbicationCategory.Find(id);
            if (ubicationCategory == null)
            {
                return HttpNotFound();
            }
            return View(ubicationCategory);
        }

        // POST: UbicationCategories/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UbicationCategoryId,Name")] UbicationCategory ubicationCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ubicationCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ubicationCategory);
        }

        // GET: UbicationCategories/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UbicationCategory ubicationCategory = db.UbicationCategory.Find(id);
            if (ubicationCategory == null)
            {
                return HttpNotFound();
            }
            return View(ubicationCategory);
        }

        // POST: UbicationCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UbicationCategory ubicationCategory = db.UbicationCategory.Find(id);
            db.UbicationCategory.Remove(ubicationCategory);
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
