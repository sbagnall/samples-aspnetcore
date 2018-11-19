using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using okta_aspnetcore_mvc_example.Models;

namespace okta_aspnetcore_mvc_example.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");
             var accessToken = await HttpContext.GetTokenAsync("access_token");
             var sessionToken = await HttpContext.GetTokenAsync("session_token");
            return View((accessToken, idToken, sessionToken));
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        public IActionResult Profile()
        {
            return View(HttpContext.User.Claims);
        }
    }
}
