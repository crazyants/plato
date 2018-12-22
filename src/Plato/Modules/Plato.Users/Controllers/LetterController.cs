using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Drawing.Abstractions.Letters;

namespace Plato.Users.Controllers
{
    public class LetterController : Controller
    {

        private readonly IInMemoryLetterRenderer _letterRenderer;

        public LetterController(IInMemoryLetterRenderer letterRenderer)
        {
            _letterRenderer = letterRenderer;
        }

        public Task Get(char letter, string color)
        {

            var r = Response;
            r.Clear();
            using (var renderer = _letterRenderer)
            {
                var fileBytes = renderer.GetLetter(new LetterOptions()
                {
                    Letter = letter.ToString(),
                    BackColor = color,
                    ForeColor = "0000ff"
                }).StreamToByteArray();

                r.ContentType = "image/png";
                r.Headers.Add("content-disposition", "filename=\"empty.png\"");
                r.Headers.Add("content-length", Convert.ToString((int)fileBytes.Length));
                r.Body.Write(fileBytes, 0, fileBytes.Length);

            }

            return Task.CompletedTask;

        }

    }
}
