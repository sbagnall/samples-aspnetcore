using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using okta_aspnetcore_mvc_example.Services;
// ReSharper disable InconsistentNaming

namespace okta_aspnetcore_mvc_example.Controllers
{
    [Route("[controller]")]
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
        [Authorize]
        public async Task<IActionResult> Index(CancellationToken cancellation)
        {
            using (var client = new HttpClient()) //httpClientFactory.CreateClient();
            {
                var token = await tokenService.GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 


                var response = await client
                    .GetAsync(
                        apiSettings.Url + apiSettings.MessagesPath,
                        cancellation);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<ApiMessages>(json);
                    return View(data.Messages);
                }

                return Content(response.StatusCode.ToString());
            }
        }
        
        [Route("messagesRestricted")]
        public async Task<IActionResult> IndexWithoutPolicy(CancellationToken cancellation)
        {
            return await GetRestrictedMessages("", cancellation);
        }

        [Route("messagesRestrictedOk")]
        public async Task<IActionResult> IndexWithPolicy(CancellationToken cancellation)
        {
            return await GetRestrictedMessages("api", cancellation);
        }

        private async Task<IActionResult> GetRestrictedMessages(string scopes, CancellationToken cancellation)
        {
            using (var client = new HttpClient()) //httpClientFactory.CreateClient();
            {
                var token = await tokenService.GetTokenAsync(scopes);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var response = await client
                    .GetAsync(
                        apiSettings.Url + "api/restricted_messages",
                        cancellation);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<ApiMessages>(json);
                    return View(nameof(Index), data.Messages);
                }

                return Content(response.StatusCode.ToString());
            }
        }
    }

    public class ApiMessages
    {
        public List<ApiDataModel> Messages { get; set; }
    }
}