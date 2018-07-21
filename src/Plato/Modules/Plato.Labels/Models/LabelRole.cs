using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Labels.Models
{
    public class LabelRole : IModel<LabelRole>
    {

        public int Id { get; set; }

        public int LabelId { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? CreatedDate { get; set; }


        public int ModifiedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }
        
        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("LabelId"))
                LabelId = Convert.ToInt32(dr["LabelId"]);

            if (dr.ColumnIsNotNull("RoleId"))
                RoleId = Convert.ToInt32(dr["RoleId"]);

            if (dr.ColumnIsNotNull("RoleName"))
                RoleName = Convert.ToString(dr["RoleName"]);
            
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
