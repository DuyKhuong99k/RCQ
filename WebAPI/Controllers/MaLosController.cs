using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class MaLosController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLosController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Gets()
        {
            if (_context.MaLo == null)
            {
                return NotFound();
            }
            return Vm.VmLo.Items;
        }

        // GET: api/MaLos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            if (_context.MaLo == null)
            {
                return NotFound();
            }
            var maLo = Vm.VmLo.Find(id);

            if (maLo == null)
            {
                return NotFound();
            }

            return maLo;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetsMSLWithSize()
        {
            var items = Vm.VmLo.ItemsWithSize;
            return Ok(items);
        }

    }
}
