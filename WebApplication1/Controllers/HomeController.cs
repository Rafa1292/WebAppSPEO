using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.ViewModel;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        public ActionResult Index()
        {
            var articlesEF = from a in db.Articles
                             where a.State && !a.SoldState
                             select a;

            var articles = articlesEF.ToList();


            List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();
            foreach (var article in articles)
            {
                ArticleViewModel articleViewModel = GetArticleViewModel(article);
                ArticleViewModelList.Add(articleViewModel);
            }

            ViewBag.UbicationPicture = db.UbicationPictures.ToList();
            ViewBag.Ubications = db.Ubications.ToList();

            var ListOutstandingEF = from a in ArticleViewModelList
                                    where a.Article.ArticleKind == EArticleKind.Sobresaliente
                                    select a;

            var ListOportunityEF = from a in ArticleViewModelList
                                    where a.Article.ArticleKind == EArticleKind.Oportunidad
                                    select a;

            var ArticleViewModelListOrder = ArticleViewModelList.OrderBy(a => a.Article.CreationDate).ToList();

            List<ArticleViewModel> ArticleViewModelMixedList = new List<ArticleViewModel>();

            var i = 0;
            foreach (var article in ArticleViewModelListOrder)
            {
                if (i <= 8)
                {
                    ArticleViewModelMixedList.Add(article);
                }
            }

            ArticleViewModelMixedList.AddRange(ListOutstandingEF.ToList());
            ArticleViewModelMixedList.AddRange(ListOportunityEF.ToList());

            return View(ArticleViewModelMixedList);
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
    }
}