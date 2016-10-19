using System;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Plato.Abstractions.Extensions;
using Plato.Models.Annotations;

namespace Plato.Models.Users
{

    [TableName("Plato_UserPhoto")]
    public class UserPhoto : IModel<UserPhoto>
    {

        #region "Public Properties"
        
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }
        
        public string BackColor { get; set; }

        public string ForeColor { get; set; }

        public int PhotoId { get; set; }

        public byte[] ContentBlob { get; set; }
             
        public string ContentType { get; set; }

        public float ContentLength { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }
        
        #endregion

        #region "constructor"

        public UserPhoto()
        {       
        }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                this.UserId = Convert.ToInt32(dr["UserId"]);

            if (dr.ColumnIsNotNull("Name"))
                this.Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("BackColor"))
                this.BackColor = Convert.ToString(dr["BackColor"]);

            if (dr.ColumnIsNotNull("ContentBlob"))
                this.ContentBlob = (byte[])(dr["ContentBlob"]);

            if (dr.ColumnIsNotNull("ContentType"))
                this.ContentType = Convert.ToString(dr["ContentType"]);

            if (dr.ColumnIsNotNull("ContentLength"))
                this.ContentLength = Convert.ToInt64(dr["ContentLength"]);
                  
            if (dr.ColumnIsNotNull("CreatedDate"))
                this.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                this.CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                this.ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);
            
        }

        #endregion        

    }

}
