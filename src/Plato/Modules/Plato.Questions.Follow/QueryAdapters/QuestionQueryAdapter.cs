using System;
using System.Text;
using Plato.Questions.Models;
using Plato.Entities.Stores;
using Plato.Follows.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Questions.Follow.QueryAdapters
{

    public class QuestionQueryAdapter : BaseQueryAdapterProvider<Question> 
    {

        public override void BuildWhere(IQuery<Question> query, StringBuilder builder)
        {

            // Ensure correct query type
            if (query.GetType() != typeof(EntityQuery<Question>))
            {
                return;
            }

            // Convert to correct query type
            var q = (EntityQuery<Question>)Convert.ChangeType(query, typeof(EntityQuery<Question>));

            // -----------------
            // FollowUserId
            // Only return entities if an entry exists within the Follows table
            // -----------------

            if (q.Params.FollowUserId.Value > 0)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" AND ");
                }

                builder.Append(" e.Id IN (")
                    .Append("SELECT ThingId FROM {prefix}_Follows f WHERE (f.[Name] = '")
                    .Append(FollowTypes.Question.Name)
                    .Append("' AND ")
                    .Append(q.Params.FollowUserId.ToSqlString("f.CreatedUserId"))
                    .Append("))");

            }

        }

    }

}
