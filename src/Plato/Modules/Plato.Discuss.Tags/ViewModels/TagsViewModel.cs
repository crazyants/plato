using System;
using System.Collections.Generic;
using System.Text;
using Plato.Tags.Models;

namespace Plato.Discuss.Tags.ViewModels
{
    public class TagsViewModel
    {

        public int SelectedLabelId { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }

}
