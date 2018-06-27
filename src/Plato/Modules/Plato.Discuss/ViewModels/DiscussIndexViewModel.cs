using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;

namespace Plato.Discuss.ViewModels
{
    public class DiscussIndexViewModel
    {

        public IPagedResults<Entity> Results { get; set; }


    }
}
