using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Entities.Models
{
    public class Entity : IModel<Entity>
    {

        public int Id { get; set; }

        public int ParentId { get; set; }

        public int FeatureId { get; set; }

        public string Title { get; set; }

        public string TitleNormalized { get; set; }

        public string Markdown { get; set; }

        public string Html { get; set; }

        public string PlainText { get; set; }

        public bool IsPublic { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsClosed { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {
            
            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ParentId"))
                ParentId = Convert.ToInt32(dr["ParentId"]);

         


        }
    }

}
