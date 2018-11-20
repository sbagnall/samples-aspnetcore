using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using JWT.Serializers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using okta_aspnetcore_mvc_example.Models;
using okta_aspnetcore_mvc_example.Services;

namespace okta_aspnetcore_mvc_example.Controllers
{
    public class HomeController : Controller
    {
        private readonly OktaApiSettings _oktaApiSettings;

        public HomeController(IOptions<OktaApiSettings> oktaApiSettingsOptions)
        {
            _oktaApiSettings = oktaApiSettingsOptions.Value;
        }
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

        [Route("token/{tokenName?}")]
        public async Task<IActionResult> Token(string tokenName = "id_token")
        {
            ViewBag.TokenName = tokenName;

            var jwtDecoder = new JWT.JwtDecoder(new JsonNetSerializer(), new JwtValidator(new JsonNetSerializer(), new UtcDateTimeProvider()), new JwtBase64UrlEncoder());

            var json = jwtDecoder.Decode(await HttpContext.GetTokenAsync(tokenName));
            
            var claims = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

            return View(claims);
        }
    }
}
