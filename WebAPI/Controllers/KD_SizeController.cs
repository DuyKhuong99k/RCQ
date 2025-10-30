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
    public class KD_SizeController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KD_SizeController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KD_Size
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KD_Size>>> Gets()
        {
          if (_context.KD_Size == null)
          {
              return NotFound();
          }
            return Vm.VmKD_Size.Items;
        }

        // GET: api/KD_Size/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KD_Size>> Get(string id)
        {
          if (_context.KD_Size == null)
          {
              return NotFound();
          }
            var kD_Size = Vm.VmKD_Size.Find(id);

            if (kD_Size == null)
            {
                return NotFound();
            }

            return kD_Size;
        }

        // PUT: api/KD_Size/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KD_Size kD_Size)
        {
            if (id != kD_Size.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKD_Size.Exists(kD_Size))
            {
                return NotFound();
            }
            Vm.VmKD_Size.Update_Command.Execute(kD_Size);
            return NoContent();
        }

        // POST: api/KD_Size
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KD_Size>> Post(KD_Size kD_Size)
        {
          if (_context.KD_Size == null)
          {
              return Problem("Entity set 'dbPMScontext.KD_Size'  is null.");
          }
            if (Vm.VmKD_Size.Exists(kD_Size))
            {
                return Conflict();
            }
            Vm.VmKD_Size.Insert_Command.Execute(kD_Size);
            return CreatedAtAction("Get", new { id = kD_Size.Ma }, kD_Size);
        }

        // DELETE: api/KD_Size/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.KD_Size == null)
            {
                return NotFound();
            }
            var kD_Size = Vm.VmKD_Size.Find(id);
            if (kD_Size == null)
            {
                return NotFound();
            }

            Vm.VmKD_Size.Delete_Command.Execute(kD_Size);

            return NoContent();
        }

        private bool KD_SizeExists(string id)
        {
            return (_context.KD_Size?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
