using System;
using System.Data;
using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Annotations;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Users
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
        public string Secret { get; set; }

        [ColumnName("Salts", typeof(List<int>))]
        public int[] Salts { get; set; }

        public string SecurityStamp { get; set; }

        #endregion

        #region "Constructor"

        public UserSecret()
        {
            
        }

        public UserSecret(IDataReader reader)
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

            if (dr.ColumnIsNotNull("Secret"))
                this.Secret = Convert.ToString(dr["Secret"]);

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
