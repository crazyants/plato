using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Plato.Internal.Abstractions.Extensions;
using Plato.Media.Attributes;
using Plato.Media.Services;
using Plato.Media.Stores;
using Plato.Media.ViewModels;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;

namespace Plato.Media.Controllers
{

    // https://github.com/aspnet/Docs/tree/master/aspnetcore/mvc/models/file-uploads/sample/FileUploadSample

    public class StreamingController : BaseWebApiController
    {

        private static readonly string[] SupportedImageContentTypes = new string[]
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/jpg",
            "image/bmp"
        };
        
        private static readonly string[] SupportedBinaryContentTypes = new string[]
        {
            "text/plain",
            "text/html",
            "application/octet-stream"
        };
        
        private readonly ILogger<StreamingController> _logger;
        private readonly IMediaStore<Models.Media> _mediaStore;

        // Get the default form options so that we can use them
        // to set the default limits for request body data
        private readonly FormOptions _defaultFormOptions = new FormOptions();

        public StreamingController(
            ILogger<StreamingController> logger,
            IMediaStore<Models.Media> mediaStore)
        {
            _logger = logger;
            _mediaStore = mediaStore;
        }

        #region "Actions"

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        

        [HttpPost, DisableFormValueModelBinding, ValidateClientAntiForgeryToken]
        public async Task<IActionResult> Upload()
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }
            
            // Ensure we are dealing with a multipart request
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            // Used to accumulate all the form url
            // encoded key value pairs in the request.
            var formAccumulator = new KeyValueAccumulator();
            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            
            var name = string.Empty;
            var contentType = string.Empty;
            long contentLength = 0;
            byte[] bytes = null;
            
            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
            
                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {

                        name = contentDisposition.FileName.ToString();
                        contentType = section.ContentType;
                        
                        // Create an in-memory stream to get the bytes and length
                        using (var ms = new MemoryStream())
                        {

                            // Read the seciton into our memory stream
                            await section.Body.CopyToAsync(ms);

                            // get bytes and length
                            bytes = ms.StreamToByteArray();
                            contentLength = ms.Length;

                        }

                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key" value
                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.

                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).ToString();
                        var encoding = GetEncoding(section);

                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            formAccumulator.Append(key, value);

                            if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // Get btye array from memory stream for storage
          
            var output = new List<UploadedFile>();

            if (bytes == null)
            {
                return BadRequest($"Could not obtain a byte array for the uploaded file.");
            }

            // Store media
            var media = await _mediaStore.CreateAsync(new Models.Media
            {
                Name = name,
                ContentType = contentType,
                ContentLength = contentLength,
                ContentBlob = bytes,
                CreatedUserId = user.Id
            });

            // Build friendly results
            if (media != null)
            {
                output.Add(new UploadedFile()
                {
                    Id = media.Id,
                    Name = media.Name,
                    FriendlySize = media.ContentLength.ToFriendlyFileSize(),
                    IsImage = IsContentTypeSupported(media.ContentType, SupportedImageContentTypes),
                    IsBinary = IsContentTypeSupported(media.ContentType, SupportedBinaryContentTypes),
                });
            }

            return base.Result(output);

        }

        #endregion

        #region "Private Methods"
        
        bool IsContentTypeSupported(string contentType, string[] supportedTypes)
        {

            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }

            foreach (var supportedType in supportedTypes)
            {
                if (contentType.Equals(supportedType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;

        }
        
        Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
            // UTF-7 is insecure and should not be honored.
            // UTF-8 will succeed in most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

        #endregion

    }

}
