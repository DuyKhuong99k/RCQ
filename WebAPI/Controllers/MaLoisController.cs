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
    public class MaLoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoisController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoi>>> Gets()
        {
          if (_context.MaLoi == null)
          {
              return NotFound();
          }
            return Vm.VmLoi.Items;
        }

        // GET: api/MaLois/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoi>> Get(string id)
        {
          if (_context.MaLoi == null)
          {
              return NotFound();
          }
            var maLoi = Vm.VmLoi.Find(id);

            if (maLoi == null)
            {
                return NotFound();
            }

            return maLoi;
        }

        // PUT: api/MaLois/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaLoi maLoi)
        {
            if (id != maLoi.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmLoi.Exists(maLoi))
            {
                return NotFound();
            }
            Vm.VmLoi.Update_Command.Execute(maLoi);
            return NoContent();
        }

        // POST: api/MaLois
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoi>> Post(MaLoi maLoi)
        {
          if (_context.MaLoi == null)
          {
              return Problem("Entity set 'dbPMScontext.MaLoi'  is null.");
          }
            if (Vm.VmLoi.Exists(maLoi))
            {
                return Conflict();
            }
            Vm.VmLoi.Insert_Command.Execute(maLoi);
            return CreatedAtAction("Get", new { id = maLoi.Ma }, maLoi);
        }

        // DELETE: api/MaLois/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoi == null)
            {
                return NotFound();
            }
            var maLoi = Vm.VmLoi.Find(id);
            if (maLoi == null)
            {
                return NotFound();
            }

            Vm.VmLoi.Delete_Command.Execute(maLoi);

            return NoContent();
        }

        private bool MaLoiExists(string id)
        {
            return (_context.MaLoi?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
