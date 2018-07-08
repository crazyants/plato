using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Follow.Models;

namespace Plato.Entities.Follow.Controllers
{
    public class FollowController : Controller
    {

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Follow(int id)
        {
            throw new NotImplementedException();
        }


        [HttpPost]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Follow(EntityFollow follow)
        {

            throw new NotImplementedException();

            //return new ObjectResult(new
            //{
            //    html,
            //    StatusCode = HttpStatusCode.OK,
            //    Message = "Markdown parsed successfully"
            //});

        }


    }
}
