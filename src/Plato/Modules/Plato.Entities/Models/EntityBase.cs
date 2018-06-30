using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Plato.Internal.Models;

namespace Plato.Entities.Models
{
    public abstract class EntityBase :  IModel<Entity>
    {

        public int Id { get; set; }

        public int FeatureId { get; set; }

        public string Title { get; set; }

        public string TitleNormalized { get; set; }

        public string Markdown { get; set; }

        public string Html { get; set; }

        public string PlainText { get; set; }

        public bool IsPublic { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsClosed { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public IList<EntityData> Data { get; set; } = new List<EntityData>();
        
        public abstract void PopulateModel(IDataReader dr);

    }
}
