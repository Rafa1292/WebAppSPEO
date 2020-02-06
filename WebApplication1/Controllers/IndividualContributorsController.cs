using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
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
            return View(db.IndividualContributors.ToList());
        }

        private void CreateUser(ApplicationDbContext db_user, IndividualContributor individualContributor)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db_user));
            var user = new ApplicationUser
            {
                UserName = individualContributor.Mail,
                Email = individualContributor.Mail
            };

            userManager.Create(user, "abcd1234.");

        }


        // GET: IndividualContributors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IndividualContributors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IndividualContributor individualContributor, string url)
        {
            if (ModelState.IsValid && url != null)
            {
                individualContributor.PictureArray = Convert.FromBase64String(url);
                db.IndividualContributors.Add(individualContributor);
                db.SaveChanges();
                ApplicationDbContext db_user = new ApplicationDbContext();
                CreateUser( db_user, individualContributor);

                return RedirectToAction("Index");
            }
            ViewBag.Url = url;
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
            return View(individualContributor);
        }

        // POST: IndividualContributors/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IndividualContributorId,Name,PhoneNumber,Identification,Mail")] IndividualContributor individualContributor, string url)
        {
            if (ModelState.IsValid && url != null)
            {

                individualContributor.PictureArray = Convert.FromBase64String(url);
                db.Entry(individualContributor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
