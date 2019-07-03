using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Entities.Models
{
    
    public class EntityReplyData : IDbModel, IEntityReplyData
    {

        #region "Public Properties"

        public int Id { get; set; }

        public int ReplyId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        #endregion

        #region "Constructor"

        public EntityReplyData()
        {
        }

        public EntityReplyData(IDataReader reader)
        {
            PopulateModel(reader);
        }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ReplyId"))
                ReplyId = Convert.ToInt32(dr["ReplyId"]);

            if (dr.ColumnIsNotNull("Key"))
                Key = Convert.ToString(dr["Key"]);

            if (dr.ColumnIsNotNull("Value"))
                Value = Convert.ToString(dr["Value"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32((dr["ModifiedUserId"]));

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = (DateTimeOffset)dr["ModifiedDate"];

        }

        #endregion
        
    }

}
