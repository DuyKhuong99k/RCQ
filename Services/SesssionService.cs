using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ViewModels.Repos.Hubs.IServices;

namespace Services
{
    public class SesssionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public void Set(string key, string value)
        {
            _httpContextAccessor.HttpContext?.Session.SetString(key, value);
        }
        public SesssionService(IHttpContextAccessor httpContextAccessor)
        {
            
            _httpContextAccessor = httpContextAccessor;
        }
        public string? Get(string key)
        {
            try
            {
                var val = _httpContextAccessor.HttpContext?.Session.GetString(key);
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
