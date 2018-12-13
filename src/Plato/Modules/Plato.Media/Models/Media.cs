using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Media.Models
{

    public class Media : IModel<Media>
    {

        #region "Public Properties"

        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] ContentBlob { get; set; }

        public string ContentType { get; set; }

        public long ContentLength { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        #endregion

        #region "constructor"

        public Media()
        {
        }

        public Media(IDataReader reader)
        {
            PopulateModel(reader);
        }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                Id = Convert.ToInt32(dr["Id"]);
            
            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("ContentBlob"))
                ContentBlob = (byte[])(dr["ContentBlob"]);

            if (dr.ColumnIsNotNull("ContentType"))
                ContentType = Convert.ToString(dr["ContentType"]);

            if (dr.ColumnIsNotNull("ContentLength"))
                this.ContentLength = Convert.ToInt64(dr["ContentLength"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = DateTimeOffset.Parse(Convert.ToString((dr["ModifiedDate"])));

        }

        #endregion

    }
}