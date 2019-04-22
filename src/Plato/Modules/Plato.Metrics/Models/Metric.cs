using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Metrics.Models
{

    public class Metric : IDbModel
    {

        public int Id { get; set; }
        
        public int FeatureId { get; set; }

        public string AreaName { get; set; }
        
        public string ControllerName { get; set; }
        
        public string ActionName { get; set; }
        
        public string IpV4Address { get; set; }

        public string IpV6Address { get; set; }

        public string UserAgent { get; set; }

        public int CreatedUserId { get; set; }

        public ISimpleUser CreatedBy { get; set; } = new SimpleUser();

        public DateTimeOffset? CreatedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("AreaName"))
                AreaName = Convert.ToString(dr["AreaName"]);

            if (dr.ColumnIsNotNull("ControllerName"))
                ControllerName = Convert.ToString(dr["ControllerName"]);

            if (dr.ColumnIsNotNull("ActionName"))
                ActionName = Convert.ToString(dr["ActionName"]);

            if (dr.ColumnIsNotNull("IpV4Address"))
                IpV4Address = Convert.ToString(dr["IpV4Address"]);

            if (dr.ColumnIsNotNull("IpV6Address"))
                IpV6Address = Convert.ToString(dr["IpV6Address"]);

            if (dr.ColumnIsNotNull("UserAgent"))
                UserAgent = Convert.ToString(dr["UserAgent"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            {
                CreatedBy = new SimpleUser()
                {
                    Id = CreatedUserId
                };
                if (dr.ColumnIsNotNull("UserName"))
                    CreatedBy.UserName = Convert.ToString(dr["UserName"]);
                if (dr.ColumnIsNotNull("DisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["DisplayName"]);
                if (dr.ColumnIsNotNull("Alias"))
                    CreatedBy.Alias = Convert.ToString(dr["Alias"]);
                if (dr.ColumnIsNotNull("PhotoUrl"))
                    CreatedBy.PhotoUrl = Convert.ToString(dr["PhotoUrl"]);
                if (dr.ColumnIsNotNull("PhotoColor"))
                    CreatedBy.PhotoColor = Convert.ToString(dr["PhotoColor"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }


}
