using System;
using System.Collections.Generic;
using System.Text;
using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;

namespace Plato.Discuss.SimilarTopics.ViewModels
{
    public class SimilarEntitiesViewModel
    {

        public IPagedResults<Topic> Results { get; set; }
    }
}
