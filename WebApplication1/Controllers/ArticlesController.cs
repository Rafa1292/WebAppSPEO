using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Web.Script.Serialization;
using WebApplication1.ViewModel;

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
        public ActionResult Create(ArticleViewModel articleViewModel )
        {
            if (articleViewModel.Article != null && articleViewModel.Terrain != null && articleViewModel.Urls.Length > 0 && articleViewModel.TerrainFeatures.Length > 0 && articleViewModel.OutstandingPicture != null)
            {
                Article article = articleViewModel.Article;
                Terrain terrain = articleViewModel.Terrain;
                string outstandingPicture = articleViewModel.OutstandingPicture;
                string[] urls = articleViewModel.Urls;
                House house = articleViewModel.House;
                HouseAux houseAux = articleViewModel.HouseAux;
                int[] terrainFeatures = articleViewModel.TerrainFeatures;
                int[] HouseFeatures = articleViewModel.HouseFeatures;
                int[] HouseAuxFeatures = articleViewModel.HouseAuxFeatures;

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Terrains.Add(terrain);
                        db.SaveChanges();

                        article.Currency = articleViewModel.Currency;
                        article.Description = articleViewModel.Description;
                        article.IndividualContributorId = articleViewModel.IndividualContributorId;
                        article.UbicationId = articleViewModel.UbicationId;
                        article.Code = "A" + terrain.TerrainId;
                        article.State = false;
                        article.TerrainId = terrain.TerrainId;                        
                        db.Articles.Add(article);

                        db.SaveChanges();
                        var a = 0;
                        var i = 2;
                        if (a < 5)
                        {
                            i /= a;
                        }

                        List<ArticlePicture> Pictures = new List<ArticlePicture>();
                        List<TerrainFeatureTerrain> Features = new List<TerrainFeatureTerrain>();
                        ArticlePicture picture = new ArticlePicture();
                        picture.OutstandingPicture = true;
                        picture.PictureArray = Convert.FromBase64String(outstandingPicture);
                        picture.ArticleId = article.Id;
                        Pictures.Add(picture);
                        foreach (var url in urls)
                        {
                            picture = new ArticlePicture();
                            picture.OutstandingPicture = false;
                            picture.PictureArray = Convert.FromBase64String(url);
                            picture.ArticleId = article.Id;
                            Pictures.Add(picture);
                        }

                        foreach (var feature in terrainFeatures)
                        {
                            TerrainFeatureTerrain terrainFeature = new TerrainFeatureTerrain();
                            terrainFeature.TerrainFeatureId = feature;
                            terrainFeature.TerrainId = article.Id;
                            Features.Add(terrainFeature);
                        }

                        db.ArticlePictures.AddRange(Pictures);
                        db.TerrainFeaturesTerrain.AddRange(Features);
                        if (house.Levels != 0)
                        {
                            AddHouse(house, HouseFeatures, article.Id);
                        }
                        if (houseAux.LevelsAux != 0)
                        {
                            House aux = new House();
                            aux.Bathrooms = houseAux.BathroomsAux;
                            aux.Bedrooms = houseAux.BedroomsAux;
                            aux.Garage = houseAux.GarageAux;
                            aux.HouseBackgroundMeasure = houseAux.HouseBackgroundMeasureAux;
                            aux.HouseForeheadMeasure = houseAux.HouseForeheadMeasureAux;
                            aux.Levels = houseAux.LevelsAux;
                            AddHouse(aux, HouseAuxFeatures, article.Id);

                        }
                        db.SaveChanges();
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ViewbagFeatures(terrainFeatures, HouseFeatures, HouseAuxFeatures);
                        ViewBag.Currency = articleViewModel.Currency;
                        ViewBag.SelectedUbication = articleViewModel.UbicationId;
                        ViewBag.SelectedColaborator = articleViewModel.IndividualContributorId;
                        ViewBag.Selectedurl = outstandingPicture;
                        ViewBag.urls = urls;
                        ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name");
                        ViewBag.TerrainId = new SelectList(db.Terrains, "TerrainId", "Topography");
                        ViewBag.UbicationId = new SelectList(db.Ubications, "UbicationId", "Name");
                        ViewBag.error = ex.Message;
                        return View(articleViewModel);
                    }
                }
            }
            return View(articleViewModel);
        }

        public void AddHouse( House house, int[] features, int articleId)
        {
            house.ArticleId = articleId;
            db.Houses.Add(house);
            db.SaveChanges();
            AddFeatures(features, house.HouseId);



        }

        public void AddFeatures(int[] features, int houseId)
        {
            HouseFeatureHouse houseFeatureHouse = new HouseFeatureHouse();
            foreach (var feature in features)
            {
                houseFeatureHouse = new HouseFeatureHouse();
                houseFeatureHouse.HouseFeatureId = feature;
                houseFeatureHouse.HouseId = houseId;
                db.HouseFeatureHouse.Add(houseFeatureHouse);
            }
            
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

        public void ViewbagFeatures(int[] terrainSelected, int[] houseSelected, int[] houseAuxSelected)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var terrainFeatures = jsonSerialiser.Deserialize<List<Feature>>(GetFeatures("Terrain"));
            var houseFeatures = jsonSerialiser.Deserialize<List<Feature>>(GetFeatures("House"));
            var houseAuxFeatures = jsonSerialiser.Deserialize<List<Feature>>(GetFeatures("HouseAux"));

            ViewBag.terrainFeaturesSelected =  FeatureFilter(terrainFeatures, terrainSelected);
            ViewBag.houseFeaturesSelected =  houseSelected != null && houseSelected.Length > 0 ? FeatureFilter(houseFeatures, houseSelected) : null;
            ViewBag.houseAuxFeaturesSelected = houseAuxSelected != null && houseAuxSelected.Length > 0 ? FeatureFilter(houseAuxFeatures, houseAuxSelected) : null;
        }

        public List<Feature> FeatureFilter(List<Feature> featuresList, int[] featuresSelected)
        {
            List<Feature> filterFeatures = new List<Feature>();

            foreach (var feature in featuresList)
            {
                if (Array.Exists(featuresSelected, f => f == feature.FeatureId))
                {
                    filterFeatures.Add(feature);
                }
            }

            return filterFeatures;
        }

        public string GetFeatures(string model)
        {
            List<Feature> features = new List<Feature>();

            switch (model)
            {
                case "Terrain":
                    List<TerrainFeature> terrainFeatures = db.TerrainFeatures.ToList();
                    Feature terrainFeature = new Feature();
                    foreach (var feature in terrainFeatures)
                    {
                        terrainFeature = new Feature();
                        terrainFeature.FeatureId = feature.TerrainFeatureId;
                        terrainFeature.Description = feature.Description;
                        terrainFeature.Model = "Terrain";
                        features.Add(terrainFeature);
                    }
                    break;
                case "House":

                    List<HouseFeature> houseFeatures = db.HouseFeatures.ToList();
                    Feature houseFeature = new Feature();
                    foreach (var feature in houseFeatures)
                    {
                        houseFeature = new Feature();
                        houseFeature.FeatureId = feature.HouseFeatureId;
                        houseFeature.Description = feature.Description;
                        houseFeature.Model = "House";
                        features.Add(houseFeature);
                    }
                    break;
                case "HouseAux":

                    List<HouseFeature> houseAuxFeatures = db.HouseFeatures.ToList();
                    Feature houseAuxFeature = new Feature();
                    foreach (var feature in houseAuxFeatures)
                    {
                        houseAuxFeature = new Feature();
                        houseAuxFeature.FeatureId = feature.HouseFeatureId;
                        houseAuxFeature.Description = feature.Description;
                        houseAuxFeature.Model = "HouseAux";
                        features.Add(houseAuxFeature);
                    }
                    break;

            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(features);
            return json;
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
