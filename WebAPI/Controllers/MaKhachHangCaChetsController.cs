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
    public class MaKhachHangCaChetsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaKhachHangCaChetsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaKhachHangCaChets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaKhachHangCaChet>>> Gets()
        {
          if (_context.MaKhachHangCaChet == null)
          {
              return NotFound();
          }
            return Vm.VmKhachHangCaChet.Items;
        }

        // GET: api/MaKhachHangCaChets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaKhachHangCaChet>> Get(string id)
        {
          if (_context.MaKhachHangCaChet == null)
          {
              return NotFound();
          }
            var maKhachHangCaChet = Vm.VmKhachHangCaChet.Find(id);

            if (maKhachHangCaChet == null)
            {
                return NotFound();
            }

            return maKhachHangCaChet;
        }

        // PUT: api/MaKhachHangCaChets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaKhachHangCaChet maKhachHangCaChet)
        {
            if (id != maKhachHangCaChet.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKhachHangCaChet.Exists(maKhachHangCaChet))
            {
                return NotFound();
            }
            Vm.VmKhachHangCaChet.Update_Command.Execute(maKhachHangCaChet);
            return NoContent();
        }

        // POST: api/MaKhachHangCaChets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaKhachHangCaChet>> Post(MaKhachHangCaChet maKhachHangCaChet)
        {
          if (_context.MaKhachHangCaChet == null)
          {
              return Problem("Entity set 'dbPMScontext.MaKhachHangCaChet'  is null.");
          }
            if (Vm.VmKhachHangCaChet.Exists(maKhachHangCaChet))
            {
                return Conflict();
            }
            Vm.VmKhachHangCaChet.Insert_Command.Execute(maKhachHangCaChet);
            return CreatedAtAction("Get", new { id = maKhachHangCaChet.Ma }, maKhachHangCaChet);
        }

        // DELETE: api/MaKhachHangCaChets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaKhachHangCaChet == null)
            {
                return NotFound();
            }
            var maKhachHangCaChet = Vm.VmKhachHangCaChet.Find(id);
            if (maKhachHangCaChet == null)
            {
                return NotFound();
            }

            Vm.VmKhachHangCaChet.Delete_Command.Execute(maKhachHangCaChet);

            return NoContent();
        }

        private bool MaKhachHangCaChetExists(string id)
        {
            return (_context.MaKhachHangCaChet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
