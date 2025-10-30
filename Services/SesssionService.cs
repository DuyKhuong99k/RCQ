using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ViewModels.Repos.Hubs.IServices;

namespace Services
{
    public class SesssionService(IHttpContextAccessor httpContextAccessor) : ISessionService
    {
        public void Set(string key, string value)
        {
            httpContextAccessor.HttpContext?.Session.SetString(key, value);
        }

        public string? Get(string key)
        {
            try
            {
                var val = httpContextAccessor.HttpContext?.Session.GetString(key);
                return val;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
