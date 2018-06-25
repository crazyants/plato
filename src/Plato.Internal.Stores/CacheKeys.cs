namespace Plato.Internal.Stores
{

    // Static class to help centralize cache keys. 
    // No logic of significance should be conducted in this class

    public static class CacheKey
    {
        public static string GetRolesCacheKey() => CacheKeys.Roles.ToString();
        
        public static string GetRoleCacheKey(int roleId)
        {
            return $"{CacheKeys.Roles.ToString()}_{roleId.ToString()}";
        }

        public static string GetRoleCacheKey(string roleName)
        {
            return $"{CacheKeys.Roles.ToString()}_{roleName.ToString()}";
        }
        
        public static string GetRolesByUserIdCacheKey(int userId)
        {
            return $"{CacheKeys.UserRoles.ToString()}_{userId.ToString()}";
        }


    }

    public enum CacheKeys
    {
        ShellDescriptor,
        SiteSettings,
        Roles,
        Users,
        UserRoles,
        UserPhotos,
        UserBanners
    }
}
