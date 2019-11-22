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
    public class DistritsController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: Distrits
        public ActionResult Index()
        {
            var distrits = db.Distrits.Include(d => d.Canton);
            return View(distrits.ToList());
        }

        // GET: Distrits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distrit distrit = db.Distrits.Find(id);
            if (distrit == null)
            {
                return HttpNotFound();
            }
            return View(distrit);
        }

        // GET: Distrits/Create
        public ActionResult Create()
        {
            ViewBag.CantonId = new SelectList(db.Cantons, "CantonId", "Name");
            return View();
        }

        // POST: Distrits/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DistritId,Name,CantonId")] Distrit distrit)
        {
            if (ModelState.IsValid)
            {
                db.Distrits.Add(distrit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CantonId = new SelectList(db.Cantons, "CantonId", "Name", distrit.CantonId);
            return View(distrit);
        }

        // GET: Distrits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distrit distrit = db.Distrits.Find(id);
            if (distrit == null)
            {
                return HttpNotFound();
            }
            ViewBag.CantonId = new SelectList(db.Cantons, "CantonId", "Name", distrit.CantonId);
            return View(distrit);
        }

        // POST: Distrits/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DistritId,Name,CantonId")] Distrit distrit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(distrit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CantonId = new SelectList(db.Cantons, "CantonId", "Name", distrit.CantonId);
            return View(distrit);
        }

        // GET: Distrits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distrit distrit = db.Distrits.Find(id);
            if (distrit == null)
            {
                return HttpNotFound();
            }
            return View(distrit);
        }

        // POST: Distrits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Distrit distrit = db.Distrits.Find(id);
            db.Distrits.Remove(distrit);
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
