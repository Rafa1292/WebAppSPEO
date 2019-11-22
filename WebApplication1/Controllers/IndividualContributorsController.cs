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
    public class IndividualContributorsController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: IndividualContributors
        public ActionResult Index()
        {
            var individualContributors = db.IndividualContributors.Include(i => i.IcPicture);
            return View(individualContributors.ToList());
        }

        // GET: IndividualContributors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndividualContributor individualContributor = db.IndividualContributors.Find(id);
            if (individualContributor == null)
            {
                return HttpNotFound();
            }
            return View(individualContributor);
        }

        // GET: IndividualContributors/Create
        public ActionResult Create()
        {
            ViewBag.IndividualContributorId = new SelectList(db.IcPictures, "IcPictureId", "IcPictureId");
            return View();
        }

        // POST: IndividualContributors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IndividualContributorId,Name,PhoneNumber,Identification,Mail,IcPictureId")] IndividualContributor individualContributor)
        {
            if (ModelState.IsValid)
            {
                db.IndividualContributors.Add(individualContributor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IndividualContributorId = new SelectList(db.IcPictures, "IcPictureId", "IcPictureId", individualContributor.IndividualContributorId);
            return View(individualContributor);
        }

        // GET: IndividualContributors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndividualContributor individualContributor = db.IndividualContributors.Find(id);
            if (individualContributor == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndividualContributorId = new SelectList(db.IcPictures, "IcPictureId", "IcPictureId", individualContributor.IndividualContributorId);
            return View(individualContributor);
        }

        // POST: IndividualContributors/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IndividualContributorId,Name,PhoneNumber,Identification,Mail,IcPictureId")] IndividualContributor individualContributor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(individualContributor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IndividualContributorId = new SelectList(db.IcPictures, "IcPictureId", "IcPictureId", individualContributor.IndividualContributorId);
            return View(individualContributor);
        }

        // GET: IndividualContributors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndividualContributor individualContributor = db.IndividualContributors.Find(id);
            if (individualContributor == null)
            {
                return HttpNotFound();
            }
            return View(individualContributor);
        }

        // POST: IndividualContributors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IndividualContributor individualContributor = db.IndividualContributors.Find(id);
            db.IndividualContributors.Remove(individualContributor);
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
