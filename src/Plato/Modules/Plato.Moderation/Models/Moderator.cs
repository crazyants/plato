using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Moderation.Models
{
    public class Moderator : IModel<Moderator>
    {
        
        public int Id { get; set; }
    
        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public List<ModeratorClaim> Claims { get; set;  } = new List<ModeratorClaim>();

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
