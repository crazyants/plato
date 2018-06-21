using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Web;
using Plato.Internal.Models.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Stores.Abstractions.Files;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.Controllers
{
    public class PhotoController : Controller
    {

        private static string _pathToEmptyImage;

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IFileStore _fileStore;
    
        public PhotoController(
            IContextFacade contextFacade,
            IPlatoUserStore<User> platoUserStore,
            IUserPhotoStore<UserPhoto> userPhotoStore,
            IHostingEnvironment hostEnvironment,
            IFileStore fileStore)
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
            _userPhotoStore = userPhotoStore;
            _hostEnvironment = hostEnvironment;
            _fileStore = fileStore;

            _pathToEmptyImage = _fileStore.Combine(hostEnvironment.ContentRootPath,
                "wwwroot",
                "images",
                "photo.png");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Upload(string returnUrl = null)
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }
                

            return View(user.Id);
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
            var existingPhoto = await _userPhotoStore.GetByUserIdAsync(user.Id);
            if (existingPhoto != null)
                id = existingPhoto.Id;

            var userPhoto = new UserPhoto
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
                userPhoto = await _userPhotoStore.UpdateAsync(userPhoto);
            else
                userPhoto = await _userPhotoStore.CreateAsync(userPhoto);

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
            
            var userPhoto = await _userPhotoStore.GetByUserIdAsync(id);
            var r = Response;
            r.Clear();
            if ((userPhoto != null) && (userPhoto.ContentLength >= 0))
            {
                r.ContentType = userPhoto.ContentType;
                r.Headers.Add("content-disposition", "filename=\"" + userPhoto.Name + "\"");
                r.Body.Write(userPhoto.ContentBlob, 0, (int) userPhoto.ContentLength);
            }
            else
            {
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