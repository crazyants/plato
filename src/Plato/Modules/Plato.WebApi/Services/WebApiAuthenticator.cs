using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.WebApi.Services
{

    public class WebApiAuthenticator : IWebApiAuthenticator
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebApiAuthenticator(
            IHttpContextAccessor httpContextAccessor,
            IPlatoUserStore<User> platoUserStore, ISiteSettingsStore siteSettingsStore)
        {
            _httpContextAccessor = httpContextAccessor;
            _platoUserStore = platoUserStore;
            _siteSettingsStore = siteSettingsStore;
        }

        #region "Implementation"

        public async Task<User> GetAuthenticatedUserAsync()
        {

            return await GetUserViaBasicAuthAsync();

            //return await GetUserViaCookieAuthAsync() ??
            //       await GetUserViaBasicAuthAsync();
        }

        #endregion

        #region "Private Methods"

        async Task<User> GetUserViaCookieAuthAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var identity = user?.Identity;
            if ((identity != null) && (identity.IsAuthenticated))
            {
                return await _platoUserStore.GetByUserNameAsync(identity.Name);
            }

            return null;
        }

        async Task<User> GetUserViaBasicAuthAsync()
        {

            var headers = _httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Authorization"))
            {

                var header = headers["Authorization"];

                var stringValues = header.Contains("Basic");

                // ensure auth.Scheme is set to basic
                if (header.Contains("Basic"))
                {
                    var value = "";

                    // ensure we have a parameter
                    if (!String.IsNullOrEmpty(value))
                    {
                        // ensure we have a apiKey
                        var credentials = value.Trim();
                        if (!String.IsNullOrEmpty(credentials))
                        {

                            var separatorIndex = credentials.IndexOf(':');
                            string appApiKey = null;
                            string userApiKey = null;

                            if (separatorIndex >= 0)
                            {
                                appApiKey = credentials.Substring(0, separatorIndex);
                                userApiKey = credentials.Substring(separatorIndex + 1);
                            }
                            else
                            {
                                appApiKey = credentials;
                            }

                            // ensure we have a app API key
                            if (!string.IsNullOrEmpty(appApiKey))
                            {

                                // Get site settings
                                var settings = await _siteSettingsStore.GetAsync();
                                if (settings == null)
                                {
                                    return null;
                                }

                                // does this match?
                                if (appApiKey == settings.ApiKey)
                                {
                                    // ensure we have a user API key
                                    if (!string.IsNullOrEmpty(userApiKey))
                                    {
                                        var user = await _platoUserStore.GetByApiKeyAsync(userApiKey);
                                        if (user != null)
                                        {
                                            return user;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }

            return null;

        }

        #endregion

    }
}
