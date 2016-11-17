using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Abstractions.Extensions;
using Plato.Stores.Users;
using Plato.Hosting.Web;
using Plato.Models.Users;

namespace Plato.Users.Controllers
{
    public class PhotoController : Controller
    {
        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore _platoUserStore;

        public PhotoController(
            IContextFacade contextFacade,
            IPlatoUserStore platoUserStore
            )
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Upload(string returnUrl = null)
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Upload(int id, ICollection<IFormFile> files)
        {

            var user = await _contextFacade.GetAuthenticatedUser();
            if (user == null)
                return View();

            foreach (var file in files)
            {
                // prepare content type
                var contentType = file.ContentType;
                var fileName = file.FileName;
                System.Byte[] byteAttay = file.OpenReadStream().StreamToByteArray();
                
                user.Photo = new UserPhoto()
                {
                    Name = fileName,
                    ContentType = contentType,
                    ContentBlob = byteAttay
                };

                var newUser = await _platoUserStore.UpdateAsync(user);


            }
            

            //_platoUserStore

            //var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            //foreach (var file in files)
            //{
            //    if (file.Length > 0)
            //    {
            //        using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
            //        {
            //            await file.CopyToAsync(fileStream);
            //        }
            //    }
            //}

            return View();

        }

    }

}
