using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Plato.WebApi.Controllers
{

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }


    public class UsersController : Controller
    {

        [HttpGet]
        public ActionResult Get()
        {

            var data = new string[]
            {
                "test",
                "test"
            };

            return new ObjectResult(new
            {
                Data = data,
                StatusCode = HttpStatusCode.OK,
                Message = "Album created successfully."
            });

        }

    }
}
