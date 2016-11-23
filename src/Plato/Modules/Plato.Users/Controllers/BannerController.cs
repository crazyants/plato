using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Abstractions.Extensions;
using Plato.Hosting.Web;
using Plato.Models.Users;
using Plato.Stores.Users;

namespace Plato.Users.Controllers
{
    public class BannerController : Controller
    {
        // byte array to represent a transparent 1x1 png image
        private static readonly byte[] _emptyImage =
            Encoding.ASCII.GetBytes(
                "137807871131026100001373726882000100018600031211961370004103657765001752005551382330002511669881168311110211611997114101065100111981013273109971031018210197100121113201101600001673686584120218982542552556336412810911317110578550000736978681746696130");

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserBannerStore<UserBanner> _userBannerStore;

        public BannerController(
            IContextFacade contextFacade,
            IPlatoUserStore<User> platoUserStore,
            IUserBannerStore<UserBanner> userBannerStore
        )
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
            _userBannerStore = userBannerStore;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Upload(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var sb = new StringBuilder();

            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
                return View();

            byte[] bytes = null;
            var stream = file.OpenReadStream();
            if (stream != null)
                bytes = stream.StreamToByteArray();
            if (bytes == null)
                return View();

            var id = 0;
            var existingPhoto = await _userBannerStore.GetByUserIdAsync(user.Id);
            if (existingPhoto != null)
                id = existingPhoto.Id;

            var userBanner = new UserBanner
            {
                Id = id,
                UserId = user.Id,
                Name = file.FileName,
                ContentType = file.ContentType,
                ContentLength = file.Length,
                ContentBlob = bytes,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            if (id > 0)
                userBanner = await _userBannerStore.UpdateAsync(userBanner);
            else
                userBanner = await _userBannerStore.CreateAsync(userBanner);

            // update user
            user.Detail.ModifiedUserId = user.Id;
            user.Detail.ModifiedDate = DateTime.UtcNow;

            await _platoUserStore.UpdateAsync(user);

            ViewData["Test"] = sb.ToString();

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Serve(int id)
        {
            var userBanner = await _userBannerStore.GetByUserIdAsync(id);
            var r = Response;
            r.Clear();
            if ((userBanner != null) && (userBanner.ContentLength >= 0))
            {
                r.ContentType = userBanner.ContentType;
                r.Headers.Add("content-disposition", "filename=\"" + userBanner.Name + "\"");
                r.Body.Write(userBanner.ContentBlob, 0, (int) userBanner.ContentLength);
            }
            else
            {
                r.ContentType = "image/png";
                r.Headers.Add("content-disposition", "filename=\"empty.png\"");
                r.Body.Write(_emptyImage, 0, _emptyImage.Length);
            }
            return View();
        }
    }
}