using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Labels.Models
{
    
    public class LabelData : IModel<LabelData>
    {

        #region "Public Properties"

        public int Id { get; set; }

        public int LabelId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        #endregion

        #region "Constructor"

        public LabelData()
        {

        }

        public LabelData(IDataReader reader)
        {
            PopulateModel(reader);
        }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("LabelId"))
                this.LabelId = Convert.ToInt32(dr["LabelId"]);

            if (dr.ColumnIsNotNull("Key"))
                this.Key = Convert.ToString(dr["Key"]);

            if (dr.ColumnIsNotNull("Value"))
                this.Value = Convert.ToString(dr["Value"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                this.CreatedDate = Convert.ToDateTime((dr["CreatedDate"]));

            if (dr.ColumnIsNotNull("CreatedUserId"))
                this.CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime((dr["ModifiedDate"]));

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

        }

        public void PopulateModel(Action<LabelData> model)
        {
            model(this);
        }

        #endregion
        
    }

}
