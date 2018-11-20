using Microsoft.AspNetCore.Authentication;

namespace okta_aspnetcore_webapi_example.Authentication
{
    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public string OktaApiUri { get; internal set; }
        public object OktaApiClientId { get; internal set; }
        public object OktaApiClientSecret { get; internal set; }
    }
}