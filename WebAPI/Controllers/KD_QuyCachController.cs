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
    public class KD_QuyCachController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KD_QuyCachController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KD_QuyCach
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KD_QuyCach>>> Gets()
        {
          if (_context.KD_QuyCach == null)
          {
              return NotFound();
          }
            return Vm.VmKD_QuyCach.Items;
        }

        // GET: api/KD_QuyCach/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KD_QuyCach>> Get(string id)
        {
          if (_context.KD_QuyCach == null)
          {
              return NotFound();
          }
            var kD_QuyCach = Vm.VmKD_QuyCach.Find(id);

            if (kD_QuyCach == null)
            {
                return NotFound();
            }

            return kD_QuyCach;
        }

        // PUT: api/KD_QuyCach/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KD_QuyCach kD_QuyCach)
        {
            if (id != kD_QuyCach.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKD_QuyCach.Exists(kD_QuyCach))
            {
                return NotFound();
            }
            Vm.VmKD_QuyCach.Update_Command.Execute(kD_QuyCach);
            return NoContent();
        }

        // POST: api/KD_QuyCach
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KD_QuyCach>> Post(KD_QuyCach kD_QuyCach)
        {
          if (_context.KD_QuyCach == null)
          {
              return Problem("Entity set 'dbPMScontext.KD_QuyCach'  is null.");
          }
            if (Vm.VmKD_QuyCach.Exists(kD_QuyCach))
            {
                return Conflict();
            }
            Vm.VmKD_QuyCach.Insert_Command.Execute(kD_QuyCach);
            return CreatedAtAction("Get", new { id = kD_QuyCach.Ma }, kD_QuyCach);
        }

        // DELETE: api/KD_QuyCach/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.KD_QuyCach == null)
            {
                return NotFound();
            }
            var kD_QuyCach = Vm.VmKD_QuyCach.Find(id);
            if (kD_QuyCach == null)
            {
                return NotFound();
            }

            Vm.VmKD_QuyCach.Delete_Command.Execute(kD_QuyCach);

            return NoContent();
        }

        private bool KD_QuyCachExists(string id)
        {
            return (_context.KD_QuyCach?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
