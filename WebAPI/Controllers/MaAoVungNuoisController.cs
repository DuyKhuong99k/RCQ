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
    public class MaAoVungNuoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaAoVungNuoisController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaAoVungNuois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaAoVungNuoi>>> Gets()
        {
          if (_context.MaAoVungNuoi == null)
          {
              return NotFound();
          }
            return Vm.VmAoVungNuoi.Items;
        }

        // GET: api/MaAoVungNuois/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaAoVungNuoi>> Get(string id)
        {
          if (_context.MaAoVungNuoi == null)
          {
              return NotFound();
          }
            var maAoVungNuoi = Vm.VmAoVungNuoi.Find(id);

            if (maAoVungNuoi == null)
            {
                return NotFound();
            }

            return maAoVungNuoi;
        }

        // PUT: api/MaAoVungNuois/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaAoVungNuoi maAoVungNuoi)
        {
            if (id != maAoVungNuoi.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmAoVungNuoi.Exists(maAoVungNuoi))
            {
                return NotFound();
            }
            Vm.VmAoVungNuoi.Update_Command.Execute(maAoVungNuoi);
            return NoContent();
        }

        // POST: api/MaAoVungNuois
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaAoVungNuoi>> Post(MaAoVungNuoi maAoVungNuoi)
        {
          if (_context.MaAoVungNuoi == null)
          {
              return Problem("Entity set 'dbPMScontext.MaAoVungNuoi'  is null.");
          }
            if (Vm.VmAoVungNuoi.Exists(maAoVungNuoi))
            {
                return Conflict();
            }
            Vm.VmAoVungNuoi.Insert_Command.Execute(maAoVungNuoi);
            return CreatedAtAction("GetMaAoVungNuoi", new { id = maAoVungNuoi.Ma }, maAoVungNuoi);
        }

        // DELETE: api/MaAoVungNuois/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaAoVungNuoi == null)
            {
                return NotFound();
            }
            var maAoVungNuoi = Vm.VmAoVungNuoi.Find(id);
            if (maAoVungNuoi == null)
            {
                return NotFound();
            }

            Vm.VmAoVungNuoi.Delete_Command.Execute(maAoVungNuoi);

            return NoContent();
        }

        private bool MaAoVungNuoiExists(string id)
        {
            return (_context.MaAoVungNuoi?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
