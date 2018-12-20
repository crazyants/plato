using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Notifications.Models;
using Plato.Notifications.Stores;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;

namespace Plato.Notifications.Controllers
{
    
    public class UserController : BaseWebApiController
    {

        private readonly IUserNotificationsStore<UserNotification> _userNotificationStore;
        private readonly IPlatoUserStore<User> _ploatUserStore;
        private readonly IContextFacade _contextFacade;

        public UserController(
            IPlatoUserStore<User> platoUserStore,
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade,
            IUserNotificationsStore<UserNotification> userNotificationStore)
        {
            _ploatUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _userNotificationStore = userNotificationStore;
        }

        #region "Actions"

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string sort = "CreatedDate",
            OrderBy order = OrderBy.Desc)
        {

            // Ensure we are authenticated
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Get notificaitons
            var userNotifications = await GetUserNotifications(
                page,
                size,
                user.Id,
                sort,
                order);

            IPagedResults<UserNotificationApiResult> results = null;
            if (userNotifications != null)
            {
                results = new PagedResults<UserNotificationApiResult>
                {
                    Total = userNotifications.Total
                };

                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var userNotification in userNotifications.Data)
                {

                    var userUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Users",
                        ["Controller"] = "Home",
                        ["Action"] = "Display",
                        ["Id"] = userNotification.User.Id,
                        ["Alias"] = userNotification.User.Alias
                    });

                    var fromUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Users",
                        ["Controller"] = "Home",
                        ["Action"] = "Display",
                        ["Id"] = userNotification.CreatedBy.Id,
                        ["Alias"] = userNotification.CreatedBy.Alias
                    });

                    var url = userNotification.Url;
                    if (url != null)
                    {
                        var noHttp = url.IndexOf("http://", StringComparison.OrdinalIgnoreCase) == -1;
                        var noHttps = url.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1;
                        var relativeUrl = (noHttp && noHttps);
                        if (relativeUrl)
                        {
                            url = baseUrl + url;
                        }
                    }

                    results.Data.Add(new UserNotificationApiResult()
                    {
                        Id = userNotification.Id,
                        User = new UserApiResult()
                        {
                            Id = userNotification.User.Id,
                            DisplayName = userNotification.User.DisplayName,
                            UserName = userNotification.User.UserName,
                            Url = userUrl
                        },
                        From = new UserApiResult()
                        {
                            Id = userNotification.CreatedBy.Id,
                            DisplayName = userNotification.CreatedBy.DisplayName,
                            UserName = userNotification.CreatedBy.UserName,
                            Url = fromUrl
                        },
                        Title = userNotification.Title,
                        Message = userNotification.Message,
                        Url = url,
                        Date = new FriendlyDate()
                        {
                            Text = userNotification.CreatedDate.ToPrettyDate(),
                            Value = userNotification.CreatedDate
                        }
                    });

                }

            }

            IPagedApiResults<UserNotificationApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<UserNotificationApiResult>()
                {
                    Page = page,
                    Size = size,
                    Total = results.Total,
                    TotalPages = results.Total.ToSafeCeilingDivision(size),
                    Data = results.Data
                };
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }


        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Delete(int id)
        {
            // Ensure notificaiton exists
            var userNotification = await _userNotificationStore.GetByIdAsync(id);
            if (userNotification == null)
            {
                return base.NotFound();
            }
            
            // Ensure we are authenticated
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Ensure we are deleting a notification we own
            if (userNotification.UserId != user.Id)
            {
                return base.UnauthorizedException();
            }

            var success = await _userNotificationStore.DeleteAsync(userNotification);
            if (success)
            {
                return base.Result("Notification deleted successfully");
            }

            return base.InternalServerError();

        }
        
        #endregion

        #region "Private Methods"

        async Task<IPagedResults<UserNotification>> GetUserNotifications(
            int page,
            int pageSize,
            int userId,
            string sortBy,
            OrderBy sortOrder)
        {
            return await _userNotificationStore.QueryAsync()
                .Take(page, pageSize)
                .Select<UserNotificationsQueryParams>(q => { q.UserId.Equals(userId); })
                .OrderBy(sortBy, sortOrder)
                .ToList();
        }

        #endregion

    }

}
