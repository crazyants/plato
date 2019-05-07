using Plato.Entities.Models;

namespace Plato.Entities.Extensions
{
    public static class EntityReplyExtensions
    {

        public static bool IsHidden(this IEntityReply reply)
        {

            if (reply.IsHidden)
            {
                return true;
            }

            if (reply.IsDeleted)
            {
                return true;
            }

            if (reply.IsSpam)
            {
                return true;
            }

            return false;

        }
        
    }

}
