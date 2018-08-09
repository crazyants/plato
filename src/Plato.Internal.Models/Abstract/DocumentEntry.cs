using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Abstract
{
    public class DocumentEntry : IModel<DocumentEntry>
    {

        #region "Public Properties"

        public int Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        #endregion

        #region "Public Methods"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("Type"))
                Type = Convert.ToString((dr["Type"]));

            if (dr.ColumnIsNotNull("Value"))
                Value = Convert.ToString((dr["Value"]));
            
            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32((dr["ModifiedUserId"]));

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = DateTimeOffset.Parse(Convert.ToString((dr["ModifiedDate"])));

        }

        public void PopulateModel(Action<DocumentEntry> model)
        {
            model(this);
        }

        #endregion

    }

}
