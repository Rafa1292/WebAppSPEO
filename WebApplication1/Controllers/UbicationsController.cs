﻿using Newtonsoft.Json;
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
                        Pictures.Add(picture);
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
                        reloadViewBags(urls.ToList(), ubicationFeatures.ToList(), outstandingPicture, CantonId, ubication);
                        return View(ubication);

                    }
                }
            }
            reloadViewBags(urls.ToList(), ubicationFeatures.ToList(), outstandingPicture, CantonId, ubication);
            return View(ubication);
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

            SetDataToViewBags(ubication);
            return View(ubication);
        }

        // POST: Ubications/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UbicationId,Name,DistritId")] Ubication ubication, string[] urls, int[] ubicationFeatures, string CantonId, string outstandingPicture)
        {
            if (ubication.Name != null && ubication.DistritId > 0 && urls.Length > 0 && ubicationFeatures.Length > 0 && outstandingPicture != null)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(ubication).State = EntityState.Modified;
                        db.SaveChanges();


                        //---------------Pictures section---------------//
                        IQueryable<UbicationPicture> currentPictures = from p in db.UbicationPictures
                                                                       where p.UbicationId == ubication.UbicationId
                                                                       select p;
                        //Tenemos la lista de fotos actuales y la foto de portada actual en formato de byte.
                        List<UbicationPicture> currentPicturesList = currentPictures.ToList();
                        UbicationPicture currentOutstandingPicture = currentPicturesList.Find(p => p.OutstandingPicture == true);

                        //Tenemos la lista de fotos nuevas y la foto de portada nueva en formato b64.
                        List<string> newPicturesList = urls.ToList();
                        //outstandingPicture = outstandingPicture;

                        //<-----------------Foto de portada----------------->//

                        /*Comparamos foto actual de portada y foto nueva en caso de que sean iguales no hacemos nada
                         *Para poder comparar necesitamos pasar lo que esta en formato byte a b64.*/
                        string currentOutstandingPictureBase64 = Convert.ToBase64String(currentOutstandingPicture.PictureArray);

                        //Procedemos a comparar
                        if (!(outstandingPicture == currentOutstandingPictureBase64))
                        {

                            SetOutstandingPicture(currentOutstandingPicture, currentPicturesList, outstandingPicture, ubication.UbicationId);
                        }

                        //<-----------------Agregar nuevas fotos----------------->//
                        /*Para agregar nuevas fotos vamos a separar de la lista nueva las fotos que existen en la lista antigua
                         * una vez separadas agregamos las fotos que no existian.
                        */
                        AddNewPictures(urls, currentPicturesList, ubication.UbicationId);
                        DeletePictures(urls, currentPicturesList, outstandingPicture);
                        //---------------End pictures section---------------//


                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            reloadViewBags(urls.ToList(), ubicationFeatures.ToList(), outstandingPicture, CantonId, ubication);
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

        // Recarga viewbags en caso de error 
        private void reloadViewBags(List<string> urls, List<int> ubicationFeatures, string outstandingPicture, string CantonId, Ubication ubication)
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

        // Prepara informacion para viewBags  
        private void SetDataToViewBags(Ubication ubication)
        {
            string outstandingPicture = "";
            List<string> urls = new List<string>();
            var pictures = from p in db.UbicationPictures
                           where p.UbicationId == ubication.UbicationId
                           select p;
            foreach (var picture in pictures)
            {
                if (picture.OutstandingPicture)
                {
                    outstandingPicture = Convert.ToBase64String(picture.PictureArray);
                }
                else
                {
                    var url = Convert.ToBase64String(picture.PictureArray);
                    urls.Add(url);
                }
            }

            List<int> ubicationFeatures = new List<int>();
            var ubicationFeaturesList = db.UbicationFeaturesUbication.ToList();
            foreach (var feature in ubicationFeaturesList)
            {
                if (feature.UbicationId == ubication.UbicationId)
                {
                    ubicationFeatures.Add(feature.UbicationFeatureId);
                }
            }
            string CantonId = "";
            Distrit distrit = db.Distrits.Find(ubication.DistritId);
            var cantons = db.Cantons.ToList();
            foreach (var canton in cantons)
            {
                if (canton.CantonId == distrit.CantonId)
                {
                    CantonId = canton.CantonId.ToString();
                }
            }

            reloadViewBags(urls, ubicationFeatures, outstandingPicture, CantonId, ubication);
        }

        //En caso de cambiar la foto de portada se ejecuta este bloque de codigo, sabemos si cambia mediante una comparacion previa.
        private void SetOutstandingPicture(UbicationPicture currentOutstandingPicture, List<UbicationPicture> currentPicturesList, string outstandingPicture, int id)
        {
            //Si no son iguales primero cambiamos a false la opcion outstandingPicture de la foto actual, ya que esta no es mas la foto de portada
            currentOutstandingPicture.OutstandingPicture = false;
            db.Entry(currentOutstandingPicture).State = EntityState.Modified;
            //Luego verificamos si la nueva foto de portada existe entre las fotos antiguas
            //Para ello creamos una variable que nos almacenara la imagen completa si existe.
            UbicationPicture outstandingPictureExists = null;
            foreach (var currentPicture in currentPicturesList)
            {
                //Debemos pasar currentPicture a b64 para poder comparar
                string currentPictureInB64 = Convert.ToBase64String(currentPicture.PictureArray);
                //comparamos
                if (currentPictureInB64 == outstandingPicture)
                {
                    outstandingPictureExists = currentPicture;
                }
            }
            // si la nueva foto de portada existe solo se le cambia el valor outstandingPicture a true
            if (outstandingPictureExists != null)
            {
                outstandingPictureExists.OutstandingPicture = true;
                db.Entry(outstandingPictureExists).State = EntityState.Modified;
            }
            //si la foto no existe debemos crearla y agregarla con el valor outstandingPicture en true.
            else
            {
                UbicationPicture picture = new UbicationPicture();
                picture.OutstandingPicture = true;
                picture.PictureArray = Convert.FromBase64String(outstandingPicture);
                picture.UbicationId = id;
                db.UbicationPictures.Add(picture);
            }
            db.SaveChanges();
        }

        private void AddNewPictures(string[] urls, List<UbicationPicture> currentPicturesList, int id)
        {
            //Creamos lista que almacenara fotos nuevas para añadirlas a la base de datos
            List<UbicationPicture> picturesToAdd = new List<UbicationPicture>();
            foreach (var url in urls)
            {
                var exists = false;
                foreach (var currentPicture in currentPicturesList)
                {
                    string currentPictureBase64 = Convert.ToBase64String(currentPicture.PictureArray);
                    if (currentPictureBase64 == url)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    UbicationPicture picture = new UbicationPicture();
                    picture.OutstandingPicture = false;
                    picture.PictureArray = Convert.FromBase64String(url);
                    picture.UbicationId = id;
                    picturesToAdd.Add(picture);
                }
            }
            //Finalizado el ciclo de verificacion procedemos a añadir las nuevas imagenes
            db.UbicationPictures.AddRange(picturesToAdd);
            db.SaveChanges();
        }

        private void DeletePictures(string[] urls, List<UbicationPicture> currentPicturesList, string outstandingPicture)
        {
            //Creamos lista que almacenara fotos nuevas para añadirlas a la base de datos
            List<UbicationPicture> picturesToAdd = new List<UbicationPicture>();
            foreach (var currentPicture in currentPicturesList)
            {
                var noExists = true;
                foreach (var url in urls)
                {
                    string currentPictureBase64 = Convert.ToBase64String(currentPicture.PictureArray);
                    if (currentPictureBase64 == url || currentPictureBase64 == outstandingPicture)
                    {
                        noExists = false;
                    }
                }
                if (noExists)
                {
                    db.UbicationPictures.Remove(currentPicture);
                }

            }

            db.SaveChanges();
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
