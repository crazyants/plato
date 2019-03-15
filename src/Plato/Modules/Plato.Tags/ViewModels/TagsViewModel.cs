using System;
using System.Collections.Generic;
using System.Text;
using Plato.Tags.Models;

namespace Plato.Tags.ViewModels
{
    public class TagsViewModel<TModel> where TModel : class, ITag
    {

        public int SelectedTagId { get; set; }

        public IEnumerable<TModel> Tags { get; set; }

    }
}
