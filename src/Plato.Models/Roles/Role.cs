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

        #region "Constructor"
        public Role()
        {

        }

        public Role(string name)
            :this(0, name)
        {
            
        }

        public Role(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Role(IDataReader reader)
        {
           PopulateModel(reader);
        }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("PermissionId"))
                PermissionId = Convert.ToInt32(dr["PermissionId"]);

            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("NormalizedName"))
                NormalizedName = Convert.ToString(dr["NormalizedName"]);

            if (dr.ColumnIsNotNull("Description"))
                Description = Convert.ToString(dr["Description"]);

            if (dr.ColumnIsNotNull("HtmlPrefix"))
                HtmlPrefix = Convert.ToString(dr["HtmlPrefix"]);

            if (dr.ColumnIsNotNull("HtmlSuffix"))
                HtmlSuffix = Convert.ToString(dr["HtmlSuffix"]);

            if (dr.ColumnIsNotNull("IsAdministrator"))
                IsAdministrator = Convert.ToBoolean(dr["IsAdministrator"]);

            if (dr.ColumnIsNotNull("IsEmployee"))
                IsEmployee = Convert.ToBoolean(dr["IsEmployee"]);
            
            if (dr.ColumnIsNotNull("IsAnonymous"))
                IsAnonymous = Convert.ToBoolean(dr["IsAnonymous"]);

            if (dr.ColumnIsNotNull("IsMember"))
                IsMember = Convert.ToBoolean(dr["IsMember"]);

            if (dr.ColumnIsNotNull("IsWaitingConfirmation"))
                IsWaitingConfirmation = Convert.ToBoolean(dr["IsWaitingConfirmation"]);

            if (dr.ColumnIsNotNull("IsBanned"))
                IsBanned = Convert.ToBoolean(dr["IsBanned"]);

            if (dr.ColumnIsNotNull("SortOrder"))
                SortOrder = Convert.ToInt32(dr["SortOrder"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("DeletedDate"))
                DeletedDate = Convert.ToDateTime(dr["DeletedDate"]);

            if (dr.ColumnIsNotNull("DeletedUserId"))
                DeletedUserId = Convert.ToInt32(dr["DeletedUserId"]);

            if (dr.ColumnIsNotNull("ConcurrencyStamp"))
                ConcurrencyStamp = Convert.ToString(dr["ConcurrencyStamp"]);
            
        }

        public void PopulateModel(Action<Role> action)
        {
            action(this);
        }


        #endregion
        
    }
}