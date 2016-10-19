using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Extensions;

namespace Plato.Models.Settings
{
    public class Setting : IModel<Setting>
    {

        #region "Private Variables"

        private IDictionary<string, string> _settings;

        #endregion

        #region "Public Properties"

        public int Id { get; set;  }

        public int SiteId { get; set; }

        public int SpaceId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }
        
        public DateTime? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        #endregion

        #region "Public Read Only Properties"

        #endregion

        #region "Public ReadOnly Properties"
        
        public IDictionary<string, string> Settings
        {
            get { return _settings; }
        }

        #endregion

        #region "Public Methods"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("SiteId"))
                this.SiteId = Convert.ToInt32(dr["SiteId"]);

            if (dr.ColumnIsNotNull("SpaceId"))
                this.SpaceId = Convert.ToInt32(dr["SpaceId"]);

            if (dr.ColumnIsNotNull("Key"))
                this.Key = Convert.ToString(dr["Key"]);

            if (dr.ColumnIsNotNull("Value"))
                this.Value = Convert.ToString((dr["Value"]));

            if (dr.ColumnIsNotNull("CreatedDate"))
                this.CreatedDate = Convert.ToDateTime((dr["CreatedDate"]));

            if (dr.ColumnIsNotNull("CreatedUserId"))
                this.CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime((dr["ModifiedDate"]));

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);


            Deserialize();
        }

        #endregion


        #region "Private Methods"

        private string Serialize()
        {

            if (!string.IsNullOrEmpty(this.Value))
            {

                _settings = new Dictionary<string, string>();

            }

            return string.Empty;
        }

        private string Deserialize()
        {


            return string.Empty;

        }

        #endregion


    }
}
