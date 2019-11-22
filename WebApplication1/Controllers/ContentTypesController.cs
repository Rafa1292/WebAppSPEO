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
    public class ContentTypesController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: ContentTypes
        public ActionResult Index()
        {
            return View(db.ContentTypes.ToList());
        }

        // GET: ContentTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentType contentType = db.ContentTypes.Find(id);
            if (contentType == null)
            {
                return HttpNotFound();
            }
            return View(contentType);
        }

        // GET: ContentTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContentTypes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContentTypeId,Description")] ContentType contentType)
        {
            if (ModelState.IsValid)
            {
                db.ContentTypes.Add(contentType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contentType);
        }

        // GET: ContentTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentType contentType = db.ContentTypes.Find(id);
            if (contentType == null)
            {
                return HttpNotFound();
            }
            return View(contentType);
        }

        // POST: ContentTypes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContentTypeId,Description")] ContentType contentType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contentType);
        }

        // GET: ContentTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentType contentType = db.ContentTypes.Find(id);
            if (contentType == null)
            {
                return HttpNotFound();
            }
            return View(contentType);
        }

        // POST: ContentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContentType contentType = db.ContentTypes.Find(id);
            db.ContentTypes.Remove(contentType);
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
