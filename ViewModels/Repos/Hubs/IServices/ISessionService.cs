using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ViewModels.Repos.Hubs.IServices
{
    public interface ISessionService
    {
       
        public void Set(string key, string value);
        public string? Get(string key);
    }
}
