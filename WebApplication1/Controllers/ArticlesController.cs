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
    public class ArticlesController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: Articles
        public ActionResult Index()
        {
            var articles = db.Articles.Include(a => a.IndividualContributor).Include(a => a.Terrain).Include(a => a.Ubication);
            return View(articles.ToList());
        }


        // GET: Articles/Create
        public ActionResult Create()
        {
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name");
            ViewBag.TerrainId = new SelectList(db.Terrains, "TerrainId", "Topography");
            ViewBag.UbicationId = new SelectList(db.Ubications, "UbicationId", "Name");
            return View();
        }

        // POST: Articles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArticleId,Description,State,Code,Price,TerrainId,UbicationId,IndividualContributorId,Currency")]
        Article article, Terrain terrain, House house, HouseAux houseAux, string[] urls, int[] houseFeatures, int[] houseFeaturesAux, string outstandingPicture)
        {
            if (ModelState.IsValid)
            {
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name", article.IndividualContributorId);
            ViewBag.TerrainId = new SelectList(db.Terrains, "TerrainId", "Topography", article.TerrainId);
            ViewBag.UbicationId = new SelectList(db.Ubications, "UbicationId", "Name", article.UbicationId);
            return View(article);
        }

        // GET: Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name", article.IndividualContributorId);
            ViewBag.TerrainId = new SelectList(db.Terrains, "TerrainId", "Topography", article.TerrainId);
            ViewBag.UbicationId = new SelectList(db.Ubications, "UbicationId", "Name", article.UbicationId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArticleId,Description,State,Code,Price,TerrainId,UbicationId,IndividualContributorId")] Article article)
        {
            if (ModelState.IsValid)
            {
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name", article.IndividualContributorId);
            ViewBag.TerrainId = new SelectList(db.Terrains, "TerrainId", "Topography", article.TerrainId);
            ViewBag.UbicationId = new SelectList(db.Ubications, "UbicationId", "Name", article.UbicationId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void GetFeatures()
        {

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
