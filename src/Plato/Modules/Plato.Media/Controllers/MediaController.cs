using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Abstractions.Extensions;
using Microsoft.AspNetCore.Hosting;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Files;
using Plato.Media.Stores;

namespace Plato.Media.Controllers
{
    
    public class MediaController : Controller
    {

        private static string _pathToEmptyImage;

        private readonly IContextFacade _contextFacade;
        private readonly IMediaStore<Models.Media> _mediaStore;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IFileStore _fileStore;
    
        public MediaController(
            IContextFacade contextFacade,
            IMediaStore<Models.Media> mediaStore,
            IHostingEnvironment hostEnvironment,
            IFileStore fileStore)
        {
            _contextFacade = contextFacade;
            _mediaStore = mediaStore;
            _hostEnvironment = hostEnvironment;
            _fileStore = fileStore;

            if (_pathToEmptyImage == null)
            {
                _pathToEmptyImage = _fileStore.Combine(hostEnvironment.ContentRootPath,
                    "wwwroot",
                    "images",
                    "photo.png");
            }
   
        }

        #region "Actions"
        
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
                
            var id = 0;
          
            var userPhoto = new Models.Media
            {
                Id = id,
                Name = file.FileName,
                ContentType = file.ContentType,
                ContentLength = file.Length,
                ContentBlob = bytes,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            if (id > 0)
                userPhoto = await _mediaStore.UpdateAsync(userPhoto);
            else
                userPhoto = await _mediaStore.CreateAsync(userPhoto);

            //// update user
            //user.Detail.ModifiedUserId = user.Id;
            //user.Detail.ModifiedDate = DateTime.UtcNow;

            //await _platoUserStore.UpdateAsync(user);

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task Serve(int id)
        {
            
            var media = await _mediaStore.GetByIdAsync(id);
            var r = Response;
            r.Clear();
            if ((media != null) && (media.ContentLength >= 0))
            {
                r.ContentType = media.ContentType;
                r.Headers.Add("content-disposition", "filename=\"" + media.Name + "\"");
                r.Headers.Add("content-length", Convert.ToString((long)media.ContentLength));
                r.Body.Write(media.ContentBlob, 0, (int)media.ContentLength);
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

        #endregion

        #region "Private Methods"
        
        #endregion

    }
}