using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;
using Microsoft.AspNetCore.Hosting;
using Plato.Internal.Drawing.Abstractions.Letters;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Files;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.Controllers
{
    public class PhotoController : Controller
    {

        private static string _pathToUploadFolder;
        private static string _pathToEmptyImage;

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly IFileStore _fileStore;
        private readonly IUploadFolder _uploadFolder;

        public PhotoController(
            IContextFacade contextFacade,
            IPlatoUserStore<User> platoUserStore,
            IUserPhotoStore<UserPhoto> userPhotoStore,
            IHostingEnvironment hostEnvironment,
            IFileStore fileStore,
            IUploadFolder uploadFolder)
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
            _userPhotoStore = userPhotoStore;
            _uploadFolder = uploadFolder;
            _fileStore = fileStore;

            if (_pathToEmptyImage == null)
            {
                _pathToEmptyImage = fileStore.Combine(hostEnvironment.ContentRootPath,
                    "wwwroot",
                    "images",
                    "photo.png");
            }

            if (_pathToUploadFolder == null)
            {
                _pathToUploadFolder = fileStore.Combine(hostEnvironment.ContentRootPath,
                    "wwwroot",
                    "uploads");
            }

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
       
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }
                
            byte[] bytes = null;
            var stream = file.OpenReadStream();
            if (stream != null)
            {
                bytes = stream.StreamToByteArray();
            }
            if (bytes == null)
            {
                return NotFound();
            }


            var avatarFileName = await _uploadFolder.SaveUniqueFileAsync(stream, file.FileName, _pathToUploadFolder);

            var id = 0;
            var existingPhoto = await _userPhotoStore.GetByUserIdAsync(user.Id);
            if (existingPhoto != null)
            {
                id = existingPhoto.Id;
            }
                
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
                await _userPhotoStore.UpdateAsync(userPhoto);
            else
                await _userPhotoStore.CreateAsync(userPhoto);

            return View();
        }

        [HttpGet, AllowAnonymous, ResponseCache(Duration = 60)]
        public async Task Serve(int id)
        {
            
            var userPhoto = await _userPhotoStore.GetByUserIdAsync(id);
            var r = Response;
            r.Clear();
            if ((userPhoto != null) && (userPhoto.ContentLength >= 0))
            {
                r.ContentType = userPhoto.ContentType;
                r.Headers.Add("content-disposition", "filename=\"" + userPhoto.Name + "\"");
                r.Headers.Add("content-length", Convert.ToString((int)userPhoto.ContentLength));
                r.Body.Write(userPhoto.ContentBlob, 0, (int) userPhoto.ContentLength);
            }
            else
            {
                var fileBytes = await _fileStore.GetFileBytesAsync(_pathToEmptyImage);
                if (fileBytes != null)
                {
                    r.ContentType = "image/png";
                    r.Headers.Add("content-disposition", "filename=\"empty.png\"");
                    r.Headers.Add("content-length", Convert.ToString((int)fileBytes.Length));
                    r.Body.Write(fileBytes, 0, fileBytes.Length);
                }
            }
            
        }
    }
}