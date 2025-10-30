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
    public class TheTusController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public TheTusController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/TheTus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheTu>>> Gets()
        {
          if (_context.TheTu == null)
          {
              return NotFound();
          }
            return Vm.VmThe.Items;
        }

        // GET: api/TheTus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TheTu>> Get(string id)
        {
          if (_context.TheTu == null)
          {
              return NotFound();
          }
            var theTu = Vm.VmThe.Find(id);

            if (theTu == null)
            {
                return NotFound();
            }

            return theTu;
        }

        // PUT: api/TheTus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, TheTu theTu)
        {
            if (id != theTu.MaTheTu)
            {
                return BadRequest();
            }

            if (!Vm.VmThe.Exists(theTu))
            {
                return NotFound();
            }
            Vm.VmThe.Update_Command.Execute(theTu);
            return NoContent();
        }

        // POST: api/TheTus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TheTu>> Post(TheTu theTu)
        {
          if (_context.TheTu == null)
          {
              return Problem("Entity set 'dbPMScontext.TheTu'  is null.");
          }
            if (Vm.VmThe.Exists(theTu))
            {
                return Conflict();
            }
            Vm.VmThe.Insert_Command.Execute(theTu);
            return CreatedAtAction("Get", new { id = theTu.MaTheTu }, theTu);
        }

        // DELETE: api/TheTus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.TheTu == null)
            {
                return NotFound();
            }
            var theTu = Vm.VmThe.Find(id);
            if (theTu == null)
            {
                return NotFound();
            }

            Vm.VmThe.Delete_Command.Execute(theTu);

            return NoContent();
        }

        private bool TheTuExists(string id)
        {
            return (_context.TheTu?.Any(e => e.MaTheTu == id)).GetValueOrDefault();
        }
    }
}
