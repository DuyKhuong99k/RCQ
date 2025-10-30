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
    public class KNH_QuyCachController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KNH_QuyCachController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KNH_QuyCach
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KNH_QuyCach>>> Gets()
        {
          if (_context.KNH_QuyCach == null)
          {
              return NotFound();
          }
            return Vm.VmKNH_QuyCach.Items;
        }

        // GET: api/KNH_QuyCach/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KNH_QuyCach>> Get(string id)
        {
          if (_context.KNH_QuyCach == null)
          {
              return NotFound();
          }
            var kNH_QuyCach = Vm.VmKNH_QuyCach.Find(id);

            if (kNH_QuyCach == null)
            {
                return NotFound();
            }

            return kNH_QuyCach;
        }

        // PUT: api/KNH_QuyCach/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KNH_QuyCach kNH_QuyCach)
        {
            if (id != kNH_QuyCach.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKNH_QuyCach.Exists(kNH_QuyCach))
            {
                return NotFound();
            }
            Vm.VmKNH_QuyCach.Update_Command.Execute(kNH_QuyCach);
            return NoContent();
        }

        // POST: api/KNH_QuyCach
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KNH_QuyCach>> Post(KNH_QuyCach kNH_QuyCach)
        {
          if (_context.KNH_QuyCach == null)
          {
              return Problem("Entity set 'dbPMScontext.KNH_QuyCach'  is null.");
          }
            if (Vm.VmKNH_QuyCach.Exists(kNH_QuyCach))
            {
                return Conflict();
            }
            Vm.VmKNH_QuyCach.Insert_Command.Execute(kNH_QuyCach);
            return CreatedAtAction("Get", new { id = kNH_QuyCach.Ma }, kNH_QuyCach);
        }

        // DELETE: api/KNH_QuyCach/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKNH_QuyCach(string id)
        {
            if (_context.KNH_QuyCach == null)
            {
                return NotFound();
            }
            var kNH_QuyCach = await _context.KNH_QuyCach.FindAsync(id);
            if (kNH_QuyCach == null)
            {
                return NotFound();
            }

            _context.KNH_QuyCach.Remove(kNH_QuyCach);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KNH_QuyCachExists(string id)
        {
            return (_context.KNH_QuyCach?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
