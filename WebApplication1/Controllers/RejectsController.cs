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
    public class RejectsController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: Rejects
        public ActionResult Index()
        {
            IndividualContributor individualContributor = db.IndividualContributors.FirstOrDefault(i => i.Mail == User.Identity.Name);

            var rejects = from r in db.Rejects
                          where r.Article.IndividualContributorId == individualContributor.IndividualContributorId
                          select r;

            return View(rejects.ToList());
        }

        // GET: Rejects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reject reject = db.Rejects.Find(id);
            if (reject == null)
            {
                return HttpNotFound();
            }
            return View(reject);
        }

        // GET: Rejects/Create
        public ActionResult Create()
        {
            ViewBag.ArticleId = new SelectList(db.Articles, "Id", "Description");
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name");
            return View();
        }

        // POST: Rejects/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RejectId,Reason,ArticleId,IndividualContributorId")] Reject reject)
        {
            if (ModelState.IsValid)
            {
                db.Rejects.Add(reject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArticleId = new SelectList(db.Articles, "Id", "Description", reject.ArticleId);
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name", reject.Article.IndividualContributorId);
            return View(reject);
        }

        // GET: Rejects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reject reject = db.Rejects.Find(id);
            if (reject == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArticleId = new SelectList(db.Articles, "Id", "Description", reject.ArticleId);
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name", reject.Article.IndividualContributorId);
            return View(reject);
        }

        // POST: Rejects/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RejectId,Reason,ArticleId,IndividualContributorId")] Reject reject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArticleId = new SelectList(db.Articles, "Id", "Description", reject.ArticleId);
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name", reject.Article.IndividualContributorId);
            return View(reject);
        }

        // GET: Rejects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reject reject = db.Rejects.Find(id);
            if (reject == null)
            {
                return HttpNotFound();
            }
            return View(reject);
        }

        // POST: Rejects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reject reject = db.Rejects.Find(id);
            db.Rejects.Remove(reject);
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
