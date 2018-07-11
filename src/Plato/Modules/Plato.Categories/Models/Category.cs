using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Categories.Models
{
    public class Category  : IModel<Category>
    {

        public int Id { get; set; }

        public int ParentId { get; set; }

        public int FeatureId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Alias { get; set; }

        public string IconCss { get; set; }

        public string ForeColor { get; set; }

        public string BackColor { get; set; }

        public int SortOrder { get; set; }

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

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);

            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("Description"))
                Description = Convert.ToString(dr["Description"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("IconCss"))
                IconCss = Convert.ToString(dr["IconCss"]);

            if (dr.ColumnIsNotNull("ForeColor"))
                ForeColor = Convert.ToString(dr["ForeColor"]);

            if (dr.ColumnIsNotNull("BackColor"))
                BackColor = Convert.ToString(dr["BackColor"]);

            if (dr.ColumnIsNotNull("SortOrder"))
                SortOrder = Convert.ToInt32(dr["SortOrder"]);

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
