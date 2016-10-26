using System;
using System.Data;
using System.Collections.Generic;
using Plato.Models.Annotations;
using Plato.Abstractions.Extensions;

namespace Plato.Models.Users
{

    [TableName("Plato_UserSecret")]
    public class UserSecret : IModel<UserSecret>
    {

        #region "Public Properties"

        [PrimaryKey]
        [ColumnName("Id", typeof(int))]
        public int Id { get; set; }

        [ColumnName("UserId", typeof(int))]
        public int UserId { get; set; }

        [ColumnName("Password", typeof(string), 255)]
        public string PasswordHash { get; set; }

        [ColumnName("Salts", typeof(List<int>))]
        public int[] Salts { get; set; }

        public string SecurityStamp { get; set; }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                this.UserId = Convert.ToInt32(dr["UserId"]);

            if (dr.ColumnIsNotNull("PasswordHash"))
                this.PasswordHash = Convert.ToString(dr["PasswordHash"]);

            if (dr.ColumnIsNotNull("Salts"))
                this.Salts = Convert.ToString(dr["Salts"]).ToIntArray();

            if (dr.ColumnIsNotNull("SecurityStamp"))
                this.SecurityStamp = Convert.ToString(dr["SecurityStamp"]);

        }

        public void PopulateModel(Action<UserSecret> model)
        {
            model(this);
        }

        #endregion


    }

}
