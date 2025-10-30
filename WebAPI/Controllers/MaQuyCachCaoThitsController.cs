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
    public class MaQuyCachCaoThitsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaQuyCachCaoThitsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaQuyCachCaoThits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaQuyCachCaoThit>>> Gets()
        {
          if (_context.MaQuyCachCaoThit == null)
          {
              return NotFound();
          }
            return Vm.VmQuyCachCaoThit.Items;
        }

        // GET: api/MaQuyCachCaoThits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaQuyCachCaoThit>> Get(string id)
        {
          if (_context.MaQuyCachCaoThit == null)
          {
              return NotFound();
          }
            var maQuyCachCaoThit = Vm.VmQuyCachCaoThit.Find(id);

            if (maQuyCachCaoThit == null)
            {
                return NotFound();
            }

            return maQuyCachCaoThit;
        }

        // PUT: api/MaQuyCachCaoThits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaQuyCachCaoThit maQuyCachCaoThit)
        {
            if (id != maQuyCachCaoThit.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmQuyCachCaoThit.Exists(maQuyCachCaoThit))
            {
                return NotFound();
            }
            Vm.VmQuyCachCaoThit.Update_Command.Execute(maQuyCachCaoThit);
            return NoContent();
        }

        // POST: api/MaQuyCachCaoThits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaQuyCachCaoThit>> Post(MaQuyCachCaoThit maQuyCachCaoThit)
        {
          if (_context.MaQuyCachCaoThit == null)
          {
              return Problem("Entity set 'dbPMScontext.MaQuyCachCaoThit'  is null.");
          }
            if (Vm.VmQuyCachCaoThit.Exists(maQuyCachCaoThit))
            {
                return Conflict();
            }
            Vm.VmQuyCachCaoThit.Update_Command.Execute(maQuyCachCaoThit);
            return CreatedAtAction("Get", new { id = maQuyCachCaoThit.Ma }, maQuyCachCaoThit);
        }

        // DELETE: api/MaQuyCachCaoThits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaQuyCachCaoThit == null)
            {
                return NotFound();
            }
            var maQuyCachCaoThit = Vm.VmQuyCachCaoThit.Find(id);
            if (maQuyCachCaoThit == null)
            {
                return NotFound();
            }

            Vm.VmQuyCachCaoThit.Delete_Command.Execute(maQuyCachCaoThit);

            return NoContent();
        }

        private bool MaQuyCachCaoThitExists(string id)
        {
            return (_context.MaQuyCachCaoThit?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
