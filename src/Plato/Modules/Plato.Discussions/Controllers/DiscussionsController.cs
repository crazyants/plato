﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Plato.Abstractions.Settings;
using System.Threading.Tasks;

namespace Plato.Discussions
{
    public class DiscussionsController : Controller
    {

        ISiteService _siteService;
        
        public DiscussionsController(
            ISiteService siteService)
        {
            _siteService = siteService;
        }
        
        public async Task<IActionResult> Index()
        {

            string path = Request.Path;
            ViewData["path"] = path;

            //string rootDirectory = _fileSystem.GetDirectoryInfo("Modules").FullName;

            //var result = _moduleLocator.LocateModules(
            //    new string[] {
            //        rootDirectory
            //    }, 
            //    "Module", 
            //    "module.txt", 
            //    false);                

            //ViewData["result"] = result;

            var settings = await _siteService.GetSiteSettingsAsync();

            List<TextObject> list = new List<TextObject>();
            list.Add(new TextObject("Ryan"));
            list.Add(new TextObject("Jane"));
            list.Add(new TextObject("Mike"));
            list.Add(new TextObject("Roger"));
            list.Add(new TextObject(settings.BaseUrl));

            return View(list);
        }

        

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }

    public class TextObject
    {
        public TextObject(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}