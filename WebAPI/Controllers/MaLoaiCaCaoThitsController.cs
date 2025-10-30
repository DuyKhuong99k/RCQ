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
    public class MaLoaiCaCaoThitsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoaiCaCaoThitsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLoaiCaCaoThits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaCaoThit>>> Gets()
        {
          if (_context.MaLoaiCaCaoThit == null)
          {
              return NotFound();
          }
            return Vm.VmLoaiCaCaoThit.Items;
        }

        // GET: api/MaLoaiCaCaoThits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaCaoThit>> Get(string id)
        {
          if (_context.MaLoaiCaCaoThit == null)
          {
              return NotFound();
          }
            var maLoaiCaCaoThit = Vm.VmLoaiCaCaoThit.Find(id);

            if (maLoaiCaCaoThit == null)
            {
                return NotFound();
            }

            return maLoaiCaCaoThit;
        }

        // PUT: api/MaLoaiCaCaoThits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaLoaiCaCaoThit maLoaiCaCaoThit)
        {
            if (id != maLoaiCaCaoThit.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmLoaiCaCaoThit.Exists(maLoaiCaCaoThit))
            {
                return NotFound();
            }
            Vm.VmLoaiCaCaoThit.Update_Command.Execute(maLoaiCaCaoThit);
            return NoContent();
        }

        // POST: api/MaLoaiCaCaoThits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaCaoThit>> Post(MaLoaiCaCaoThit maLoaiCaCaoThit)
        {
          if (_context.MaLoaiCaCaoThit == null)
          {
              return Problem("Entity set 'dbPMScontext.MaLoaiCaCaoThit'  is null.");
          }
            if (Vm.VmLoaiCaCaoThit.Exists(maLoaiCaCaoThit))
            {
                return Conflict();
            }

            return CreatedAtAction("Get", new { id = maLoaiCaCaoThit.Ma }, maLoaiCaCaoThit);
        }

        // DELETE: api/MaLoaiCaCaoThits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoaiCaCaoThit == null)
            {
                return NotFound();
            }
            var maLoaiCaCaoThit = Vm.VmLoaiCaCaoThit.Find(id);
            if (maLoaiCaCaoThit == null)
            {
                return NotFound();
            }

            Vm.VmLoaiCaCaoThit.Delete_Command.Execute(maLoaiCaCaoThit);

            return NoContent();
        }

        private bool MaLoaiCaCaoThitExists(string id)
        {
            return (_context.MaLoaiCaCaoThit?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
