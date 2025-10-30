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
    public class KNH_SizeController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KNH_SizeController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KNH_Size
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KNH_Size>>> Gets()
        {
          if (_context.KNH_Size == null)
          {
              return NotFound();
          }
            return Vm.VmKNH_Size.Items;
        }

        // GET: api/KNH_Size/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KNH_Size>> Get(string id)
        {
          if (_context.KNH_Size == null)
          {
              return NotFound();
          }
            var kNH_Size = Vm.VmKNH_Size.Find(id);

            if (kNH_Size == null)
            {
                return NotFound();
            }

            return kNH_Size;
        }

        // PUT: api/KNH_Size/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KNH_Size kNH_Size)
        {
            if (id != kNH_Size.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKNH_Size.Exists(kNH_Size))
            {
                return NotFound();
            }
            Vm.VmKNH_Size.Update_Command.Execute(kNH_Size);
            return NoContent();
        }

        // POST: api/KNH_Size
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KNH_Size>> Post(KNH_Size kNH_Size)
        {
          if (_context.KNH_Size == null)
          {
              return Problem("Entity set 'dbPMScontext.KNH_Size'  is null.");
          }
            if (Vm.VmKNH_Size.Exists(kNH_Size))
            {
                return Conflict();
            }
            Vm.VmKNH_Size.Insert_Command.Execute(kNH_Size);
            return CreatedAtAction("Get", new { id = kNH_Size.Ma }, kNH_Size);
        }

        // DELETE: api/KNH_Size/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.KNH_Size == null)
            {
                return NotFound();
            }
            var kNH_Size = Vm.VmKNH_Size.Find(id);
            if (kNH_Size == null)
            {
                return NotFound();
            }

            Vm.VmKNH_Size.Delete_Command.Execute(kNH_Size);

            return NoContent();
        }

        private bool KNH_SizeExists(string id)
        {
            return (_context.KNH_Size?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
