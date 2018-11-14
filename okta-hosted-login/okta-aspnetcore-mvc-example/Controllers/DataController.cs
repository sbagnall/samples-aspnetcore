using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using okta_aspnetcore_mvc_example.Services;
// ReSharper disable InconsistentNaming

namespace okta_aspnetcore_mvc_example.Controllers
{
    public class DataController : Controller
    {
        private readonly ITokenService tokenService;

        //private readonly IHttpClientFactory httpClientFactory;
        private readonly ApiSettings apiSettings;

        public DataController(//IHttpClientFactory httpClientFactory,                            
                              IOptions<ApiSettings> apiSettings,
                              ITokenService tokenService)
        {
            this.tokenService = tokenService;
            //this.httpClientFactory = httpClientFactory;
            this.apiSettings = apiSettings?.Value ?? new ApiSettings();
        }
        public async Task<IActionResult> IndexWithoutToken(CancellationToken cancellation)
        {
            var client = new HttpClient();//httpClientFactory.CreateClient();

            var data = await client
                .GetAsync(
                    apiSettings.Url + apiSettings.MessagesPath,
                    cancellation);

            return Json(data.StatusCode);
        }

        [Route("messages")]
        public async Task<IActionResult> Index(CancellationToken cancellation)
        {
            using (var client = new HttpClient()) //httpClientFactory.CreateClient();
            {
                var token = await tokenService.GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                var response = await client
                    .GetAsync(
                        apiSettings.Url + apiSettings.MessagesPath,
                        cancellation);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<ApiDataModel>>(json);
                    return View(data);
                }

                return Content(response.StatusCode.ToString());
            }
        }

    }


}