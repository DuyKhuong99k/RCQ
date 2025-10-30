using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class MaThanhPhamCaoThitsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamCaoThitsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThanhPhamCaoThits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamCaoThit>>> Gets()
        {
          if (_context.MaThanhPhamCaoThit == null)
          {
              return NotFound();
          }
            return Vm.VmThanhPhamCaoThit.Items;
        }

        // GET: api/MaThanhPhamCaoThits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamCaoThit>> Get(string id)
        {
          if (_context.MaThanhPhamCaoThit == null)
          {
              return NotFound();
          }
            var maThanhPhamCaoThit = Vm.VmThanhPhamCaoThit.Find(id);

            if (maThanhPhamCaoThit == null)
            {
                return NotFound();
            }

            return maThanhPhamCaoThit;
        }

        // PUT: api/MaThanhPhamCaoThits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThanhPhamCaoThit maThanhPhamCaoThit)
        {
            if (id != maThanhPhamCaoThit.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmThanhPhamCaoThit.Exists(maThanhPhamCaoThit))
            {
                return NotFound();
            }
            Vm.VmThanhPhamCaoThit.Update_Command.Execute(maThanhPhamCaoThit);
            return NoContent();
        }

        // POST: api/MaThanhPhamCaoThits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamCaoThit>> Post(MaThanhPhamCaoThit maThanhPhamCaoThit)
        {
          if (_context.MaThanhPhamCaoThit == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamCaoThit'  is null.");
          }
            if (Vm.VmThanhPhamCaoThit.Exists(maThanhPhamCaoThit))
            {
                return Conflict();
            }
            Vm.VmThanhPhamCaoThit.Insert_Command.Execute(maThanhPhamCaoThit);
            return CreatedAtAction("Get", new { id = maThanhPhamCaoThit.Ma }, maThanhPhamCaoThit);
        }

        // DELETE: api/MaThanhPhamCaoThits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThanhPhamCaoThit == null)
            {
                return NotFound();
            }
            var maThanhPhamCaoThit = Vm.VmThanhPhamCaoThit.Find(id);
            if (maThanhPhamCaoThit == null)
            {
                return NotFound();
            }

            Vm.VmThanhPhamCaoThit.Delete_Command.Execute(maThanhPhamCaoThit);

            return NoContent();
        }

        private bool MaThanhPhamCaoThitExists(string id)
        {
            return (_context.MaThanhPhamCaoThit?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
