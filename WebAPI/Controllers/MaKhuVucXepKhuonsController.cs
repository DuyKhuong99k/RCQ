using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;

namespace WebAPI.Controllers
{
     [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaKhuVucXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaKhuVucXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaKhuVucXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaKhuVucXepKhuon>>> Gets()
        {
          if (_context.MaKhuVucXepKhuon == null)
          {
              return NotFound();
          }
            return Vm.VmKhuVucXepKhuon.Items;
        }

        // GET: api/MaKhuVucXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaKhuVucXepKhuon>> Get(string id)
        {
          if (_context.MaKhuVucXepKhuon == null)
          {
              return NotFound();
          }
            var maKhuVucXepKhuon = Vm.VmKhuVucXepKhuon.Find(id);

            if (maKhuVucXepKhuon == null)
            {
                return NotFound();
            }

            return maKhuVucXepKhuon;
        }

        // PUT: api/MaKhuVucXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaKhuVucXepKhuon maKhuVucXepKhuon)
        {
            if (id != maKhuVucXepKhuon.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKhuVucXepKhuon.Exists(maKhuVucXepKhuon))
            {
                return NotFound();
            }
            Vm.VmKhuVucXepKhuon.Update_Command.Execute(maKhuVucXepKhuon);
            return NoContent();
        }

        // POST: api/MaKhuVucXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaKhuVucXepKhuon>> Post(MaKhuVucXepKhuon maKhuVucXepKhuon)
        {
          if (_context.MaKhuVucXepKhuon == null)
          {
              return Problem("Entity set 'dbPMScontext.MaKhuVucXepKhuon'  is null.");
          }
            if (Vm.VmKhuVucXepKhuon.Exists(maKhuVucXepKhuon))
            {
                return Conflict();
            }
            Vm.VmKhuVucXepKhuon.Insert_Command.Execute(maKhuVucXepKhuon);
            return CreatedAtAction("Get", new { id = maKhuVucXepKhuon.Ma }, maKhuVucXepKhuon);
        }

        // DELETE: api/MaKhuVucXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaKhuVucXepKhuon == null)
            {
                return NotFound();
            }
            var maKhuVucXepKhuon = Vm.VmKhuVucXepKhuon.Find(id);
            if (maKhuVucXepKhuon == null)
            {
                return NotFound();
            }

            Vm.VmKhuVucXepKhuon.Delete_Command.Execute(maKhuVucXepKhuon);

            return NoContent();
        }

        private bool MaKhuVucXepKhuonExists(string id)
        {
            return (_context.MaKhuVucXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaKhuVucXepKhuon.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
    }
}
