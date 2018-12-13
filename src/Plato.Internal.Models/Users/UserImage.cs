using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Users
{

    public class UserImage : IModel<UserImage>
    {

        #region "Public Properties"

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public byte[] ContentBlob { get; set; }

        public string ContentType { get; set; }

        public long ContentLength { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        #endregion

        #region "constructor"

        public UserImage()
        {
        }

        public UserImage(IDataReader reader)
        {
            PopulateModel(reader);
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
