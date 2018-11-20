using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using okta_aspnetcore_webapi_example.Authentication;

namespace okta_aspnetcore_webapi_example.Controllers
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = nameof(CustomAuthHandler))]
    public class MessagesController : Controller
    {
        [HttpGet]
        [Route("~/api/messages")]
        public dynamic Get()
        {
            var principal = HttpContext.User.Identity as ClaimsIdentity;

            var login = principal.Claims
                .SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            return new 
            {
                Messages = new []
                {
                    new { Date = DateTime.Now, Text = "I am a Robot." },
                    new { Date = DateTime.Now, Text = "Hello, world!" },
                }
            };
        }
        
        [HttpGet]
        [Route("~/api/restricted_messages")]
        public dynamic GetData()
        {
            var principal = HttpContext.User.Identity as ClaimsIdentity;

            var login = principal.Claims
                .SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            return new 
            {
                Messages = new []
                {
                    new { Date = DateTime.Now, Text = "I am the IRobot." },
                    new { Date = DateTime.Now, Text = "Hello, world!" },
                }
            };
        }
    }
}