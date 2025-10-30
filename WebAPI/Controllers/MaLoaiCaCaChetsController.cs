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
    public class MaLoaiCaCaChetsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoaiCaCaChetsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLoaiCaCaChets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaCaChet>>> Gets()
        {
          if (_context.MaLoaiCaCaChet == null)
          {
              return NotFound();
          }
            return Vm.VmLoaiCaCaChet.Items;
        }

        // GET: api/MaLoaiCaCaChets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaCaChet>> Get(string id)
        {
          if (_context.MaLoaiCaCaChet == null)
          {
              return NotFound();
          }
            var maLoaiCaCaChet = Vm.VmLoaiCaCaChet.Find(id);

            if (maLoaiCaCaChet == null)
            {
                return NotFound();
            }

            return maLoaiCaCaChet;
        }

        // PUT: api/MaLoaiCaCaChets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaLoaiCaCaChet(string id, MaLoaiCaCaChet maLoaiCaCaChet)
        {
            if (id != maLoaiCaCaChet.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmLoaiCaCaChet.Exists(maLoaiCaCaChet))
            {
                return NotFound();
            }
            Vm.VmLoaiCaCaChet.Update_Command.Execute(maLoaiCaCaChet);
            return NoContent();
        }

        // POST: api/MaLoaiCaCaChets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaCaChet>> Post(MaLoaiCaCaChet maLoaiCaCaChet)
        {
          if (_context.MaLoaiCaCaChet == null)
          {
              return Problem("Entity set 'dbPMScontext.MaLoaiCaCaChet'  is null.");
          }
            if (Vm.VmLoaiCaCaChet.Exists(maLoaiCaCaChet))
            {
                return Conflict();
            }
            Vm.VmLoaiCaCaChet.Insert_Command.Execute(maLoaiCaCaChet);
            return CreatedAtAction("Get", new { id = maLoaiCaCaChet.Ma }, maLoaiCaCaChet);
        }

        // DELETE: api/MaLoaiCaCaChets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoaiCaCaChet == null)
            {
                return NotFound();
            }
            var maLoaiCaCaChet = Vm.VmLoaiCaCaChet.Find(id);
            if (maLoaiCaCaChet == null)
            {
                return NotFound();
            }

            Vm.VmLoaiCaCaChet.Delete_Command.Execute(maLoaiCaCaChet);
            return NoContent();
        }

        private bool MaLoaiCaCaChetExists(string id)
        {
            return (_context.MaLoaiCaCaChet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
