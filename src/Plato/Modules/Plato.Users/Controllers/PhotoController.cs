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
    public class PhotoController : Controller
    {
        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;

        public PhotoController(
            IContextFacade contextFacade,
            IPlatoUserStore<User> platoUserStore,
            IUserPhotoStore<UserPhoto> userPhotoStore
        )
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
            _userPhotoStore = userPhotoStore;
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
            if (userPhoto == null)
                return View();
            if (userPhoto.ContentLength <= 0)
                return View();

            var r = Response;
            r.Clear();
            r.ContentType = userPhoto.ContentType;
            r.Headers.Add("content-disposition", "filename=\"" + userPhoto.Name + "\"");
            r.Body.Write(userPhoto.ContentBlob, 0, (int) userPhoto.ContentLength);

            return View();
        }
    }
}