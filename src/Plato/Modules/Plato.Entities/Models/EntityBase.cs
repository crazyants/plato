using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Entities.Models
{
    public abstract class EntityBase :  IModel<Entity>
    {

        public int Id { get; set; }

  
        public string Title { get; set; }

        public string TitleNormalized { get; set; }

        public string Message { get; set; }

        public string Html { get; set; }

        public string Abstract { get; set; }

        public bool IsPublic { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsClosed { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);
            
            if (dr.ColumnIsNotNull("Title"))
                Title = Convert.ToString(dr["Title"]);

            if (dr.ColumnIsNotNull("TitleNormalized"))
                TitleNormalized = Convert.ToString(dr["TitleNormalized"]);

            if (dr.ColumnIsNotNull("Message"))
                Message = Convert.ToString(dr["Message"]);

            if (dr.ColumnIsNotNull("Html"))
                Html = Convert.ToString(dr["Html"]);

            if (dr.ColumnIsNotNull("Abstract"))
                Abstract = Convert.ToString(dr["Abstract"]);

            if (dr.ColumnIsNotNull("IsPublic"))
                IsPublic = Convert.ToBoolean(dr["IsPublic"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("IsPinned"))
                IsPinned = Convert.ToBoolean(dr["IsPinned"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("IsClosed"))
                IsClosed = Convert.ToBoolean(dr["IsClosed"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

        }


    }
}
