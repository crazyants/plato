using System;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Plato.Models.Annotations;
using Plato.Abstractions.Extensions;

namespace Plato.Models.Roles
{

    [TableName("Plato_Roles")]
    public class Role : IdentityRole<int>, IModel<Role>
    {

        #region "Public Properties"
        
        public override int Id { get; set; }

        public int PermissionId { get; set; }

        public override string Name { get; set; }

        public string Description { get; set;  }

        public string HtmlPrefix { get; set; }

        public string HtmlSuffix { get; set; }

        public bool IsAdministrator { get; set; }

        public bool IsEmployee { get; set; }

        public bool IsAnonymous { get; set; }

        public bool IsMember { get; set; }

        public bool IsWaitingConfirmation { get; set; }

        public bool IsBanned { get; set; }

        public int SortOrder { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public int DeletedUserId { get; set; }
        
        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("PermissionId"))
                this.PermissionId = Convert.ToInt32(dr["PermissionId"]);

            if (dr.ColumnIsNotNull("Name"))
                this.Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("Description"))
                this.Description = Convert.ToString(dr["Description"]);

            if (dr.ColumnIsNotNull("HtmlPrefix"))
                this.HtmlPrefix = Convert.ToString(dr["HtmlPrefix"]);

            if (dr.ColumnIsNotNull("HtmlSuffix"))
                this.HtmlSuffix = Convert.ToString(dr["HtmlSuffix"]);

            if (dr.ColumnIsNotNull("IsAdministrator"))
                this.IsAdministrator = Convert.ToBoolean(dr["IsAdministrator"]);

            if (dr.ColumnIsNotNull("IsEmployee"))
                this.IsEmployee = Convert.ToBoolean(dr["IsEmployee"]);
            
            if (dr.ColumnIsNotNull("IsAnonymous"))
                this.IsAnonymous = Convert.ToBoolean(dr["IsAnonymous"]);

            if (dr.ColumnIsNotNull("IsMember"))
                this.IsMember = Convert.ToBoolean(dr["IsMember"]);

            if (dr.ColumnIsNotNull("IsWaitingConfirmation"))
                this.IsWaitingConfirmation = Convert.ToBoolean(dr["IsWaitingConfirmation"]);

            if (dr.ColumnIsNotNull("IsBanned"))
                this.IsBanned = Convert.ToBoolean(dr["IsBanned"]);

            if (dr.ColumnIsNotNull("SortOrder"))
                this.SortOrder = Convert.ToInt32(dr["SortOrder"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                this.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                this.CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                this.ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                this.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("DeletedDate"))
                this.DeletedDate = Convert.ToDateTime(dr["DeletedDate"]);

            if (dr.ColumnIsNotNull("DeletedUserId"))
                this.DeletedUserId = Convert.ToInt32(dr["DeletedUserId"]);

            if (dr.ColumnIsNotNull("ConcurrencyStamp"))
                this.ConcurrencyStamp = Convert.ToString(dr["ConcurrencyStamp"]);
            

        }

        public void PopulateModel(Action<Role> action)
        {
            action(this);
        }


        #endregion
        
    }
}