using System;
using System.Data;
using Plato.Internal.Models.Annotations;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Users
{
    
    public class UserData : IModel<UserData>
    {

        #region "Public Properties"

        [PrimaryKey]
        [ColumnName("Id", typeof(int))]
        public int Id { get; set; }

        [ColumnName("UserId", typeof(int))]
        public int UserId { get; set; }

        [ColumnName("Key", typeof(string), 255)]
        public string Key { get; set; }

        [ColumnName("Value", typeof(string))]
        public string Value { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        #endregion

        #region "Constructor"

        public UserData()
        {

        }

        public UserData(IDataReader reader)
        {
            PopulateModel(reader);
        }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                this.UserId = Convert.ToInt32(dr["UserId"]);

            if (dr.ColumnIsNotNull("Key"))
                this.Key = Convert.ToString(dr["Key"]);

            if (dr.ColumnIsNotNull("Value"))
                this.Value = Convert.ToString(dr["Values"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                this.CreatedDate = Convert.ToDateTime((dr["CreatedDate"]));

            if (dr.ColumnIsNotNull("CreatedUserId"))
                this.CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime((dr["ModifiedDate"]));

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

        }

        public void PopulateModel(Action<UserData> model)
        {
            model(this);
        }

        #endregion
        
    }

}
