﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Users.Controllers
{
    public class ProfileController : Controller
    {

        public Task<IActionResult> Index()
        {

            return  Task.FromResult((IActionResult)View());
        }

    }
}