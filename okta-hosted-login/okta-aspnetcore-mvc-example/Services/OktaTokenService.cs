// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace okta_aspnetcore_mvc_example.Services
{
    public class OktaTokenService : ITokenService
    {
        private OktaToken accessToken = new OktaToken();
        private readonly IOptions<OktaApiSettings> oktaSettings;

        public OktaTokenService(IOptions<OktaApiSettings> oktaSettings)
        {
            this.oktaSettings = oktaSettings;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!this.accessToken.IsValidAndNotExpiring)
            {
                this.accessToken = await this.GetNewAccessToken();
            }
            return accessToken.AccessToken;
        }

        private async Task<OktaToken> GetNewAccessToken()
        {
            using (var client = new HttpClient())
            {
                PrepareOktaClient(client);

                var request = GetContent();

                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    throw new ApplicationException("Unable to retrieve access token from Okta");

                var json = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<OktaToken>(json);
                token.ExpiresAt = DateTime.UtcNow.AddSeconds(this.accessToken.ExpiresIn);
                return token;
            }
        }

        private HttpRequestMessage GetContent()
        {
            var postMessage = new Dictionary<string, string>();
            postMessage.Add("grant_type", "client_credentials");
            postMessage.Add("redirect_uri", "http://localhost:56708/authorization-code/callback");
            postMessage.Add("scope", "access_token");
            var request = new HttpRequestMessage(HttpMethod.Post, this.oktaSettings.Value.TokenUrl)
            {
                Content = new FormUrlEncodedContent(postMessage)
            };
            return request;
        }

        private void PrepareOktaClient(HttpClient client)
        {
            var client_id = this.oktaSettings.Value.ClientId;
            var client_secret = this.oktaSettings.Value.ClientSecret;
            var clientCreds = System.Text.Encoding.UTF8.GetBytes($"{client_id}:{client_secret}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(clientCreds));
        }
    }
}
