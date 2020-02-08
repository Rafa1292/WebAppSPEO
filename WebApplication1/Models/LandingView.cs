using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.ViewModel;

namespace WebApplication1.Models
{
    public class LandingView
    {
        public IEnumerable<ArticleViewModel> ArticleViewModels { get; set; }

        public IEnumerable<IndividualContributor> IndividualContributors { get; set; }

    }
}