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

        
        private const string HeaderName = "Authorization";
        private const string Token = "Basic ";

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
            return await GetUserViaCookieAuthAsync() ??
                   await GetUserViaBasicAuthAsync();
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
            if (!headers.ContainsKey(HeaderName))
            {
                return null;
            }

            var headerValue = string.Empty;
            if (headers[HeaderName].Count > 0)
            {
                headerValue = headers[HeaderName][0];
            }

            if (String.IsNullOrWhiteSpace(headerValue))
            {
                return null;
            }

            var startIndex = headerValue.IndexOf(Token, StringComparison.InvariantCultureIgnoreCase);
            if (startIndex == -1)
            {
                return null;
            }

            // ensure we have a credentials
            var credentials = headerValue.Substring(Token.Length);
            if (String.IsNullOrEmpty(credentials))
            {
                return null;
            }

            var separatorIndex = credentials.IndexOf(':');
            string appApiKey = null, userApiKey = null;

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
            if (String.IsNullOrEmpty(appApiKey))
            {
                return null;
            }

            // Get site settings
            var settings = await _siteSettingsStore.GetAsync();
            if (settings == null)
            {
                return null;
            }

            // Do the app keys match?
            if (!appApiKey.Equals(settings.ApiKey, StringComparison.InvariantCulture))
            {
                return null;
            }

            // Do we have a user api key?
            if (String.IsNullOrEmpty(userApiKey))
            {
                return null;
            }

            return await _platoUserStore.GetByApiKeyAsync(userApiKey);


        }

        #endregion

    }
}
