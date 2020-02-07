using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.ViewModel;
using WebApplication1.Models;
using System.Net.Mail;
using System.Text;
using System.Net.Mime;
using System.IO;
using System.Drawing;
using System.Dynamic;
using System.Web.Script.Serialization;

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
            var ubicationList = from u in db.Ubications
                                where u.UbicationCategory.Name == "Condominio"
                                select u;
            ViewBag.Ubications = ubicationList.ToList();
            ViewBag.UbicationCategory = new SelectList(db.UbicationCategory, "UbicationCategoryId", "Name");



            var ArticleViewModelListOrder = ArticleViewModelList.OrderByDescending(a => a.Article.CreationDate).ToList();

            List<ArticleViewModel> ArticleViewModelCutList = new List<ArticleViewModel>();

            var i = 0;
            foreach (var article in ArticleViewModelListOrder)
            {
                if (i <= 8 && article.Article.ArticleKind == EArticleKind.Venta)
                {
                    ArticleViewModelCutList.Add(article);
                    i++;
                }
            }

            //ArticleViewModelMixedList.AddRange(ListOutstandingEF.ToList());
            //ArticleViewModelMixedList.AddRange(ListOportunityEF.ToList());

            return View(ArticleViewModelCutList);
        }

        public ActionResult ArticleViewModel(int id)
        {
            Article article = db.Articles.Find(id);
            ArticleViewModel articleViewModel = GetArticleViewModel(article);

            IQueryable<UbicationPicture> ubicationPictures = from p in db.UbicationPictures
                                                             where p.UbicationId == article.UbicationId && !p.OutstandingPicture 
                                                             select p;
            IQueryable<int> ubicationFeatureUbication = from u in db.UbicationFeaturesUbication
                                                             where u.UbicationId == articleViewModel.UbicationId 
                                                             select u.UbicationFeatureId;

            ViewBag.ubicationPictures = ubicationPictures.ToList();
            ViewBag.TerrainFeatures = new SelectList(db.TerrainFeatures, "TerrainFeatureId", "Description");
            ViewBag.HouseFeatures = new SelectList(db.HouseFeatures, "HouseFeatureId", "Description");
            ViewBag.UbicationFeaturesUbication = ubicationFeatureUbication.ToList();
            ViewBag.UbicationFeatures = new SelectList(db.UbicationFeatures, "UbicationFeatureId", "Description");

            return View(articleViewModel);
        }

        public ActionResult GetIndexArticles(int id)
        {
            var articlesEF = from a in db.Articles
                             where a.State && !a.SoldState
                             select a;

            var articles = articlesEF.ToList().OrderBy(a => a.CreationDate).ToList(); ;
            List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();
            List<ArticleViewModel> ArticleViewModelPartialList = new List<ArticleViewModel>();

            foreach (var article in articles)
            {
                ArticleViewModel articleViewModel = GetArticleViewModel(article);
                ArticleViewModelList.Add(articleViewModel);
            }

            switch (id)
            {
                case 1:
                    var ListOutstandingEF = from a in ArticleViewModelList
                                            where a.Article.ArticleKind == EArticleKind.Sobresaliente
                                            select a;
                    List<ArticleViewModel> articleViewModelsOrderedList = ListOutstandingEF.ToList();
                    ArticleViewModelPartialList = articleViewModelsOrderedList.OrderByDescending(a => a.Article.CreationDate).ToList();
                    break;

                case 2:
                    List<ArticleViewModel> ArticleViewModelCutList = new List<ArticleViewModel>();

                    var i = 0;
                    foreach (var article in ArticleViewModelList)
                    {
                        if (i <= 8 && article.Article.ArticleKind == EArticleKind.Venta)
                        {
                            ArticleViewModelCutList.Add(article);
                            i++;
                        }
                    }
                    ArticleViewModelPartialList = ArticleViewModelCutList;
                    break;

                case 3:
                    var ListOportunityEF = from a in ArticleViewModelList
                                           where a.Article.ArticleKind == EArticleKind.Oportunidad
                                           select a;
                    ArticleViewModelPartialList = ListOportunityEF.ToList();
                    break;



            }



            return PartialView("IndexArticleList", ArticleViewModelPartialList);
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


        public string ClientForm(string name, string mail, string phoneNumber, int UbicationCategory, int rangeFrom, int rangeTo)
        {
            dynamic result = new ExpandoObject();
            string ErrorMessage = "Error al agregar el cliente a la base de datos";
            string ExistingMessage = "Este correo ya ha sido agregado anteriormente";
            string SendErrorMessage = "Error al enviar el correo";
            var jsonSerialiser = new JavaScriptSerializer();

            try
            {
                Client client = new Client();
                client.Join = DateTime.Now;
                client.Name = name;
                client.Mail = mail;
                client.PhoneNumber = phoneNumber;
                client.UbicationCategory = UbicationCategory;
                client.RangeFrom = rangeFrom;
                client.RangeTo = rangeTo;
                if (VerifyClient(client))
                {
                    NewClient(client);
                }
                else
                {
                    result.status = false;
                    result.message = ExistingMessage;
                    return jsonSerialiser.Serialize(result);

                }
            }
            catch (Exception)
            {
                result.status = false;
                result.message = ErrorMessage;
                return jsonSerialiser.Serialize(result);
            }


            /*var picture = db.UbicationPictures.Find(3);
            var picture1 = db.UbicationPictures.Find(4);
            var picture2 = db.UbicationPictures.Find(3);

            MemoryStream image = new MemoryStream(picture.PictureArray);
            MemoryStream image1 = new MemoryStream(picture1.PictureArray);
            MemoryStream image2 = new MemoryStream(picture2.PictureArray);*/

            try
            {
                var ubicationCategoryName = db.UbicationCategory.Find(UbicationCategory).Name;

                var body =
                            "<h2 > Buenas,</h2>" +
                            "<p>" +
                            "Mi nombre es " + name + ", estoy en busca de una propiedad en " + ubicationCategoryName + "<br />" +
                            "tengo un presupuesto entre ¢" + rangeFrom + ",000,000 y ¢" + rangeTo + ",000,000.<br />" +
                            "<strong> Espero pronta respuesta</strong> <br />" +
                            "Gracias!!!" +
                            "</p >" +
                            " <br />" +
                            "<h4 > Informacion de contacto</h4 >" +
                            "<span > Correo: " + mail + "</span >" +
                            "<br />" +
                            "<span > Movil: " + phoneNumber + "</span > <br />" +
                            "<img width='250' height='250' src='cid:imagen' /><br />" +
                            "<img width='250' height='250' src='cid:imagen1' /><br />" +
                            "<img width='250' height='250' src='cid:imagen2' /><br />";

                AlternateView plainView =
                AlternateView.CreateAlternateViewFromString(body,
                                Encoding.UTF8,
                                MediaTypeNames.Text.Html);
                /*LinkedResource img =
                             new LinkedResource(image,
                            MediaTypeNames.Image.Jpeg);
                LinkedResource img1 =
                 new LinkedResource(image1,
                MediaTypeNames.Image.Jpeg);
                LinkedResource img2 =
                 new LinkedResource(image2,
                MediaTypeNames.Image.Jpeg);
                img.ContentId = "imagen";
                img1.ContentId = "imagen1";
                img2.ContentId = "imagen2";
                plainView.LinkedResources.Add(img);
                plainView.LinkedResources.Add(img1);
                plainView.LinkedResources.Add(img2);*/


                SendMail("", "Solicitud de informacion sobre bienes raices", plainView);

                result.status = true;
                return jsonSerialiser.Serialize(result);
            }
            catch (Exception)
            {

                result.status = false;
                result.message = SendErrorMessage;
                return jsonSerialiser.Serialize(result);
            }

        }

        public bool VerifyClient(Client client)
        {
            var status = true;
            List<Client> ClientList = db.Clients.ToList();
            foreach (var clientItem in ClientList)
            {
                if (clientItem.Mail == client.Mail)
                {
                    status = false;
                }
            }

            return status;
        }

        public bool NewClient(Client client)
        {
            var status = false;
            try
            {
                db.Clients.Add(client);
                db.SaveChanges();
                var State = db.ClientStates.FirstOrDefault(c => c.Name == "Lead");
                var Action = db.StateActions.FirstOrDefault(a => a.Name == "Informacion");
                var StateActionState = db.StateActionState.FirstOrDefault(s => s.ClientStateId == State.ClientStateId && s.StateActionId == Action.StateActionId);
                ClientStateAction clientStateAction = new ClientStateAction();
                clientStateAction.ClientId = client.ClientId;
                clientStateAction.StateActionStateId = StateActionState.StateActionStateId;
                clientStateAction.JoinAction = DateTime.Now;
                clientStateAction.Message = "Solicitud de informacion";
                db.ClientStateAction.Add(clientStateAction);
                db.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            {

                return status;
            }

        }

        public bool SendMail(string to, string subject, AlternateView alternateView)
        {
            var status = false;

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("rvilla3452@gmail.com");
                if (to != "")
                {
                    mail.To.Add(to);

                }
                mail.To.Add("jrvj_1292@hotmail.com");
                mail.Subject = subject;
                mail.AlternateViews.Add(alternateView);
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 25;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;
                string MailAccount = "rvilla3452@gmail.com";
                string Password = "dhliugyqeqsyqrbi";
                smtp.Credentials = new System.Net.NetworkCredential(MailAccount, Password);
                smtp.Send(mail);
                status = true;
                return status;

            }
            catch (Exception)
            {
                status = false;
                return status;
            }


        }

        public ActionResult PropertiesView()
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

            return View(ArticleViewModelList);
        }

        public ActionResult GetArticles(string type, string param)
        {
            List<ArticleViewModel> ArticleViewModelList = new List<ArticleViewModel>();

            switch (type)
            {
                case "Ubicacion":
                    ArticleViewModelList = FilterUbication(param);
                    break;

                case "Categoria":
                    ArticleViewModelList = FilterCategory(param);
                    break;

                case "Tamaño":
                    ArticleViewModelList = FilterSize(Int32.Parse(param));
                    break;

                case "Precio":
                    ArticleViewModelList = FilterPrice(Int32.Parse(param));
                    break;

                default:
                    var articles = db.Articles.ToList();
                    ArticleViewModelList = GetArticleViewModelList(articles);
                    break;

            }

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

        public List<ArticleViewModel> FilterCategory(string category)
        {
            var articlesEF = from a in db.Articles
                             where a.Ubication.UbicationCategory.Name == category
                             select a;

            var articles = articlesEF.ToList();


            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

            return ArticleViewModelList;
        }

        public List<ArticleViewModel> FilterPrice(int size)
        {
            int initialPrice = 0;
            int finalPrice = 0;
            string currency = "";

            switch (size)
            {
                case 1:
                    initialPrice = 20000;
                    finalPrice = 30000;
                    currency = "$";
                    break;

                case 2:
                    initialPrice = 30000;
                    finalPrice = 40000;
                    currency = "$";
                    break;

                case 3:
                    initialPrice = 40000;
                    finalPrice = 500000;
                    currency = "$";
                    break;

                case 4:
                    initialPrice = 10000000;
                    finalPrice = 20000000;
                    currency = "¢";
                    break;
                case 5:
                    initialPrice = 20000000;
                    finalPrice = 30000000;
                    currency = "¢";
                    break;
                case 6:
                    initialPrice = 30000000;
                    finalPrice = 1000000000;
                    currency = "¢";
                    break;
            }


            var articles = db.Articles.ToList();
            List<Article> ArticleList = new List<Article>();
            foreach (var art in articles)
            {
                if (Int32.Parse(art.Price) >= initialPrice && Int32.Parse(art.Price) <= finalPrice && art.Currency == currency)
                {
                    ArticleList.Add(art);
                }
            }


            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(ArticleList);

            return ArticleViewModelList;
        }

        public List<ArticleViewModel> FilterSize(int size)
        {
            int initialSize = 0;
            int finalSize = 0;

            switch (size)
            {
                case 1:
                    initialSize = 100;
                    finalSize = 200;
                    break;

                case 2:
                    initialSize = 300;
                    finalSize = 500;
                    break;

                case 3:
                    initialSize = 500;
                    finalSize = 700;
                    break;

                case 4:
                    initialSize = 700;
                    finalSize = 100000;
                    break;
            }


            var articlesEF = from a in db.Articles
                             where (a.Terrain.BackgroundMeasure * a.Terrain.ForeheadMeasure) >= initialSize && 
                             (a.Terrain.BackgroundMeasure * a.Terrain.ForeheadMeasure) <= finalSize
                             select a;

            var articles = articlesEF.ToList();


            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);

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
                case "Categoria":
                    var category = from c in db.UbicationCategory
                                     select c.Name;
                    objectList = category.ToList();
                    break;

            }
            var content = "<option selected disabled>Seleccionar filtro</option>";
            foreach (var obj in objectList)
            {
                content += "<option value='" + obj + "'>" + obj + "</option>";
            }

            return content;
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