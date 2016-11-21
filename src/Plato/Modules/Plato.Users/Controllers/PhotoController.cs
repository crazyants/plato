using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Abstractions.Extensions;
using Plato.FileSystem;
using Plato.Hosting.Web;
using Plato.Models.Users;
using Plato.Stores.Users;
using Microsoft.AspNetCore.Hosting;

namespace Plato.Users.Controllers
{
    public class PhotoController : Controller
    {
        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPlatoFileSystem _fileSystem;

        public PhotoController(
            IContextFacade contextFacade,
            IPlatoUserStore<User> platoUserStore,
            IUserPhotoStore<UserPhoto> userPhotoStore,
            IHostingEnvironment hostingEnvironment,
            IPlatoFileSystem fileSystem
        )
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
            _userPhotoStore = userPhotoStore;
            _hostingEnvironment = hostingEnvironment;
            _fileSystem = fileSystem;
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


            var r = Response;
            r.Clear();
            if (userPhoto != null && userPhoto.ContentLength >= 0)
            {
                r.ContentType = userPhoto.ContentType;
                r.Headers.Add("content-disposition", "filename=\"" + userPhoto.Name + "\"");
                r.Body.Write(userPhoto.ContentBlob, 0, (int) userPhoto.ContentLength);
            }
            else
            {
                var bytes = GetPlaceHolder();
                r.ContentType = "image/png";
                r.Headers.Add("content-disposition", "filename=\"empty.png\"");
                r.Body.Write(bytes, 0, (int) bytes.Length);
            }

            return View();

        }


        public byte[] GetPlaceHolder()
        {

            var file = _fileSystem.OpenFile(_fileSystem.Combine(
                _hostingEnvironment.ContentRootPath,
                "wwwroot",
                "images",
                "empty.png"));

            return file.StreamToByteArray();
            
        }



    }
}