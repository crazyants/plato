using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Web;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Stores.Files;
using Microsoft.AspNetCore.Hosting;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Files;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.Controllers
{
    public class BannerController : Controller
    {

        private static string _pathToEmptyImage;

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserBannerStore<UserBanner> _userBannerStore;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IFileStore _fileStore;

        public BannerController(
            IContextFacade contextFacade,
            IPlatoUserStore<User> platoUserStore,
            IUserBannerStore<UserBanner> userBannerStore,
            IHostingEnvironment hostEnvironment,
            IFileStore fileStore)
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
            _userBannerStore = userBannerStore;
            _hostEnvironment = hostEnvironment;
            _fileStore = fileStore;
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

            //// update user
            //user.Detail.ModifiedUserId = user.Id;
            //user.Detail.ModifiedDate = DateTime.UtcNow;

            //await _platoUserStore.UpdateAsync(user);

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
                if (string.IsNullOrEmpty(_pathToEmptyImage))
                    _pathToEmptyImage = _fileStore.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "images", "empty.png");
                var fileBytes = await _fileStore.GetFileBytesAsync(_pathToEmptyImage);
                if (fileBytes != null)
                {
                    r.ContentType = "image/png";
                    r.Headers.Add("content-disposition", "filename=\"empty.png\"");
                    r.Body.Write(fileBytes, 0, fileBytes.Length);
                }
            }
            return View();
        }
    }
}