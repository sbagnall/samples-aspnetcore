using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace okta_aspnetcore_mvc_example.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }
}
