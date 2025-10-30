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
    public class KD_TuiController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KD_TuiController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KD_Tui
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KD_Tui>>> Gets()
        {
          if (_context.KD_Tui == null)
          {
              return NotFound();
          }
            return Vm.VmKD_Tui.Items;
        }

        // GET: api/KD_Tui/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KD_Tui>> Get(string id)
        {
          if (_context.KD_Tui == null)
          {
              return NotFound();
          }
            var kD_Tui = Vm.VmKD_Tui.Find(id);

            if (kD_Tui == null)
            {
                return NotFound();
            }

            return kD_Tui;
        }

        // PUT: api/KD_Tui/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KD_Tui kD_Tui)
        {
            if (id != kD_Tui.Ma)
            {
                return BadRequest();
            }
            if (!Vm.VmKD_Tui.Exists(kD_Tui))
            {
                return NotFound();
            }
            Vm.VmKD_Tui.Update_Command.Execute(kD_Tui);

            return NoContent();
        }

        // POST: api/KD_Tui
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KD_Tui>> Post(KD_Tui kD_Tui)
        {
          if (_context.KD_Tui == null)
          {
              return Problem("Entity set 'dbPMScontext.KD_Tui'  is null.");
          }
            if (Vm.VmKD_Tui.Exists(kD_Tui))
            {
                return Conflict();
            }
            Vm.VmKD_Tui.Insert_Command.Execute(kD_Tui);
            return CreatedAtAction("Get", new { id = kD_Tui.Ma }, kD_Tui);
        }

        // DELETE: api/KD_Tui/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.KD_Tui == null)
            {
                return NotFound();
            }
            var kD_Tui = Vm.VmKD_Tui.Find(id);
            if (kD_Tui == null)
            {
                return NotFound();
            }

            Vm.VmKD_Tui.Delete_Command.Execute(kD_Tui);

            return NoContent();
        }

        private bool KD_TuiExists(string id)
        {
            return (_context.KD_Tui?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
