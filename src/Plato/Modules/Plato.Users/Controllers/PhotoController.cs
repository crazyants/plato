﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Abstractions.Extensions;
using Plato.FileSystem;
using Plato.Hosting;
using Plato.Hosting.Web;
using Plato.Models.Users;
using Plato.Stores.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp;

namespace Plato.Users.Controllers
{
    public class PhotoController : Controller
    {
        // byte array to represent a transparent 1x1 png image
        private static readonly byte[] _emptyImage =
            Encoding.UTF8.GetBytes(
                "137807871131026100001373726882000100018600031211961370004103657765001752005551382330002511669881168311110211611997114101065100111981013273109971031018210197100121113201101600001673686584120218982542552556336412810911317110578550000736978681746696130");

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;

        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IPlatoFileSystem _platoFileSystem;

        public PhotoController(
            IContextFacade contextFacade,
            IPlatoUserStore<User> platoUserStore,
            IUserPhotoStore<UserPhoto> userPhotoStore,
            IHostingEnvironment hostEnvironment,
            IPlatoFileSystem platoFileSystem
        )
        {
            _contextFacade = contextFacade;
            _platoUserStore = platoUserStore;
            _userPhotoStore = userPhotoStore;

            _hostEnvironment = hostEnvironment;
            _platoFileSystem = platoFileSystem;

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Upload(string returnUrl = null)
        {


            var path = _platoFileSystem.Combine(
                  _hostEnvironment.ContentRootPath,
                  "wwwroot",
                  "images",
                  "empty.png");


            var sb = new StringBuilder();

            var bytes = new byte[0];
            if (_platoFileSystem.FileExists(path))
            {
                var file = _platoFileSystem.OpenFile(path);
                bytes = file.StreamToByteArray();
                foreach (var b in bytes)
                {
                    sb.Append(b);
                }
            }

            sb.Append(System.Environment.NewLine);
            sb.Append(path);

            ViewData["test"] = sb.ToString();

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

            var path = _platoFileSystem.Combine(
                _hostEnvironment.ContentRootPath,
                "wwwroot",
                "images",
                "empty.png");

            var btyes = new byte[0];
            if (_platoFileSystem.FileExists(path))
            {
                var file = _platoFileSystem.OpenFile(path);
                btyes = file.StreamToByteArray();
            }

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
                r.ContentType = "image/png";
                r.Headers.Add("content-disposition", "filename=\"empty.png\"");
                r.Body.Write(btyes, 0, btyes.Length);
            }
            return View();
        }
    }
}