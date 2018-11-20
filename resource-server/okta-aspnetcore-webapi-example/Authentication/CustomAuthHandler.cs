using System.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace okta_aspnetcore_webapi_example.Authentication
{
    public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        private const string accessTokenName = "access_token";

        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headerAuthorization = (Request.Headers as FrameRequestHeaders).HeaderAuthorization;

            if (headerAuthorization == default(FrameRequestHeaders))
            {
                return AuthenticateResult.Fail("token is empty");
            }

            var token = Regex.Match(headerAuthorization, "Bearer (.*)").Groups[1].Value.Trim();

            var client = new HttpClient();

            var postMessage = new Dictionary<string, string>();
            postMessage.Add("token", token);
            postMessage.Add("token_type_hint", "access_token");

            var byteArray = Encoding.ASCII.GetBytes($"{Options.OktaApiClientId}:{Options.OktaApiClientSecret}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, Options.OktaApiUri)
            {
                Content = new FormUrlEncodedContent(postMessage),
            };

            var response = await client.SendAsync(request);

            var isValidToken = false;
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

                if (result.Any(x => x.Key == "active"))
                {
                    var isActive = result.First(x => x.Key == "active");
                    isValidToken = (bool)isActive.Value;
                }
            }

            if (!isValidToken)
            {
                return AuthenticateResult.Fail("token not valid");
            }

            var claims = new[] { new Claim("token", token) };
            var identity = new ClaimsIdentity(claims, nameof(CustomAuthHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        private object Bearer(object p)
        {
            throw new NotImplementedException();
        }
    }
}
