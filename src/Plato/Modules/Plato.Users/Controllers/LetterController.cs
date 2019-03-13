﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Drawing.Abstractions.Letters;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Files;

namespace Plato.Users.Controllers
{
    public class LetterController : Controller
    {

        private readonly IFileStore _fileStore;
        private readonly IInMemoryLetterRenderer _letterRenderer;
        private readonly IUploadFolder _uploadFolder;
        private readonly IHostingEnvironment _hostEnvironment;

        private static string _pathToAvatarFolder;
        private static string _urlToAvatarFolder;

        public LetterController(
            IInMemoryLetterRenderer letterRenderer,
            IFileStore fileStore,
            IShellSettings shellSettings,
            IUploadFolder uploadFolder,
            IHostingEnvironment hostEnvironment)
        {
            _letterRenderer = letterRenderer;
            _fileStore = fileStore;
            _uploadFolder = uploadFolder;
            _hostEnvironment = hostEnvironment;

            _pathToAvatarFolder = fileStore.Combine(hostEnvironment.ContentRootPath, shellSettings.Location, "avatars");
            _urlToAvatarFolder = $"/uploads/{shellSettings.Location}/avatars/";
        }

        [HttpGet, ResponseCache(Duration = 1200)]
        public async Task Get(char letter, string color)
        {
            
            var fileName = $"{letter}-{color}.png";
            var r = Response;
            r.Clear();
    
            var existingFileBytes = await _fileStore.GetFileBytesAsync(_fileStore.Combine(
                _uploadFolder.Path,
                _pathToAvatarFolder,
                fileName));
            if (existingFileBytes != null)
            {
                r.ContentType = "image/png";
                r.Headers.Add("content-disposition", $"filename=\"{fileName}\"");
                r.Headers.Add("content-length", Convert.ToString((int)existingFileBytes.Length));
                r.Body.Write(existingFileBytes, 0, existingFileBytes.Length);
            }
            else
            {

                // Ensure we have valid hex characters
                if (!color.IsValidHex())
                {
                    throw new Exception("The supplied color is not a valid hexadecimal value.");
                }
                
                // For the first request to the image generate the image,
                // save the image to disk & serve the response. Subsequent requests
                // for the same image will be served from disk to avoid creating
                // the image upon every request and to reduce global locks due
                // to System.Drawing and internal calls to GDI+ on Windows
                using (var renderer = _letterRenderer)
                {
                    var fileBytes = renderer.GetLetter(new LetterOptions()
                    {
                        Letter = letter.ToString().ToUpper(),
                        BackColor = color
                    }).StreamToByteArray();
                    
                    using (var stream = new MemoryStream(fileBytes))
                    {
                       await _uploadFolder.SaveFileAsync(stream, fileName, _pathToAvatarFolder);
                    }

                    r.ContentType = "image/png";
                    r.Headers.Add("content-disposition", $"filename=\"{fileName}\"");
                    r.Headers.Add("content-length", Convert.ToString((int)fileBytes.Length));
                    r.Body.Write(fileBytes, 0, fileBytes.Length);

                }

            }

        }
        
    }

}
