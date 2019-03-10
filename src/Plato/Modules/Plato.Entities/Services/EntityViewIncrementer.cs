using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Shell;

namespace Plato.Entities.Services
{
    
    public class EntityViewIncrementer<TEntity> : IEntityViewIncrementer<TEntity> where TEntity : class, IEntity
    {

        public const string CookieName = "plato_reads";
        private HttpContext _context;

        private readonly IShellSettings _shellSettings;
        private readonly IEntityStore<TEntity> _entityStore;
   
        public EntityViewIncrementer(
            IEntityStore<TEntity> entityStore, 
            IShellSettings shellSettings)
        {
            _entityStore = entityStore;
            _shellSettings = shellSettings;
        }

        public IEntityViewIncrementer<TEntity> Contextulize(HttpContext context)
        {
            _context = context;
            return this;
        }

        public async Task<TEntity> IncrementAsync(TEntity entity)
        {
            
            // Transform tracking cookie into int array
            List<int> values = null;

            var storage = "";
            if (_context != null)
            {
                storage = _context.Request.Cookies[CookieName];
            }
          
            // Read existing into values
            if (!String.IsNullOrEmpty(storage))
            {
                values = storage.ToIntArray().ToList();
            }

            // Does the entity Id we are accessing exist in our store
            if (values != null)
            {
                if (values.Contains(entity.Id))
                {
                    return entity;
                }
            }

            // Increment counts for view
            entity.TotalViews = entity.TotalViews + 1;
            entity.DailyViews = entity.TotalViews.ToSafeDevision(DateTimeOffset.Now.DayDifference(entity.CreatedDate));
            var result = await _entityStore.UpdateAsync(entity);
            
            if (result != null)
            {
                if (values == null)
                {
                    values = new List<int>();
                }

                values.Add(result.Id);

                var tennantPath = "/";
                if (_shellSettings != null)
                {
                    tennantPath += _shellSettings.RequestedUrlPrefix;
                }

                // If a context is supplied use a client side cookie to track views
                // Expire the cookie every 10 minutes using a sliding expiration to
                // ensure views are updated often but not on every refresh
                _context?.Response.Cookies.Append(
                    CookieName,
                    values.ToArray().ToDelimitedString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = tennantPath,
                        Expires = DateTime.Now.AddMinutes(10)
                    });
                
            }
            
            return entity;

        }

    }

}
