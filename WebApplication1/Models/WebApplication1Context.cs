using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class WebApplication1Context : DbContext
    {

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        //}

        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public WebApplication1Context() : base("name=SuPropiedadEnOccidente_db")
        {
        }

        public System.Data.Entity.DbSet<WebApplication1.Models.Canton> Cantons { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.Distrit> Distrits { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.Client> Clients { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.IndividualContributor> IndividualContributors { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TerrainFeature> TerrainFeatures { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.HouseFeature> HouseFeatures { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.HouseFeatureHouse> HouseFeatureHouse { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.UbicationFeature> UbicationFeatures { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.Ubication> Ubications { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.UbicationPicture> UbicationPictures { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.ArticlePicture> ArticlePictures { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.UbicationFeatureUbication> UbicationFeaturesUbication { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TerrainFeatureTerrain> TerrainFeaturesTerrain { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.UbicationCategory> UbicationCategory { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.Article> Articles { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.House> Houses { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.Terrain> Terrains { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.ClientState> ClientStates { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.StateAction> StateActions { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.StateActionState> StateActionState { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.ClientStateAction> ClientStateAction { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.Archivo> Archivos { get; set; }



    }

    public class ModelBuilder
    {
    }
}
