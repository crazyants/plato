using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Abstractions.Extensions;
using Plato.Stores.Users;
using Plato.Hosting.Web;
using Plato.Models.Users;
using Plato.Repositories.Users;

namespace Plato.Users.Controllers
{
    public class PhotoController : Controller
    {
        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore _platoUserStore;
        private readonly IUserPhotoRepository<UserPhoto> _userPhotoRepository;

        public PhotoController(
            IContextFacade contextFacade,
            IPlatoUserStore platoUserStore,
            IUserPhotoRepository<UserPhoto> userPhotoRepository
            )
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
            _userPhotoRepository = userPhotoRepository;
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
            
            System.Text.StringBuilder sb = new StringBuilder();

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
            var userPhoto = await _userPhotoRepository.SelectByUserIdAsync(user.Id);
            if (userPhoto != null)
                id = userPhoto.Id;
            
            userPhoto = await _userPhotoRepository.InsertUpdateAsync(
                new UserPhoto()
            {
                Id = id,
                UserId = user.Id,
                Name = file.FileName,
                ContentType = file.ContentType,
                ContentLength = file.Length,
                ContentBlob = bytes,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            });
            
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

            var userPhoto = await _userPhotoRepository.SelectByUserIdAsync(id);
            if (userPhoto == null)
                return View();
            if (userPhoto.ContentLength <= 0)
                return View();

            var r = Response;
            r.Clear();
            r.ContentType = userPhoto.ContentType;
            r.Headers.Add("content-disposition", "filename=\"" + userPhoto.Name + "\"");
            r.Body.Write(userPhoto.ContentBlob, 0, (int)userPhoto.ContentLength);
            
            return View();

        }




    }

}
