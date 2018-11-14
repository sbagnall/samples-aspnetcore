using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace okta_aspnetcore_mvc_example.Controllers
{
    public class DataController : Controller
    {
        //private readonly IHttpClientFactory httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public DataController(//IHttpClientFactory httpClientFactory,                            
                              IOptions<ApiSettings> apiSettings)
        {
            //this.httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings?.Value ?? new ApiSettings();
        }
        public async Task<IActionResult> IndexWithoutToken(CancellationToken cancellation)
        {
            var client = new HttpClient();//httpClientFactory.CreateClient();

            var data = await client
                .GetAsync(
                    _apiSettings.Url + _apiSettings.MessagesPath,
                    cancellation);

            return Json(data.StatusCode);
        }

        [Route("messages")]
        public async Task<IActionResult> Index(CancellationToken cancellation)
        {
            var client = new HttpClient();//httpClientFactory.CreateClient();

            var accessToken = "_tokenService";
            var token = new List<string>(){$"SSWS {accessToken}"};
            client.DefaultRequestHeaders.Add("Authorization", token);

            var data = await client
                .GetAsync(
                    _apiSettings.Url + _apiSettings.MessagesPath,
                    cancellation);

            return View(data);
        }

    }


}