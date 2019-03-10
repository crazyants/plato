using Plato.Entities.Models;

namespace Plato.Entities.Extensions
{
    public static class EntityExtensions
    {

        public static bool IsHidden(this IEntity entity)
        {

            if (entity.IsPrivate)
            {
                return true;
            }

            if (entity.IsDeleted)
            {
                return true;
            }

            if (entity.IsSpam)
            {
                return true;
            }

            return false;

        }

    }

}
