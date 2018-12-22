using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Moderation.Models
{
    public class Moderator : IModel<Moderator>
    {
        
        public int Id { get; set; }
    
        public int UserId { get; set; }

        public SimpleUser User { get; set; } = new SimpleUser();

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public IList<ModeratorClaim> Claims { get; set;  } = new List<ModeratorClaim>();

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {
            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToInt32(dr["UserId"]);

            if (UserId > 0)
            {
                User.Id = UserId;
                if (dr.ColumnIsNotNull("UserName"))
                    User.UserName = Convert.ToString(dr["UserName"]);
                if (dr.ColumnIsNotNull("DisplayName"))
                    User.DisplayName = Convert.ToString(dr["DisplayName"]);
                if (dr.ColumnIsNotNull("Alias"))
                    User.Alias = Convert.ToString(dr["Alias"]);
                if (dr.ColumnIsNotNull("PhotoUrl"))
                    User.PhotoUrl = Convert.ToString(dr["PhotoUrl"]);
                if (dr.ColumnIsNotNull("PhotoColor"))
                    User.PhotoColor = Convert.ToString(dr["PhotoColor"]);
            }

            if (dr.ColumnIsNotNull("CategoryId"))
                CategoryId = Convert.ToInt32(dr["CategoryId"]);
            
            if (dr.ColumnIsNotNull("Claims"))
            {
                var claims = Convert.ToString(dr["Claims"]).Deserialize<List<ModeratorClaim>>();
                if (claims != null)
                {
                    foreach (var claim in claims)
                    {
                        this.Claims.Add(claim);
                    }
                }
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = DateTimeOffset.Parse(Convert.ToString((dr["ModifiedDate"])));

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);
        }

    }

}
