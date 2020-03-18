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
        public WebApplication1Context db = new WebApplication1Context();

        public ActionResult Index()
        {
            List<HouseFeatureHouse> houseFeatureHouseList = db.HouseFeatureHouse.ToList();


            var articlesEF = from a in db.Articles
                             where a.State && !a.SoldState
                             select a;

            var articles = articlesEF.ToList();




            List<ArticleViewModel> ArticleViewModelList = GetArticleViewModelList(articles);
            var individualContributors = db.IndividualContributors.ToList();
            IQueryable<UbicationPicture> OutstandingPictures = from p in db.UbicationPictures
                                                               where p.OutstandingPicture == true && p.Extension != null
                                                               select p;

            ViewBag.UbicationPicture = OutstandingPictures.ToList();
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



            LandingView landingView = new LandingView
            {
                ArticleViewModels = ArticleViewModelCutList,
                IndividualContributors = individualContributors
            };

            return View(landingView);
        }

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

        public ActionResult ArticleViewModel(int id)
        {
            Article article = db.Articles.Find(id);
            ArticleViewModel articleViewModel = GetArticleViewModel(article);

            IQueryable<UbicationPicture> ubicationPictures = from p in db.UbicationPictures
                                                             where p.UbicationId == article.UbicationId && !p.OutstandingPicture
                                                             select p;
            IQueryable<UbicationFeatureUbication> ubicationFeatureUbication = from u in db.UbicationFeaturesUbication
                                                                              where u.UbicationId == articleViewModel.Article.UbicationId
                                                                              select u;

            var filesEF = from f in db.Archivos
                          where f.ArticleId == id
                          select f;
            ViewBag.Files = filesEF.ToList();
            ViewBag.ubicationPictures = ubicationPictures.ToList();
            ViewBag.TerrainFeatures = new SelectList(db.TerrainFeatures, "TerrainFeatureId", "Description");
            ViewBag.HouseFeatures = new SelectList(db.HouseFeatures, "HouseFeatureId", "Description");
            articleViewModel.Article.Ubication.UbicationFeaturesUbication = ubicationFeatureUbication.ToList();
            ViewBag.UbicationFeatures = new SelectList(db.UbicationFeatures, "UbicationFeatureId", "Description");
            return View(articleViewModel);
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


        //------------------------------------------------------------------------------------------------------//


        //---------------PictureSection---------------//



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


            LandingView landingView = new LandingView()
            {
                ArticleViewModels = ArticleViewModelPartialList
            };

            return PartialView("IndexArticleList", landingView);
        }

        [HttpGet]
        public ActionResult DescargarArchivo(Guid id)
        {
            Archivo _archivo;
            FileContentResult _fileContent;

            _archivo = db.Archivos.FirstOrDefault(x => x.Id == id);

            if (_archivo == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    // Descargamos el archivo del Servidor.
                    _fileContent = new FileContentResult(_archivo.DescargarArchivo(),
                                                         "application/octet-stream");
                    _fileContent.FileDownloadName = _archivo.Nombre + "." + _archivo.Extension;

                    // Actualizamos el nº de descargas en la base de datos.




                    return _fileContent;
                }
                catch (Exception ex)
                {
                    return HttpNotFound();
                }
            }
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

            //articleViewModel.Article.Terrain.TerrainFeaturesTerrain = terrainFeatures.ToArray();


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
                //articleViewModel.HouseFeatures = houseFeatures.ToArray();
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
                //articleViewModel.HouseAuxFeatures = houseAuxFeatures.ToArray();

            }



            return articleViewModel;
        }

        public ArticleViewModel GetHouses(int id)
        {
            try
            {


                ArticleViewModel articleViewModel = new ArticleViewModel();

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
                        var houseFeatureHouse = from f in db.HouseFeatureHouse
                                                where f.HouseId == house.HouseId
                                                select f;

                        house.HouseFeaturesHouse = houseFeatureHouse.ToList();
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

                        var houseFeatureHouseAux = from f in db.HouseFeatureHouse
                                                   where f.HouseId == houseAux.Id
                                                   select f;

                        houseAux.HouseFeaturesHouse = houseFeatureHouseAux.ToList();
                    }
                }

                articleViewModel.House = house;
                articleViewModel.HouseAux = houseAux;

                return articleViewModel;
            }
            catch (Exception ex)
            {

                throw;
            }
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

        public ActionResult ErrorPage()
        {
            return View();
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