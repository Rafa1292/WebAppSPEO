using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage;
using System.IO;
using Microsoft.Azure;
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
    [Authorize(Roles = "Create")]

    public class UbicationsController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: Ubications
        public ActionResult Index()
        {
            reloadViewBags();
            return View(db.Ubications.ToList());
        }

        // GET: Ubications/Create
        public ActionResult Create()
        {
            ViewBag.UbicationCategoryId = db.UbicationCategory.ToList();
            ViewBag.UbicationFeaturesId = new SelectList(db.UbicationFeatures, "UbicationFeatureId", "Description");
            ViewBag.CantonId = new SelectList(db.Cantons, "CantonId", "Name");

            return View();
        }

        public string AddBlobToStorage(int UbicationId, string b64String)
        {
            var fecha = DateTime.Now.ToString("hhmmss");
            string name = "Foto" + fecha + UbicationId.ToString();
            byte[] pictureBytes = Convert.FromBase64String(b64String);
            string keys = CloudConfigurationManager.GetSetting("ConnectionBlob");
            CloudStorageAccount cuentaAlmacenamiento = CloudStorageAccount.Parse(keys);
            CloudBlobClient clienteBlob = cuentaAlmacenamiento.CreateCloudBlobClient();
            CloudBlobContainer container = clienteBlob.GetContainerReference("ubicationpictures");
            CloudBlockBlob blob = container.GetBlockBlobReference(name);

            using (var stream = new MemoryStream(pictureBytes))
            {
                blob.UploadFromStream(stream);
            }
            return name;

        }

        // POST: Ubications/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ubication ubication, string[] urls, int[] ubicationFeatures, string CantonId, string outstandingPicture)
        {
            if (ubication.Name != null && ubication.DistritId > 0 && ubicationFeatures.Length > 0 && outstandingPicture != null)
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
                        picture.Extension = AddBlobToStorage(ubication.UbicationId, outstandingPicture);
                        picture.UbicationId = ubication.UbicationId;
                        Pictures.Add(picture);
                        if (urls != null)
                        {
                            foreach (var url in urls)
                            {
                                picture = new UbicationPicture();
                                picture.OutstandingPicture = false;
                                picture.Extension = AddBlobToStorage(ubication.UbicationId, url);
                                picture.UbicationId = ubication.UbicationId;
                                Pictures.Add(picture);
                            }
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
        public ActionResult Edit([Bind(Include = "UbicationId,Description,Name,DistritId,UbicationCategoryId")] Ubication ubication, string[] urls, int[] ubicationFeatures, string CantonId, string outstandingPicture)
        {
            if (ubication.Name != null && ubication.DistritId > 0 && ubicationFeatures.Length > 0 && outstandingPicture != null)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(ubication).State = EntityState.Modified;
                        db.SaveChanges();


                        //---------------Pictures section---------------//

                        UbicationPicture ubicationPicture = new UbicationPicture();
                        List<UbicationPicture> ubicationPictures = new List<UbicationPicture>();

                        IQueryable<UbicationPicture> currentPicturesEF = from p in db.UbicationPictures
                                                                         where p.UbicationId == ubication.UbicationId
                                                                         select p;

                        //Tenemos la lista de fotos actuales y la foto de portada actual en formato de byte.
                        List<UbicationPicture> currentPicturesList = currentPicturesEF.ToList();
                        UbicationPicture currentOutstandingPicture = currentPicturesList.Find(p => p.OutstandingPicture == true);

                        SetOutstandingPicture(currentOutstandingPicture, currentPicturesList, outstandingPicture, ubication.UbicationId);

                        var i = 0;
                        foreach (var url in urls)
                        {
                            if (!currentPicturesList.Exists(x => x.Extension == url))
                            {
                                ubicationPicture = new UbicationPicture();
                                ubicationPicture.OutstandingPicture = false;
                                ubicationPicture.Extension = AddBlobToStorage(ubication.UbicationId, url, i);
                                ubicationPicture.UbicationId = ubication.UbicationId;
                                ubicationPictures.Add(ubicationPicture);
                            }
                            i++;
                        }

                        db.UbicationPictures.AddRange(ubicationPictures);
                        db.SaveChanges();
                        //---------------End pictures section---------------//


                        //---------------Features section---------------//

                        IQueryable<UbicationFeatureUbication> currentFeatures = from f in db.UbicationFeaturesUbication
                                                                                where f.UbicationId == ubication.UbicationId
                                                                                select f;
                        List<UbicationFeatureUbication> currentFeaturesList = currentFeatures.ToList();

                        foreach (var currentFeature in currentFeaturesList)
                        {
                            var NoExists = true;
                            foreach (var feature in ubicationFeatures)
                            {
                                if (currentFeature.UbicationFeatureId == feature)
                                {
                                    NoExists = false;
                                }
                            }

                            if (NoExists)
                            {
                                db.UbicationFeaturesUbication.Remove(currentFeature);
                            }
                        }

                        List<UbicationFeatureUbication> ubicationFeaturesToAdd = new List<UbicationFeatureUbication>();

                        foreach (var feature in ubicationFeatures)
                        {
                            var Exists = false;
                            foreach (var currentFeature in currentFeaturesList)
                            {
                                if (currentFeature.UbicationFeatureId == feature)
                                {
                                    Exists = true;
                                }
                            }

                            if (!Exists)
                            {
                                UbicationFeatureUbication ubicationFeatureUbication = new UbicationFeatureUbication();
                                ubicationFeatureUbication.UbicationFeatureId = feature;
                                ubicationFeatureUbication.UbicationId = ubication.UbicationId;
                                ubicationFeaturesToAdd.Add(ubicationFeatureUbication);
                            }
                        }
                        db.UbicationFeaturesUbication.AddRange(ubicationFeaturesToAdd);
                        db.SaveChanges();
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
            reloadViewBags();
            return View(ubication);
        }

        [Authorize(Roles = "Admin")]
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

        public string FillFeaturesModal(int UbicationId)
        {
            IQueryable<UbicationFeatureUbication> ubicationFeatureUbications = from f in db.UbicationFeaturesUbication
                                                                               where f.UbicationId == UbicationId
                                                                               select f;
            List<UbicationFeatureUbication> ubicationFeatureUbicationsList = ubicationFeatureUbications.ToList();
            List<UbicationFeature> ubicationFeature = db.UbicationFeatures.ToList();

            string content = "";
            foreach (var Feature in ubicationFeature)
            {
                if (ubicationFeatureUbicationsList.Any(uf => uf.UbicationFeatureId == Feature.UbicationFeatureId))
                {
                    content += "<li class=\"list-group-item\">" + Feature.Description + "</li>";
                }
            }

            return content;

        }

        // Recarga viewbags en caso de error 
        private void reloadViewBags(List<string> urls, List<int> ubicationFeatures, string outstandingPicture, string CantonId, Ubication ubication)
        {
            ViewBag.UbicationCategoryId = db.UbicationCategory.ToList();
            ViewBag.SelectedCategory = ubication.UbicationCategoryId.ToString();
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

        private void SetOutstandingPicture(UbicationPicture currentOutstandingPicture, List<UbicationPicture> currentPicturesList, string outstandingPicture, int id)
        {
            if (currentOutstandingPicture != null)
            {
                if (currentOutstandingPicture.Extension == outstandingPicture)
                {
                    return;
                }

                else
                {
                    //Si no son iguales primero cambiamos a false la opcion outstandingPicture de la foto actual, ya que esta no es mas la foto de portada
                    currentOutstandingPicture.OutstandingPicture = false;
                    db.Entry(currentOutstandingPicture).State = EntityState.Modified;
                    //Luego verificamos si la nueva foto de portada existe entre las fotos antiguas
                    //Para ello creamos una variable que nos almacenara la imagen completa si existe.
                    UbicationPicture outstandingPictureExists = null;
                    foreach (var currentPicture in currentPicturesList)
                    {
                        //comparamos
                        if (currentPicture.Extension == outstandingPicture)
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
                        picture.Extension = AddBlobToStorage(id, outstandingPicture, 20);
                        picture.UbicationId = id;
                        db.UbicationPictures.Add(picture);
                        db.SaveChanges();
                    }
                }
            }
            else
            {

                UbicationPicture picture = new UbicationPicture();
                picture.OutstandingPicture = true;
                picture.Extension = AddBlobToStorage(id, outstandingPicture, 20);
                picture.UbicationId = id;
                db.UbicationPictures.Add(picture);
                db.SaveChanges();

            }
        }


        public string AddBlobToStorage(int ubicationId, string b64String, int iterator)
        {
            var fecha = DateTime.Now.ToString("hhmmss");
            string name = "Foto" + fecha + ubicationId.ToString() + iterator.ToString();
            byte[] pictureBytes = Convert.FromBase64String(b64String);
            string keys = CloudConfigurationManager.GetSetting("ConnectionBlob");
            CloudStorageAccount cuentaAlmacenamiento = CloudStorageAccount.Parse(keys);
            CloudBlobClient clienteBlob = cuentaAlmacenamiento.CreateCloudBlobClient();
            CloudBlobContainer container = clienteBlob.GetContainerReference("ubicationpictures");
            CloudBlockBlob blob = container.GetBlockBlobReference(name);

            using (var stream = new MemoryStream(pictureBytes))
            {
                blob.UploadFromStream(stream);
            }
            return name;

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
                    outstandingPicture = picture.Extension;
                }
                else
                {
                    var url = picture.Extension;
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

        [HttpPost]
        public string DeletePicture(string extension)
        {
            var state = "false";
            try
            {
                UbicationPicture ubicationPicture = db.UbicationPictures.FirstOrDefault(p => p.Extension == extension);
                if (ubicationPicture != null)
                {
                    db.UbicationPictures.Remove(ubicationPicture);
                    db.SaveChanges();
                    state = "true";
                    return state;
                }
                else
                {
                    return state;
                }
            }
            catch (Exception)
            {

                return state;
            }
        }

        // Recarga viewbags en caso de error 
        private void reloadViewBags()
        {
            IQueryable<UbicationPicture> OutstandingPictures = from p in db.UbicationPictures
                                                               where p.OutstandingPicture == true && p.Extension != null
                                                               select p;


            ViewBag.Selectedurl = OutstandingPictures.ToList();
            ViewBag.UbicationCategoryId = db.UbicationCategory.ToList();
            ViewBag.DistritId = db.Distrits.ToList();
            ViewBag.CantonId = db.Cantons.ToList();
            ViewBag.UbicationFeaturesId = db.UbicationFeatures.ToList();
            ViewBag.UbicationFeaturesUbication = db.UbicationFeaturesUbication.ToList();
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
