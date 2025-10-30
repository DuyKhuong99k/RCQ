using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public UtilitiesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;




        [HttpGet("{search}")]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get(string search = "")
        {
            var controllerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

            var controllerNames = controllerTypes.Select(type => type.Name.Replace("Controller", ""));

            if (!string.IsNullOrEmpty(search))
            {
                var filteredControllers = controllerNames
                    .Where(name => name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderBy(name => name);

                return Ok(filteredControllers);
            }

            return Ok(controllerNames);
        }
    }
}
