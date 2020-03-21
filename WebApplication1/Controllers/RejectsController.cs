using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    public class RejectsController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: Rejects
        public ActionResult Index()
        {

            var rejects = from r in db.Rejects
                          where r.Article.IndividualContributor.Mail == User.Identity.Name
                          select r;

            ViewBag.ArticlePictures = GetPictureList();
            List<Reject> rejectList = rejects.ToList();
            return View(rejectList);
        }


        public bool Create(int ArticleId, string Reason)
        {
            try
            {
                Reject reject = new Reject();
                reject.ArticleId = ArticleId;
                reject.Reason = Reason;
                db.Rejects.Add(reject);
                db.SaveChanges();
                return true;

            }
            catch (Exception)
            {

                return false;

            }
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

        public List<ArticlePicture> GetPictureList()
        {
            try
            {
                List<ArticlePicture> PicturesList = db.ArticlePictures.ToList();

                return PicturesList;
            }
            catch (Exception ex)
            {
                ViewBag.error += "Error al obtener las fotos" + ex.InnerException;
                return null;
            }

        }
        public void ApprovesBadge()
        {
            Article article = new Article();

            Session["rejects"] = article.RefreshRejects(User.Identity.Name);
            Session["approve"] = article.RefreshApproves();

        }


        protected override void Dispose(bool disposing)
        {
            ApprovesBadge();

            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
