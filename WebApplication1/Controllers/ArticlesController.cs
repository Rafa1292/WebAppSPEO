using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Create")]

    public class ArticlesController : Controller
    {
        public WebApplication1Context db = new WebApplication1Context();

        // GET: Articles
        public ActionResult Index()
        {

            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList();
            ViewBag.ArticleKindId = EnumHelper.GetSelectList(typeof(EArticleKind));

            return View(ArticleViewModelList.ToList());
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            ViewBag.houseForm = "none";
            ViewBag.houseAuxForm = "none";
            ViewBag.TerrainId = new SelectList(db.Terrains.ToList(), "TerrainId", "Topography");
            ViewBag.UbicationId = new SelectList(db.Ubications.ToList(), "UbicationId", "Name");
            return View();
        }

        // POST: Articles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArticleViewModel articleViewModel, string[] urls, int[] Article_TerrainFeatures, string outstandingPicture, int[] Article_HouseFeatures,
            int[] Article_HouseAuxFeatures)
        {
            //HttpFileCollectionBase files = Request.Files;


            if (articleViewModel.Article != null && articleViewModel.Article.Terrain != null && urls.Length > 0
                && Article_TerrainFeatures.Length > 0 && outstandingPicture != null)
            {
                Article article = articleViewModel.Article;
                Terrain terrain = article.Terrain;

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        try
                        {
                            db.Terrains.Add(terrain);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ViewBag.error = "Error al añadir terreno" + ex.InnerException;
                            return View("ErrorPage");
                        }

                        try
                        {
                            IndividualContributor individualContributor = db.IndividualContributors.FirstOrDefault(i => i.Mail == User.Identity.Name);
                            article.IndividualContributorId = individualContributor.IndividualContributorId;
                            article.Code = "A" + terrain.TerrainId;
                            article.State = false;
                            article.TerrainId = terrain.TerrainId;
                            article.SoldState = false;
                            article.CreationDate = DateTime.Now;
                            article.ArticleKind = EArticleKind.Venta;
                            db.Articles.Add(article);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ViewBag.error = "Error al añadir casa" + ex.InnerException;
                            return View("ErrorPage");

                        }



                        AddTerrainFeatures(Article_TerrainFeatures, terrain.TerrainId);


                        //for (int i = 0; i < files.Count; i++)
                        //{
                        //    HttpPostedFileBase file = files[i];
                        //    if (file != null)
                        //    {
                        //        SubirArchivo(file, article.Id);
                        //    }
                        //}

                        try
                        {
                            List<ArticlePicture> Pictures = new List<ArticlePicture>();
                            ArticlePicture picture = new ArticlePicture();
                            picture.OutstandingPicture = true;
                            picture.Extension = AddBlobToStorage(article.Id, outstandingPicture, 20);
                            picture.ArticleId = article.Id;
                            Pictures.Add(picture);

                            var i = 0;
                            foreach (var url in urls)
                            {
                                picture = new ArticlePicture();
                                picture.OutstandingPicture = false;
                                picture.Extension = AddBlobToStorage(article.Id, url, i);
                                picture.ArticleId = article.Id;
                                Pictures.Add(picture);
                                i++;
                            }


                            db.ArticlePictures.AddRange(Pictures);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                            ViewBag.error = "Error al añadir fotos" + ex.InnerException;
                            return View("ErrorPage");

                        }



                        if (articleViewModel.House.HouseForeheadMeasure > 0 && articleViewModel.House.HouseBackgroundMeasure > 0)
                        {
                            AddHouse(articleViewModel.House, Article_HouseFeatures, article.Id);
                        }

                        if (articleViewModel.HouseAux.HouseForeheadMeasureAux > 0 && articleViewModel.HouseAux.HouseBackgroundMeasureAux > 0)
                        {
                            HouseAux houseAux = articleViewModel.HouseAux;
                            House house = new House();
                            house.Bathrooms = houseAux.BathroomsAux;
                            house.Bedrooms = houseAux.BedroomsAux;
                            house.Garage = houseAux.GarageAux;
                            house.HouseBackgroundMeasure = houseAux.HouseBackgroundMeasureAux;
                            house.HouseForeheadMeasure = houseAux.HouseForeheadMeasureAux;
                            house.Levels = houseAux.LevelsAux;
                            AddHouse(house, Article_HouseAuxFeatures, article.Id);
                        }


                        db.SaveChanges();
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }

                    catch (Exception ex)
                    {
                        ViewBag.houseForm = "none";
                        ViewBag.houseAuxForm = "none";
                        ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name");
                        ViewBag.TerrainId = new SelectList(db.Terrains, "TerrainId", "Topography");
                        ViewBag.UbicationId = new SelectList(db.Ubications, "UbicationId", "Name");
                        transaction.Rollback();
                        ReloadViewBags(articleViewModel);
                        ViewBag.error = ex.InnerException;
                        return View(articleViewModel);
                    }
                }
            }
            ReloadViewBags(articleViewModel);
            return View(articleViewModel);
        }

        // GET: Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.error = "El id no es valido";
                return View("ErrorPage");
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                ViewBag.error = "El articulo no existe";
                return View("ErrorPage");
            }

            Reject reject = db.Rejects.FirstOrDefault(r => r.ArticleId == id);

            if (reject != null)
            {
                ViewBag.error = reject.Reason;

            }

            //var filesEF = from f in db.Archivos
            //              where f.ArticleId == id
            //              select f;
            //ViewBag.Files = filesEF.ToList();
            ArticleViewModel articleViewModel = GetArticleViewModel(article);
            ReloadViewBags(articleViewModel);
            return View(articleViewModel);
        }

        // POST: Articles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArticleViewModel articleViewModel, string[] urls, int[] Article_TerrainFeatures, string outstandingPicture, int[] Article_HouseFeatures,
            int[] Article_HouseAuxFeatures)
        {
            Article article = db.Articles.Find(articleViewModel.Article.Id);

            HttpFileCollectionBase files = Request.Files;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (articleViewModel.Article != null)
                    {
                        article.UbicationId = articleViewModel.Article.UbicationId;
                        article.Currency = articleViewModel.Article.Currency;
                        article.Description = articleViewModel.Article.Description;
                        article.OwnerName = articleViewModel.Article.OwnerName;
                        article.OwnerPhone = articleViewModel.Article.OwnerPhone;
                        article.Price = articleViewModel.Article.Price;
                        article.State = false;
                        db.Entry(article).State = EntityState.Modified;
                    }

                    //for (int i = 0; i < files.Count; i++)
                    //{
                    //    HttpPostedFileBase file = files[i];
                    //    SubirArchivo(file, article.Id);
                    //}

                    if (articleViewModel.Article.Terrain != null)
                    {
                        db.Entry(articleViewModel.Article.Terrain).State = EntityState.Modified;
                    }

                    EditTerrainFeatures(articleViewModel.Article.Terrain, Article_TerrainFeatures);

                    articleViewModel.House = articleViewModel.House.Levels > 0 ? articleViewModel.House : null;
                    articleViewModel.HouseAux = articleViewModel.HouseAux.LevelsAux > 0 ? articleViewModel.HouseAux : null;
                    List<House> HouseList = new List<House>();

                    if (articleViewModel.House != null)
                    {
                        articleViewModel.House.ArticleId = article.Id;
                        HouseList.Add(articleViewModel.House);
                    }

                    House house = articleViewModel.HouseAux != null ? new House() : null;

                    if (articleViewModel.HouseAux != null)
                    {
                        HouseAux houseAux = articleViewModel.HouseAux;
                        house = houseAux.Id > 0 ? db.Houses.Find(houseAux.Id) : new House();
                        house.ArticleId = article.Id;
                        house.Bathrooms = houseAux.BathroomsAux;
                        house.Bedrooms = houseAux.BedroomsAux;
                        house.Garage = houseAux.GarageAux;
                        house.HouseBackgroundMeasure = houseAux.HouseBackgroundMeasureAux;
                        house.HouseForeheadMeasure = houseAux.HouseForeheadMeasureAux;
                        house.Levels = houseAux.LevelsAux;
                        HouseList.Add(house);
                    }

                    CompareHouses(HouseList, article.Id, articleViewModel.House, house, Article_HouseFeatures, Article_HouseAuxFeatures);
                    UpdatePictures(articleViewModel.Article.Id, urls, outstandingPicture);

                    Reject reject = db.Rejects.FirstOrDefault(r => r.ArticleId == article.Id);

                    if (reject != null)
                    {
                        db.Rejects.Remove(reject);
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ReloadViewBags(articleViewModel);
                    return View(articleViewModel);
                }
            }


        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Admin")]
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
            ArticleViewModel articleViewModel = GetArticleViewModel(article);

            return View(articleViewModel);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            ApprovesBadge();
            return RedirectToAction("Index");
        }
        //-----------------------------------------------------------------------------------------//

        //---------------ClasificationSection---------------//        
        public string clasify(int articleId, int kindValue)
        {
            try
            {
                Article article = db.Articles.Find(articleId);

                var oportunityListEF = from a in db.Articles
                                       where a.ArticleKind == EArticleKind.Oportunidad
                                       select a;
                var outstandingListEF = from a in db.Articles
                                        where a.ArticleKind == EArticleKind.Sobresaliente
                                        select a;
                var oportunityList = oportunityListEF.ToList();
                var outstandingList = outstandingListEF.ToList();

                var previewKind = GetEnumValue(article);
                var status = false;
                dynamic result = new ExpandoObject();
                string newClass = "";
                switch (kindValue)
                {
                    case 0:
                        article.ArticleKind = EArticleKind.Venta;
                        status = true;
                        newClass = "Venta";
                        break;
                    case 1:
                        if (!(outstandingList.Count() >= 9))
                        {
                            article.ArticleKind = EArticleKind.Sobresaliente;
                            status = true;
                            newClass = "Sobresaliente";

                        }
                        break;
                    case 2:
                        if (!(oportunityList.Count() >= 9))
                        {
                            article.ArticleKind = EArticleKind.Oportunidad;
                            status = true;
                            newClass = "Oportunidad";
                        }
                        break;
                }

                result.Status = status;
                result.PreviewKind = previewKind;
                result.NewClass = newClass;
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(result);

                if (status)
                {
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();
                    return json;
                }
                else
                {
                    return json;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }
        public int GetEnumValue(Article article)
        {
            var id = 0;
            try
            {
                EArticleKind eArticleKind = article.ArticleKind;


                switch (eArticleKind)
                {
                    case EArticleKind.Sobresaliente:
                        id = 1;
                        break;
                    case EArticleKind.Oportunidad:
                        id = 2;
                        break;

                }
                return id;
            }
            catch (Exception)
            {

                return id;

            }

        }
        //-----------------------------------------------------------------------------------------//

        //---------------AjaxSection---------------//        
        public ActionResult ApproveArticles()
        {
            List<ArticleViewModel> ArticleViewModelList = ArticlesToApprove();



            return View(ArticleViewModelList);
        }

        public ActionResult ApproveArticlesPartialView()
        {
            List<ArticleViewModel> ArticleViewModelList = ArticlesToApprove();

            return PartialView("ApproveArticles", ArticleViewModelList);
        }

        public ActionResult PropertySold(int id, bool state)
        {
            try
            {
                Article article = db.Articles.Find(id);
                article.SoldState = !state;
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();

                var articles = db.Articles.ToList();


                List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

                ViewBag.ArticleKindId = EnumHelper.GetSelectList(typeof(EArticleKind));

                return PartialView("articleList", ArticleViewModelList);
            }
            catch (Exception)
            {
                ViewBag.error = "Error al cambiar el estado de la propiedad";
                return View("ErrorPage");
            }

        }

        public ActionResult ApproveArticle(int id)
        {
            try
            {
                Article article = db.Articles.Find(id);
                article.State = true;
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                ApprovesBadge();
                return View("ApproveArticles", ArticlesToApprove());
            }
            catch (Exception)
            {
                ViewBag.error = "Error al aprobar articulo";
                return View("ErrorPage");
            }

        }

        public void ApprovesBadge()
        {
            Article article = new Article();

            Session["rejects"] = article.RefreshRejects(User.Identity.Name);
            Session["approve"] = article.RefreshApproves();

        }

        public List<ArticleViewModel> ArticlesToApprove()
        {
            var rejects = from r in db.Rejects
                          select r.Article;
            var rejectsList = rejects.ToList();
            var articlesEF = from a in db.Articles
                             where !(a.State) 
                             select a;

            var articles = articlesEF.ToList();

            foreach (var a in rejects)
            {
                if (articles.Contains(a))
                {
                    articles.Remove(a);
                }
            }


            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);
            return ArticleViewModelList;

        }

        //-----------------------------------------------------------------------------------------//

        //---------------FilterSection---------------//
        public ActionResult GetfilterArticlesPartialView(string type, string param)
        {
            try
            {
                List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();

                switch (type)
                {
                    case "Ubicacion":
                        ArticleViewModelList = FilterUbication(param);
                        break;

                    case "Estado":
                        ArticleViewModelList = FilterState(param);
                        break;

                    case "Asesor":
                        ArticleViewModelList = FilterContributor(param);
                        break;

                    case "Categoria":
                        ArticleViewModelList = FilterCategory(param);
                        break;
                    case "Disponibilidad":
                        ArticleViewModelList = Filteravailability(param);
                        break;
                    default:
                        var articles = db.Articles.ToList();
                        ArticleViewModelList = GetArticleViewModelList(articles);
                        break;
                }

                ViewBag.ArticleKindId = EnumHelper.GetSelectList(typeof(EArticleKind));
                return PartialView("articleList", ArticleViewModelList);

            }
            catch (Exception)
            {

                ViewBag.error = "Error al filtrar articulos";
                return View("ErrorPage");
            }
        }

        public List<ArticleViewModel> FilterUbication(string ubication)
        {
            try
            {
                var articlesEF = from a in db.Articles
                                 where a.Ubication.Name == ubication
                                 select a;

                var articles = articlesEF.ToList();
                List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

                return ArticleViewModelList;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public List<ArticleViewModel> Filteravailability(string availability)
        {
            try
            {
                var availabilityBool = availability == "Vendida" ? true : false;
                var articlesEF = from a in db.Articles
                                 where a.SoldState == availabilityBool
                                 select a;
                var articles = articlesEF.ToList();

                List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

                return ArticleViewModelList;
            }
            catch
            {
                return null;
            }
        }

        public List<ArticleViewModel> FilterState(string state)
        {
            try
            {
                var stateBool = state == "Pendiente" ? false : true;
                var articlesEF = from a in db.Articles
                                 where a.State == stateBool
                                 select a;
                var articles = articlesEF.ToList();

                List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

                return ArticleViewModelList;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public List<ArticleViewModel> FilterContributor(string IC)
        {
            try
            {
                var articlesEF = from a in db.Articles
                                 where a.IndividualContributor.Name == IC
                                 select a;

                var articles = articlesEF.ToList();


                List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

                return ArticleViewModelList;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public List<ArticleViewModel> FilterCategory(string category)
        {
            try
            {
                var articlesEF = from a in db.Articles
                                 where a.Ubication.UbicationCategory.Name == category
                                 select a;

                var articles = articlesEF.ToList();


                List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

                return ArticleViewModelList;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public List<Feature> FeatureFilter(List<Feature> featuresList, int[] featuresSelected)
        {
            List<Feature> filterFeatures = new List<Feature>();

            try
            {
                foreach (var feature in featuresList)
                {
                    if (Array.Exists(featuresSelected, f => f == feature.FeatureId))
                    {
                        filterFeatures.Add(feature);
                    }
                }

                return filterFeatures;
            }
            catch (Exception)
            {

                return null; ;
            }
        }

        [HttpPost]
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
                        terrainFeature.Model = "Article_Terrain";
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
                        houseFeature.Model = "Article_House";
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
                        houseAuxFeature.Model = "Article_HouseAux";
                        features.Add(houseAuxFeature);
                    }
                    break;

            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(features);
            return json;
        }

        public string GetParamsList(string type)
        {

            List<string> objectList = new List<string>();

            switch (type)
            {
                case "Ubicacion":
                    var ubications = from u in db.Ubications
                                     select u.Name;
                    objectList = ubications.ToList();
                    break;
                case "Estado":
                    var state = new List<string>();
                    state.Add("Aprobada");
                    state.Add("Pendiente");
                    objectList = state;
                    break;
                case "Asesor":
                    var contributor = from c in db.IndividualContributors
                                      select c.Name;
                    objectList = contributor.ToList();
                    break;
                case "Categoria":
                    var ubicationCategory = from u in db.UbicationCategory
                                            select u.Name;
                    objectList = ubicationCategory.ToList();
                    break;
                case "Disponibilidad":
                    var stateSale = new List<string>();
                    stateSale.Add("En venta");
                    stateSale.Add("Vendida");
                    objectList = stateSale;
                    break;
            }
            var content = "<option selected disabled>Seleccione una opcion</option>";
            foreach (var obj in objectList)
            {
                content += "<option value='" + obj + "'>" + obj + "</option>";
            }

            return content;
        }


        //-----------------------------------------------------------------------------------------//

        //---------------TerrainSection---------------//
        public List<TerrainFeatureTerrain> GetTerrainFeatures(Terrain terrain)
        {
            try
            {
                IQueryable<TerrainFeatureTerrain> TerrainFeatureTerrainEF = from tf in db.TerrainFeaturesTerrain
                                                                            where tf.TerrainId == terrain.TerrainId
                                                                            select tf;

                List<TerrainFeatureTerrain> TerrainFeatureTerrainList = TerrainFeatureTerrainEF.ToList();


                return TerrainFeatureTerrainList;
            }
            catch (Exception ex)
            {
                ViewBag.error += "Error al obtener las caracteristicas de terreno" + ex.InnerException;
                return null;
            }

        }

        public void AddTerrainFeatures(int[] featuresId, int terrainId)
        {
            try
            {
                List<TerrainFeatureTerrain> Features = new List<TerrainFeatureTerrain>();
                foreach (var feature in featuresId)
                {
                    TerrainFeatureTerrain terrainFeature = new TerrainFeatureTerrain();
                    terrainFeature.TerrainFeatureId = feature;
                    terrainFeature.TerrainId = terrainId;
                    Features.Add(terrainFeature);
                }
                db.TerrainFeaturesTerrain.AddRange(Features);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                ViewBag.error = "Error al añadir caracteristicas de terreno" + ex.InnerException;
            }
        }

        public void EditTerrainFeatures(Terrain terrain, int[] terrainFeatures)
        {
            //---------------Features section---------------//
            try
            {
                IQueryable<TerrainFeatureTerrain> currentFeatures = from f in db.TerrainFeaturesTerrain
                                                                    where f.TerrainId == terrain.TerrainId
                                                                    select f;

                List<TerrainFeatureTerrain> currentFeaturesList = currentFeatures.ToList();

                foreach (var currentFeature in currentFeaturesList)
                {
                    var NoExists = true;

                    foreach (var feature in terrainFeatures)
                    {
                        if (currentFeature.TerrainFeatureId == feature)
                        {
                            NoExists = false;
                        }
                    }

                    if (NoExists)
                    {
                        db.TerrainFeaturesTerrain.Remove(currentFeature);
                    }
                }


                foreach (var feature in terrainFeatures)
                {
                    var Exists = false;

                    foreach (var currentFeature in currentFeaturesList)
                    {
                        if (currentFeature.TerrainFeatureId == feature)
                        {
                            Exists = true;
                        }
                    }

                    if (!Exists)
                    {
                        CreateTerrainFeature(feature, terrain.TerrainId);
                    }
                }
            }
            catch (Exception)
            {

                ViewBag.error = "Error al editar terreno";

            }
        }

        public void CreateTerrainFeature(int terrainFeature, int terrainId)
        {
            try
            {
                TerrainFeatureTerrain terrainFeatureTerrain = new TerrainFeatureTerrain();
                terrainFeatureTerrain.TerrainFeatureId = terrainFeature;
                terrainFeatureTerrain.TerrainId = terrainId;
                db.TerrainFeaturesTerrain.Add(terrainFeatureTerrain);
            }
            catch (Exception)
            {

                ViewBag.error = "Error al crear caracteristicas de terreno";
            }
        }


        //-----------------------------------------------------------------------------------------//

        //---------------HouseSection---------------//
        public void AddHouse(House house, int[] features, int articleId)
        {
            try
            {
                house.ArticleId = articleId;
                db.Houses.Add(house);
                db.SaveChanges();
                AddHouseFeatures(features, house.HouseId);
            }
            catch (Exception ex)
            {

                ViewBag.error = "Error al añadir casa" + ex.InnerException;
            }


        }

        public void AddHouseFeatures(int[] features, int houseId)
        {
            try
            {
                HouseFeatureHouse houseFeatureHouse = new HouseFeatureHouse();
                foreach (var feature in features)
                {
                    houseFeatureHouse = new HouseFeatureHouse();
                    houseFeatureHouse.HouseFeatureId = feature;
                    houseFeatureHouse.HouseId = houseId;
                    db.HouseFeatureHouse.Add(houseFeatureHouse);
                }
                db.SaveChanges();

            }
            catch (Exception ex)
            {

                ViewBag.error = "Error al añadir caracteristicas de casa" + ex.InnerException;

            }


        }

        public ArticleViewModel GetHouses(int id)
        {
            ArticleViewModel articleViewModel = new ArticleViewModel();

            try
            {
                IQueryable<House> HouseListEF = from h in db.Houses
                                                where h.ArticleId == id
                                                select h;
                List<House> HouseList = HouseListEF.ToList();

                House house = null;
                HouseAux houseAux = null;

                foreach (var houseResult in HouseList)
                {
                    if (house == null)
                    {
                        house = houseResult;
                        house.HouseFeaturesHouse = GetHouseFeatures(house);

                    }
                    else
                    {
                        houseAux = new HouseAux();
                        houseAux.Id = houseResult.HouseId;
                        houseAux.BathroomsAux = houseResult.Bathrooms;
                        houseAux.BedroomsAux = houseResult.Bedrooms;
                        houseAux.GarageAux = houseResult.Garage;
                        houseAux.HouseBackgroundMeasureAux = houseResult.HouseBackgroundMeasure;
                        houseAux.HouseForeheadMeasureAux = houseResult.HouseForeheadMeasure;
                        houseAux.LevelsAux = houseResult.Levels;
                        houseAux.HouseFeaturesHouse = GetHouseAuxFeatures(houseAux);

                    }
                }

                articleViewModel.House = house;
                articleViewModel.HouseAux = houseAux;

            }
            catch (Exception)
            {

                ViewBag.error += "Error al obtener casas";
            }

            return articleViewModel;

        }

        public List<HouseFeatureHouse> GetHouseFeatures(House house)
        {
            try
            {
                IQueryable<HouseFeatureHouse> HouseFeatureHouseEF = from hf in db.HouseFeatureHouse
                                                                    where hf.HouseId == house.HouseId
                                                                    select hf;
                List<HouseFeatureHouse> houseFeatureHouseList = HouseFeatureHouseEF.ToList();


                return houseFeatureHouseList;
            }
            catch (Exception ex)
            {

                ViewBag.error += "Error al obtener las caracteristicas de terreno" + ex.InnerException;
                return null;
            }

        }

        public List<HouseFeatureHouse> GetHouseAuxFeatures(HouseAux house)
        {
            try
            {
                IQueryable<HouseFeatureHouse> HouseFeatureHouseEF = from hf in db.HouseFeatureHouse
                                                                    where hf.HouseId == house.Id
                                                                    select hf;
                List<HouseFeatureHouse> houseFeatureHouseList = HouseFeatureHouseEF.ToList();


                return houseFeatureHouseList;
            }
            catch (Exception ex)
            {

                ViewBag.error += "Error al obtener las caracteristicas de terreno" + ex.InnerException;
                return null;
            }

        }

        public void EditFeatures(House house, int[] houseFeatures)
        {
            //---------------Features section---------------//

            try
            {
                IQueryable<HouseFeatureHouse> currentFeatures = from f in db.HouseFeatureHouse
                                                                where f.HouseId == house.HouseId
                                                                select f;

                List<HouseFeatureHouse> currentFeaturesList = currentFeatures.ToList();

                foreach (var currentFeature in currentFeaturesList)
                {
                    var NoExists = true;

                    foreach (var feature in houseFeatures)
                    {
                        if (currentFeature.HouseFeatureId == feature)
                        {
                            NoExists = false;
                        }
                    }

                    if (NoExists)
                    {
                        db.HouseFeatureHouse.Remove(currentFeature);
                    }
                }

                foreach (var feature in houseFeatures)
                {
                    var Exists = false;

                    foreach (var currentFeature in currentFeaturesList)
                    {
                        if (currentFeature.HouseFeatureId == feature)
                        {
                            Exists = true;
                        }
                    }

                    if (!Exists)
                    {
                        CreateFeature(feature, house.HouseId);
                    }
                }
            }
            catch (Exception)
            {

                ViewBag.error = "Error al editar caracteristicas";
            }

        }

        public void CompareHouses(List<House> houseList, int id, House frstHouse, House scndHouse, int[] frstFeatures, int[] scndFeatures)
        {
            try
            {

                IQueryable<House> PreviewHouses = from h in db.Houses
                                                  where h.ArticleId == id
                                                  select h;

                List<House> housePreviewList = PreviewHouses.ToList();

                if (housePreviewList.Count() > 0)
                {
                    foreach (House Previewhouse in housePreviewList)
                    {
                        var result = houseList.Exists(x => x.HouseId == Previewhouse.HouseId);
                        if (!result)
                        {
                            DeleteHouse(Previewhouse);
                        }

                    }
                }

                if (frstHouse != null)
                {
                    if (!(housePreviewList.Exists(x => x.HouseId == frstHouse.HouseId)))
                    {
                        House house = CreateHouse(frstHouse);
                        foreach (var feature in frstFeatures)
                        {
                            CreateFeature(feature, house.HouseId);
                        }
                    }
                    else
                    {
                        EditHouse(frstHouse);
                        EditFeatures(frstHouse, frstFeatures);
                    }
                }

                if (scndHouse != null)
                {
                    if (!(housePreviewList.Contains(scndHouse)))
                    {
                        House house = CreateHouse(scndHouse);
                        foreach (var feature in scndFeatures)
                        {
                            CreateFeature(feature, house.HouseId);
                        }
                    }
                    else
                    {
                        EditHouse(scndHouse);
                        EditFeatures(scndHouse, scndFeatures);
                    }
                }
            }
            catch (Exception)
            {

                ViewBag.error = "Error al actualizar casas";
            }

        }

        public void CreateFeature(int houseFeature, int houseId)
        {
            try
            {

                HouseFeatureHouse houseFeaturehouse = new HouseFeatureHouse();
                houseFeaturehouse.HouseFeatureId = houseFeature;
                houseFeaturehouse.HouseId = houseId;
                db.HouseFeatureHouse.Add(houseFeaturehouse);
            }
            catch (Exception)
            {

                ViewBag.error = "Error al agregar caracteristica de casa";
            }
        }

        public void EditHouse(House house)
        {
            try
            {
                House EntityHouse = db.Houses.Find(house.HouseId);
                EntityHouse.Bathrooms = house.Bathrooms;
                EntityHouse.Bedrooms = house.Bedrooms;
                EntityHouse.Garage = house.Garage;
                EntityHouse.HouseBackgroundMeasure = house.HouseBackgroundMeasure;
                EntityHouse.HouseForeheadMeasure = house.HouseForeheadMeasure;
                EntityHouse.Levels = house.Levels;

                db.Entry(EntityHouse).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {

                ViewBag.error = "Error al editar casa";
            }
        }

        public void DeleteHouse(House house)
        {
            try
            {
                db.Houses.Remove(house);
                db.SaveChanges();

            }
            catch (Exception)
            {

                ViewBag.error = "Error al borrar casa";
            }
        }

        public House CreateHouse(House house)
        {
            try
            {
                db.Houses.Add(house);
                db.SaveChanges();
                return house;

            }
            catch (Exception)
            {

                ViewBag.error = "Error al agregar casa";
                return null;
            }
        }

        //-----------------------------------------------------------------------------------------//

        //---------------PicturesSection---------------//
        public ArticlePicture GetOutstandingPicture(int id)
        {
            try
            {
                ArticlePicture OutstandingPicture = new ArticlePicture();

                OutstandingPicture = db.ArticlePictures.FirstOrDefault(a => a.ArticleId == id && a.OutstandingPicture == true);

                return OutstandingPicture;
            }
            catch (Exception ex)
            {

                ViewBag.error += "Error al obtener la foto de portada" + ex.InnerException;
                return null;
            }

        }

        public List<ArticlePicture> GetOutstandingPictureList()
        {
            try
            {
                List<ArticlePicture> OutstandingPictureList = new List<ArticlePicture>();
                IQueryable<ArticlePicture> OutstandingPicturesEF = from p in db.ArticlePictures
                                                                   where p.OutstandingPicture == true
                                                                   select p;
                OutstandingPictureList = OutstandingPicturesEF.ToList();
                return OutstandingPictureList;
            }
            catch (Exception ex)
            {

                ViewBag.error += "Error al obtener las fotos de portada" + ex.InnerException;
                return null;
            }

        }

        public List<ArticlePicture> GetPictureList(int id)
        {
            try
            {
                IQueryable<ArticlePicture> PicturesEF = from p in db.ArticlePictures
                                                        where p.ArticleId == id && p.OutstandingPicture == false
                                                        select p;

                List<ArticlePicture> PicturesList = PicturesEF.ToList();

                return PicturesList;
            }
            catch (Exception ex)
            {
                ViewBag.error += "Error al obtener las fotos" + ex.InnerException;
                return null;
            }

        }

        public string AddBlobToStorage(int articleId, string b64String, int iterator)
        {
            var fecha = DateTime.Now.ToString("hhmmss");
            string name = "Foto" + fecha + articleId.ToString() + iterator.ToString();
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

        private void SetOutstandingPicture(ArticlePicture currentOutstandingPicture, List<ArticlePicture> currentPicturesList, string outstandingPicture, int id)
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
                ArticlePicture outstandingPictureExists = null;
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
                    ArticlePicture picture = new ArticlePicture();
                    picture.OutstandingPicture = true;
                    picture.Extension = AddBlobToStorage(id, outstandingPicture, 20);
                    picture.ArticleId = id;
                    db.ArticlePictures.Add(picture);
                    db.SaveChanges();
                }
            }
        }

        public string DeletePicture(int id)
        {
            var state = "false";
            try
            {
                ArticlePicture articlePicture = db.ArticlePictures.Find(id);
                if (articlePicture != null)
                {
                    db.ArticlePictures.Remove(articlePicture);
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

        public void UpdatePictures(int ArticleId, string[] urls, string OutstandingPicture)
        {
            ArticlePicture picture = new ArticlePicture();
            List<ArticlePicture> Pictures = new List<ArticlePicture>();
            IQueryable<ArticlePicture> currentPicturesEF = from p in db.ArticlePictures
                                                           where p.ArticleId == ArticleId
                                                           select p;
            List<ArticlePicture> currentPicturesList = currentPicturesEF.ToList();
            ArticlePicture currentOutstandingPicture = currentPicturesList.FirstOrDefault(p => p.OutstandingPicture == true);

            Article article = db.Articles.Find(ArticleId);
            //ArticlePicture articlePicture = db.ArticlePictures.FirstOrDefault(p => p.Extension == OutstandingPicture);

            SetOutstandingPicture(currentOutstandingPicture, currentPicturesList, OutstandingPicture, ArticleId);


            var i = 0;
            foreach (var url in urls)
            {                
                if (!currentPicturesList.Exists(x => x.Extension == url))
                {
                    picture = new ArticlePicture();
                    picture.OutstandingPicture = false;
                    picture.Extension = AddBlobToStorage(article.Id, url, i);
                    picture.ArticleId = article.Id;
                    Pictures.Add(picture);
                }
                i++;
            }
            db.ArticlePictures.AddRange(Pictures);
            db.SaveChanges();
        }

        //-----------------------------------------------------------------------------------------//

        //---------------ArticleViewModelSection---------------//
        public ArticleViewModel GetArticleViewModel(Article article)
        {
            try
            {
                List<ArticlePicture> pictures = GetPictureList(article.Id);
                ArticlePicture outstandingPicture = GetOutstandingPicture(article.Id);
                var houses = GetHouses(article.Id);
                article.Terrain.TerrainFeaturesTerrain = GetTerrainFeatures(article.Terrain);

                ArticleViewModel articleViewModel = SetArticleViewModel(pictures, outstandingPicture, houses.House, houses.HouseAux, article);

                return articleViewModel;
            }
            catch (Exception ex)
            {

                ViewBag.error += "Error al obtener el modelo de articulo" + ex.InnerException;
                return null;
            }

        }

        public List<ArticleViewModel> GetArticleViewModelList(List<Article> articles)
        {

            List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();
            foreach (var article in articles)
            {
                ArticleViewModel articleViewModel = GetArticleViewModel(article);
                ArticleViewModelList.Add(articleViewModel);
            }
            return ArticleViewModelList;

        }

        public ArticleViewModel SetArticleViewModel(List<ArticlePicture> pictures, ArticlePicture outstandingPicture, House house, HouseAux houseAux, Article article)
        {
            try
            {
                ArticleViewModel articleViewModel = new ArticleViewModel();
                articleViewModel.Article = article;
                articleViewModel.Pictures = pictures;
                articleViewModel.OutstandingPicture = outstandingPicture;
                articleViewModel.House = house;
                articleViewModel.HouseAux = houseAux;

                return articleViewModel;
            }
            catch (Exception ex)
            {

                ViewBag.error += "Error al setear el modelo de articulo" + ex.InnerException;
                return null;
            }

        }

        public List<ArticleViewModel> GetArticleViewModelList()
        {
            try
            {
                List<Article> articles = db.Articles.ToList();
                List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();

                foreach (var article in articles)
                {
                    ArticleViewModel articleViewModel = GetArticleViewModel(article);
                    ArticleViewModelList.Add(articleViewModel);
                }

                return ArticleViewModelList;
            }
            catch (Exception)
            {

                return null;
            }

        }

        //-----------------------------------------------------------------------------------------//


        //---------------FileSection---------------//
        [HttpPost]
        public void SubirArchivo(HttpPostedFileBase file, int id)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Extraemos el contenido en Bytes del archivo subido.
                var _contenido = new byte[file.ContentLength];
                file.InputStream.Read(_contenido, 0, file.ContentLength);

                // Separamos el Nombre del archivo de la Extensión.
                int indiceDelUltimoPunto = file.FileName.LastIndexOf('.');
                string _nombre = file.FileName.Substring(0, indiceDelUltimoPunto);
                string _extension = file.FileName.Substring(indiceDelUltimoPunto + 1,
                                    file.FileName.Length - indiceDelUltimoPunto - 1);

                // Instanciamos la clase Archivo y asignammos los valores.
                Archivo _archivo = new Archivo()
                {
                    Nombre = _nombre,
                    Extension = _extension,
                    ArticleId = id
                };

                try
                {
                    // Subimos el archivo al Servidor.
                    _archivo.SubirArchivo(_contenido);
                    // Guardamos en la base de datos la instancia del archivo
                    db.Archivos.Add(_archivo);
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    // Aquí el código para manejar la Excepción.
                }
            }

            // Redirigimos a la Acción 'Index' para mostrar
            // Los archivos subidos al Servidor.
        }

        [HttpPost]
        public void EliminarArchivo(Guid id)
        {
            Archivo _archivo;


            _archivo = db.Archivos.FirstOrDefault(x => x.Id == id);


            if (_archivo != null)
            {

                _archivo = db.Archivos.FirstOrDefault(x => x.Id == id);
                db.Archivos.Remove(_archivo);
                if (db.SaveChanges() > 0)
                {
                    // Eliminamos el archivo del Servidor.
                    _archivo.EliminarArchivo();
                }

                // Redirigimos a la Acción 'Index' para mostrar
                // Los archivos subidos al Servidor.
            }

        }

        //-----------------------------------------------------------------------------------------//


        public void ReloadViewBags(ArticleViewModel model)
        {
            ViewBag.houseForm = model.House != null ? "flex" : "none";
            ViewBag.houseAuxForm = model.HouseAux != null ? "flex" : "none";
            ViewBag.houseFormBtn = model.House != null && model.HouseAux != null ? "none" : "block";
            ViewBag.SelectedUbication = model.Article.UbicationId;
            ViewBag.Selectedurl = model.OutstandingPicture;
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name");
            ViewBag.TerrainId = new SelectList(db.Terrains, "TerrainId", "Topography");
            ViewBag.UbicationId = new SelectList(db.Ubications, "UbicationId", "Name");
        }

        public void ViewbagFeatures(int[] terrainSelected, int[] houseSelected, int[] houseAuxSelected)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var terrainFeatures = jsonSerialiser.Deserialize<List<Feature>>(GetFeatures("Terrain"));
            var houseFeatures = jsonSerialiser.Deserialize<List<Feature>>(GetFeatures("House"));
            var houseAuxFeatures = jsonSerialiser.Deserialize<List<Feature>>(GetFeatures("HouseAux"));

            ViewBag.terrainFeaturesSelected = FeatureFilter(terrainFeatures, terrainSelected);
            ViewBag.houseFeaturesSelected = houseSelected != null && houseSelected.Length > 0 ? FeatureFilter(houseFeatures, houseSelected) : null;
            ViewBag.houseAuxFeaturesSelected = houseAuxSelected != null && houseAuxSelected.Length > 0 ? FeatureFilter(houseAuxFeatures, houseAuxSelected) : null;
        }

        public ActionResult ErrorPage(string message)
        {
            ViewBag.error = message;
            return View();
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
