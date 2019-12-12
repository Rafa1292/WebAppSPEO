using Newtonsoft.Json;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UbicationsController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: Ubications
        public ActionResult Index()
        {
            var ubications = db.Ubications.Include(u => u.Distrit);
            return View(ubications.ToList());
        }



        // GET: Ubications/Create
        public ActionResult Create()
        {
            ViewBag.UbicationFeaturesId = new SelectList(db.UbicationFeatures, "UbicationFeatureId", "Description");
            ViewBag.CantonId = new SelectList(db.Cantons, "CantonId", "Name");

            return View();
        }

        // POST: Ubications/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UbicationId,Name,DistritId")] Ubication ubication, string[] urls, int[] ubicationFeatures, string CantonId, string outstandingPicture)
        {

            if (ubication.Name != null && ubication.DistritId > 0 && urls.Length > 0 && ubicationFeatures.Length > 0 && outstandingPicture != null)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Ubications.Add(ubication);
                        db.SaveChanges();

                        List<UbicationPicture> Pictures = new List<UbicationPicture>();
                        List<UbicationFeatureUbication> Features = new List<UbicationFeatureUbication>();
                        UbicationPicture picture = new UbicationPicture();
                        picture.OutstandingPicture = true;
                        picture.PictureArray = Convert.FromBase64String(outstandingPicture);
                        picture.UbicationId = ubication.UbicationId;
                        foreach (var url in urls)
                        {
                            picture = new UbicationPicture();
                            picture.OutstandingPicture = false;
                            picture.PictureArray = Convert.FromBase64String(url);
                            picture.UbicationId = ubication.UbicationId;
                            Pictures.Add(picture);
                        }

                        foreach (var feature in ubicationFeatures)
                        {
                            UbicationFeatureUbication ubicationFeature = new UbicationFeatureUbication();
                            ubicationFeature.UbicationFeatureId = feature;
                            ubicationFeature.UbicationId = ubication.UbicationId;
                            Features.Add(ubicationFeature);
                        }

                        db.UbicationPictures.AddRange(Pictures);
                        db.UbicationFeaturesUbication.AddRange(Features);
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ViewBag.Error = ex.Message;
                        reloadViewBags(urls, ubicationFeatures, outstandingPicture, CantonId, ubication);
                        return View(ubication);

                    }
                }
            }
            reloadViewBags(urls, ubicationFeatures, outstandingPicture, CantonId, ubication);
            return View(ubication);
        }


        // Recarga viewbags en caso de error 
        private void reloadViewBags(string[] urls, int[] ubicationFeatures, string outstandingPicture, string CantonId, Ubication ubication)
        {
            ViewBag.SelectedCanton = CantonId;
            ViewBag.Selectedurl = outstandingPicture;
            ViewBag.urls = urls;
            ViewBag.SelectedDistrit = ubication.DistritId.ToString();
            var distrits = from d in db.Distrits
                           where d.CantonId.ToString() == CantonId
                           select d;
            ViewBag.DistritId = new SelectList(distrits.ToList(), "DistritId", "Name");
            ViewBag.CantonId = new SelectList(db.Cantons.ToList(), "CantonId", "Name");
            ViewBag.UbicationFeaturesId = new SelectList(db.UbicationFeatures, "UbicationFeatureId", "Description");
            ViewBag.SelectedUbicationFeatures = ubicationFeatures;
        }

        // GET: Ubications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ubication ubication = db.Ubications.Find(id);
            if (ubication == null)
            {
                return HttpNotFound();
            }
            ViewBag.DistritId = new SelectList(db.Distrits, "DistritId", "Name", ubication.DistritId);
            return View(ubication);
        }

        // POST: Ubications/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UbicationId,Name,DistritId")] Ubication ubication)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ubication).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DistritId = new SelectList(db.Distrits, "DistritId", "Name", ubication.DistritId);
            return View(ubication);
        }

        // GET: Ubications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ubication ubication = db.Ubications.Find(id);
            if (ubication == null)
            {
                return HttpNotFound();
            }
            return View(ubication);
        }

        // POST: Ubications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ubication ubication = db.Ubications.Find(id);
            db.Ubications.Remove(ubication);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public string GetDistrits(int CantonId)
        {
            List<Distrit> FilterList = db.Distrits.Where(x => x.CantonId == CantonId).ToList();
            string data = "";
            data += "<option value=0 disabled selected>Seleccione un distrito</option >";

            foreach (var distrit in FilterList)
            {
                data += "<option value = " + distrit.DistritId + " >" + distrit.Name + "</option >";
            }
            return data;
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
