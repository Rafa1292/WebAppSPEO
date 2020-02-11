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
using System.Web.Mvc.Html;
using System.Web.Helpers;
using System.Dynamic;
using Newtonsoft.Json;
using System.Web.Security;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticlesController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: Articles
        public ActionResult Index()
        {
            var articles = db.Articles.ToList();


            List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();
            foreach (var article in articles)
            {
                ArticleViewModel articleViewModel = GetArticleViewModel(article);
                ArticleViewModelList.Add(articleViewModel);
            }
            ViewBag.ArticleKindId = EnumHelper.GetSelectList(typeof(EArticleKind));

            return View(ArticleViewModelList);
        }

        public string clasify(int articleId, int kindValue)
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

        public int GetEnumValue(Article article)
        {
            EArticleKind eArticleKind = article.ArticleKind;
            var id = 0;


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

        public List<ArticleViewModel> ArticlesToApprove()
        {
            var articlesEF = from a in db.Articles
                             where !(a.State)
                             select a;
            var articles = articlesEF.ToList();

            List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();
            foreach (var article in articles)
            {
                ArticleViewModel articleViewModel = GetArticleViewModel(article);
                ArticleViewModelList.Add(articleViewModel);
            }
            return ArticleViewModelList;

        }

        public ActionResult ApproveArticles()
        {
            List<ArticleViewModel> ArticleViewModelList = ArticlesToApprove();


            return View(ArticleViewModelList);
        }


        public ActionResult PropertySold(int id, bool state)
        {
            Article article = db.Articles.Find(id);
            article.SoldState = !state;
            db.Entry(article).State = EntityState.Modified;
            db.SaveChanges();

            var articles = db.Articles.ToList();


            List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();
            foreach (var item in articles)
            {
                ArticleViewModel articleViewModel = GetArticleViewModel(item);
                ArticleViewModelList.Add(articleViewModel);
            }
            ViewBag.ArticleKindId = EnumHelper.GetSelectList(typeof(EArticleKind));

            return PartialView("articleList", ArticleViewModelList);
        }

        public ActionResult ApproveArticle(int id)
        {
            Article article = db.Articles.Find(id);
            article.State = true;
            db.Entry(article).State = EntityState.Modified;
            db.SaveChanges();
            return View("ApproveArticles", ArticlesToApprove());
        }

        public ActionResult GetArticles(string type, string param)
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

        public List<ArticleViewModel> FilterUbication(string ubication)
        {

            var articlesEF = from a in db.Articles
                             where a.Ubication.Name == ubication
                             select a;
            var articles = articlesEF.ToList();


            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

            return ArticleViewModelList;
        }

        public List<ArticleViewModel> Filteravailability(string availability)
        {
            var availabilityBool = availability == "Vendida" ? true : false;
            var articlesEF = from a in db.Articles
                             where a.SoldState == availabilityBool
                             select a;
            var articles = articlesEF.ToList();

            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

            return ArticleViewModelList;
        }

        public List<ArticleViewModel> FilterState(string state)
        {
            var stateBool = state == "Pendiente" ? false : true;
            var articlesEF = from a in db.Articles
                             where a.State == stateBool
                             select a;
            var articles = articlesEF.ToList();

            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

            return ArticleViewModelList;
        }

        public List<ArticleViewModel> FilterContributor(string IC)
        {
            var articlesEF = from a in db.Articles
                             where a.IndividualContributor.Name == IC
                             select a;

            var articles = articlesEF.ToList();


            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

            return ArticleViewModelList;
        }

        public List<ArticleViewModel> FilterCategory(string category)
        {
            var articlesEF = from a in db.Articles
                             where a.Ubication.UbicationCategory.Name == category
                             select a;

            var articles = articlesEF.ToList();


            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

            return ArticleViewModelList;
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            ViewBag.houseForm = "none";
            ViewBag.houseAuxForm = "none";
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
        public ActionResult Create(ArticleViewModel articleViewModel)
        {
            HttpFileCollectionBase files = Request.Files;


            if (articleViewModel.Article != null && articleViewModel.Terrain != null && articleViewModel.Urls.Length > 0
                && articleViewModel.TerrainFeatures.Length > 0 && articleViewModel.OutstandingPicture != null)
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


                        IndividualContributor individualContributor = db.IndividualContributors.FirstOrDefault(i => i.Mail == User.Identity.Name);
                        article.Currency = articleViewModel.Currency;
                        article.Description = articleViewModel.Description;
                        article.IndividualContributorId = individualContributor.IndividualContributorId;
                        article.UbicationId = articleViewModel.UbicationId;
                        article.Code = "A" + terrain.TerrainId;
                        article.State = false;
                        article.SoldState = false;
                        article.TerrainId = terrain.TerrainId;
                        article.CreationDate = DateTime.Now;
                        article.ArticleKind = EArticleKind.Venta;
                        db.Articles.Add(article);

                        db.SaveChanges();
                        AddTerrainFeatures(terrainFeatures, terrain.TerrainId, article.Id);


                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            if (file != null)
                            {
                                SubirArchivo(file, article.Id);
                            }
                        }

                        List<ArticlePicture> Pictures = new List<ArticlePicture>();
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

                        db.ArticlePictures.AddRange(Pictures);
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
                        ReloadViewBags(articleViewModel);
                        ViewBag.error = ex.InnerException;
                        return View(articleViewModel);
                    }
                }
            }
            ReloadViewBags(articleViewModel);
            return View(articleViewModel);
        }

        public void ReloadViewBags(ArticleViewModel model)
        {
            ViewBag.houseForm = model.House != null ? "flex" : "none";
            ViewBag.houseAuxForm = model.HouseAux != null ? "flex" : "none";
            ViewBag.houseFormBtn = model.House != null && model.HouseAux != null ? "none" : "block";
            ViewbagFeatures(model.TerrainFeatures, model.HouseFeatures, model.HouseAuxFeatures);
            ViewBag.Currency = model.Currency;
            ViewBag.SelectedUbication = model.UbicationId;
            ViewBag.SelectedColaborator = model.IndividualContributorId;
            ViewBag.Selectedurl = model.OutstandingPicture;
            ViewBag.urls = model.Urls;
            ViewBag.IndividualContributorId = new SelectList(db.IndividualContributors, "IndividualContributorId", "Name");
            ViewBag.TerrainId = new SelectList(db.Terrains, "TerrainId", "Topography");
            ViewBag.UbicationId = new SelectList(db.Ubications, "UbicationId", "Name");
        }

        public void AddHouse(House house, int[] features, int articleId)
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

        public void AddTerrainFeatures(int[] features, int terrainId, int articleId)
        {
            List<TerrainFeatureTerrain> Features = new List<TerrainFeatureTerrain>();
            foreach (var feature in features)
            {
                TerrainFeatureTerrain terrainFeature = new TerrainFeatureTerrain();
                terrainFeature.TerrainFeatureId = feature;
                terrainFeature.TerrainId = terrainId;
                Features.Add(terrainFeature);
            }
            db.TerrainFeaturesTerrain.AddRange(Features);

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

            var filesEF = from f in db.Archivos
                          where f.ArticleId == id
                          select f;
            ViewBag.Files = filesEF.ToList();
            ArticleViewModel articleViewModel = GetArticleViewModel(article);
            ReloadViewBags(articleViewModel);
            return View(articleViewModel);
        }

        public ArticleViewModel GetArticleViewModel(Article article)
        {
            var pictures = GetPictures(article.Id);
            string[] urls = pictures.Urls;
            string outstandingPicture = pictures.OutstandingPicture;
            var houses = GetHouses(article.Id);
            House house = houses.House != null ? houses.House : null;
            HouseAux houseAux = houses.HouseAux != null ? houses.HouseAux : null;
            ArticleViewModel Features = GetFeatures(article, house, houseAux);
            int[] terrainFeatures = Features.TerrainFeatures;
            int[] houseFeatures = Features.HouseFeatures;
            int[] houseAuxFeatures = Features.HouseAuxFeatures;
            ArticleViewModel articleViewModel = SetArticleViewModel(article, urls, outstandingPicture, house, houseAux, houseFeatures, houseAuxFeatures, terrainFeatures);

            return articleViewModel;
        }

        public ArticleViewModel GetFeatures(Article article, House house, HouseAux houseAux)
        {
            ArticleViewModel articleViewModel = new ArticleViewModel();

            var terrainFeaturesid = from f in db.TerrainFeaturesTerrain
                                    where f.TerrainId == article.TerrainId
                                    select f.TerrainFeatureId;
            ICollection<int> terrainFeatures = new List<int>();
            foreach (var feature in terrainFeaturesid)
            {
                terrainFeatures.Add(feature);
            }

            articleViewModel.TerrainFeatures = terrainFeatures.ToArray();


            var houseFeaturesid = from h in db.HouseFeatureHouse
                                  where h.HouseId == house.HouseId
                                  select h.HouseFeatureId;

            if (house != null)
            {
                ICollection<int> houseFeatures = new List<int>();
                foreach (var feature in houseFeaturesid)
                {
                    houseFeatures.Add(feature);
                }
                articleViewModel.HouseFeatures = houseFeatures.ToArray();
            }

            var houseAuxFeaturesid = from h in db.HouseFeatureHouse
                                     where h.HouseId == houseAux.Id
                                     select h.HouseFeatureId;

            if (houseAux != null)
            {
                ICollection<int> houseAuxFeatures = new List<int>();
                foreach (var feature in houseAuxFeaturesid)
                {
                    houseAuxFeatures.Add(feature);
                }
                articleViewModel.HouseAuxFeatures = houseAuxFeatures.ToArray();

            }



            return articleViewModel;
        }

        public ArticleViewModel GetHouses(int id)
        {
            ArticleViewModel articleViewModel = new ArticleViewModel();

            IQueryable<House> HouseList = from h in db.Houses
                                          where h.ArticleId == id
                                          select h;

            House house = null;
            HouseAux houseAux = null;

            foreach (var houseResult in HouseList)
            {
                if (house == null)
                {
                    house = houseResult;
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

                }
            }

            articleViewModel.House = house;
            articleViewModel.HouseAux = houseAux;

            return articleViewModel;
        }

        public ArticleViewModel GetPictures(int id)
        {
            ArticleViewModel articleViewModel = new ArticleViewModel();
            List<ArticlePicture> Pictures = db.ArticlePictures.ToList();
            IEnumerable<string> urls = from p in Pictures
                                       where p.ArticleId == id && p.OutstandingPicture == false
                                       select Convert.ToBase64String(p.PictureArray);

            var outstandingPictureModel = Pictures.Find(x => x.OutstandingPicture == true && x.ArticleId == id);
            string outstandingPicture = Convert.ToBase64String(outstandingPictureModel.PictureArray);


            articleViewModel.Urls = urls.ToArray();
            articleViewModel.OutstandingPicture = outstandingPicture;

            return articleViewModel;
        }

        public ArticleViewModel SetArticleViewModel(
            Article article, string[] urls, string outstandingPicture, House house, HouseAux houseAux, int[] houseFeatures, int[] houseAuxFeatures, int[] terrainFeatures)
        {
            ArticleViewModel articleViewModel = new ArticleViewModel();
            articleViewModel.Article = article;
            articleViewModel.Currency = article.Currency;
            articleViewModel.Description = article.Description;
            articleViewModel.Terrain = article.Terrain;
            articleViewModel.UbicationId = article.UbicationId;
            articleViewModel.IndividualContributorId = article.IndividualContributorId;
            articleViewModel.Urls = urls;
            articleViewModel.OutstandingPicture = outstandingPicture;
            articleViewModel.House = house;
            articleViewModel.HouseAux = houseAux;
            articleViewModel.HouseFeatures = houseFeatures;
            articleViewModel.HouseAuxFeatures = houseAuxFeatures;
            articleViewModel.TerrainFeatures = terrainFeatures;
            return articleViewModel;
        }

        // POST: Articles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArticleViewModel articleViewModel)
        {
            Article article = db.Articles.Find(articleViewModel.Article.Id);

            HttpFileCollectionBase files = Request.Files;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (articleViewModel.Article != null)
                    {
                        article.OwnerName = articleViewModel.Article.OwnerName;
                        article.OwnerPhone = articleViewModel.Article.OwnerPhone;
                        article.Price = articleViewModel.Article.Price;
                        article.Currency = articleViewModel.Currency;
                        article.Description = articleViewModel.Description;
                        article.UbicationId = articleViewModel.UbicationId;
                        article.State = false;
                        db.Entry(article).State = EntityState.Modified;
                    }

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        SubirArchivo(file, article.Id);
                    }

                    if (articleViewModel.Terrain != null)
                    {
                        db.Entry(articleViewModel.Terrain).State = EntityState.Modified;
                    }

                    EditTerrainFeatures(articleViewModel.Terrain, articleViewModel.TerrainFeatures);

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

                    CompareHouses(HouseList, article.Id, articleViewModel.House, house, articleViewModel.HouseFeatures, articleViewModel.HouseAuxFeatures);
                    UpdatePictures(articleViewModel.Article.Id, articleViewModel.Urls, articleViewModel.OutstandingPicture);
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

        public void EditTerrainFeatures(Terrain terrain, int[] terrainFeatures)
        {
            //---------------Features section---------------//

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

        public void CompareHouses(List<House> houseList, int id, House frstHouse, House scndHouse, int[] frstFeatures, int[] scndFeatures)
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

        public void EditFeatures(House house, int[] houseFeatures)
        {
            //---------------Features section---------------//

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

        public void CreateFeature(int houseFeature, int houseId)
        {

            HouseFeatureHouse houseFeaturehouse = new HouseFeatureHouse();
            houseFeaturehouse.HouseFeatureId = houseFeature;
            houseFeaturehouse.HouseId = houseId;
            db.HouseFeatureHouse.Add(houseFeaturehouse);
        }

        public void CreateTerrainFeature(int terrainFeature, int terrainId)
        {

            TerrainFeatureTerrain terrainFeatureTerrain = new TerrainFeatureTerrain();
            terrainFeatureTerrain.TerrainFeatureId = terrainFeature;
            terrainFeatureTerrain.TerrainId = terrainId;
            db.TerrainFeaturesTerrain.Add(terrainFeatureTerrain);
        }

        public void EditHouse(House house)
        {
            House EntityHouse = db.Houses.Find(house.HouseId);
            EntityHouse.Bathrooms = house.Bathrooms;
            EntityHouse.Bedrooms = house.Bedrooms;
            EntityHouse.Garage = house.Garage;
            EntityHouse.HouseBackgroundMeasure = house.HouseBackgroundMeasure;
            EntityHouse.HouseForeheadMeasure = house.HouseForeheadMeasure;
            EntityHouse.Levels = house.Levels;

            db.Entry(EntityHouse).State = EntityState.Modified;
        }

        public void DeleteHouse(House house)
        {
            db.Houses.Remove(house);
        }

        public House CreateHouse(House house)
        {
            db.Houses.Add(house);

            return house;
        }

        public void UpdatePictures(int id, string[] urls, string outstandingPicture)
        {
            //---------------Pictures section---------------//
            IQueryable<ArticlePicture> currentPictures = from p in db.ArticlePictures
                                                         where p.ArticleId == id
                                                         select p;
            //Tenemos la lista de fotos actuales y la foto de portada actual en formato de byte.
            List<ArticlePicture> currentPicturesList = currentPictures.ToList();
            ArticlePicture currentOutstandingPicture = currentPicturesList.Find(p => p.OutstandingPicture == true);

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
                //Metodo que establece datos segun  nueva foto de portada. Mas info en el metodo.
                SetOutstandingPicture(currentOutstandingPicture, currentPicturesList, outstandingPicture, id);
            }

            //<-----------------Agregar nuevas fotos----------------->//
            /*Para agregar nuevas fotos vamos a separar de la lista nueva las fotos que existen en la lista antigua
             * una vez separadas agregamos las fotos que no existian.
            */
            AddNewPictures(urls, currentPicturesList, id);

            //<-----------------Eliminar fotos----------------->//
            /*Para eliminar  fotos vamos a comparar el contenido de la lista nueva contra las fotos en la lista antigua
             * si no existen en la  lista nueva se procede a la eliminacion
            */
            DeletePictures(urls, currentPicturesList, outstandingPicture);
            //---------------End pictures section---------------//
        }

        private void DeletePictures(string[] urls, List<ArticlePicture> currentPicturesList, string outstandingPicture)
        {
            //recorremos la lista antigua 
            foreach (var currentPicture in currentPicturesList)
            {
                //variable que nos indicara si el elemento existe
                var noExists = true;
                //recorremos lista nueva comparando el elemento en ciclo de la lista antigua contra los elementos
                //de la lista nueva.
                foreach (var url in urls)
                {
                    string currentPictureBase64 = Convert.ToBase64String(currentPicture.PictureArray);
                    if (currentPictureBase64 == url || currentPictureBase64 == outstandingPicture)
                    {
                        noExists = false;
                    }
                }
                //si el elemento no existe lo borramos
                if (noExists)
                {
                    db.ArticlePictures.Remove(currentPicture);
                }

            }
        }

        private void AddNewPictures(string[] urls, List<ArticlePicture> currentPicturesList, int id)
        {
            //Creamos lista que almacenara fotos nuevas para añadirlas a la base de datos
            List<ArticlePicture> picturesToAdd = new List<ArticlePicture>();
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
                    ArticlePicture picture = new ArticlePicture();
                    picture.OutstandingPicture = false;
                    picture.PictureArray = Convert.FromBase64String(url);
                    picture.ArticleId = id;
                    picturesToAdd.Add(picture);
                }
            }
            //Finalizado el ciclo de verificacion procedemos a añadir las nuevas imagenes
            db.ArticlePictures.AddRange(picturesToAdd);
        }

        private void SetOutstandingPicture(ArticlePicture currentOutstandingPicture, List<ArticlePicture> currentPicturesList, string outstandingPicture, int id)
        {
            //Si no son iguales primero cambiamos a false la opcion outstandingPicture de la foto actual, ya que esta no es mas la foto de portada
            currentOutstandingPicture.OutstandingPicture = false;
            db.Entry(currentOutstandingPicture).State = EntityState.Modified;
            //Luego verificamos si la nueva foto de portada existe entre las fotos antiguas
            //Para ello creamos una variable que nos almacenara la imagen completa si existe.
            ArticlePicture outstandingPictureExists = null;
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
                ArticlePicture picture = new ArticlePicture();
                picture.OutstandingPicture = true;
                picture.PictureArray = Convert.FromBase64String(outstandingPicture);
                picture.ArticleId = id;
                db.ArticlePictures.Add(picture);
            }
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
            ArticleViewModel articleViewModel = GetArticleViewModel(article);

            return View(articleViewModel);
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

            ViewBag.terrainFeaturesSelected = FeatureFilter(terrainFeatures, terrainSelected);
            ViewBag.houseFeaturesSelected = houseSelected != null && houseSelected.Length > 0 ? FeatureFilter(houseFeatures, houseSelected) : null;
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
