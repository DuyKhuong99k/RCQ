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
    public class KD_ThanhPhamController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KD_ThanhPhamController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KD_ThanhPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KD_ThanhPham>>> Gets()
        {
          if (_context.KD_ThanhPham == null)
          {
              return NotFound();
          }
            return Vm.VmKD_ThanhPham.Items;
        }

        // GET: api/KD_ThanhPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KD_ThanhPham>> Get(string id)
        {
          if (_context.KD_ThanhPham == null)
          {
              return NotFound();
          }
            var kD_ThanhPham = Vm.VmKD_ThanhPham.Find(id);

            if (kD_ThanhPham == null)
            {
                return NotFound();
            }

            return kD_ThanhPham;
        }

        // PUT: api/KD_ThanhPham/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KD_ThanhPham kD_ThanhPham)
        {
            if (id != kD_ThanhPham.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKD_ThanhPham.Exists(kD_ThanhPham))
            {
                return NotFound();
            }
            Vm.VmKD_ThanhPham.Update_Command.Execute(kD_ThanhPham);
            return NoContent();
        }

        // POST: api/KD_ThanhPham
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KD_ThanhPham>> Post(KD_ThanhPham kD_ThanhPham)
        {
          if (_context.KD_ThanhPham == null)
          {
              return Problem("Entity set 'dbPMScontext.KD_ThanhPham'  is null.");
          }
            if (Vm.VmKD_ThanhPham.Exists(kD_ThanhPham))
            {
                return Conflict();
            }
            Vm.VmKD_ThanhPham.Insert_Command.Execute(kD_ThanhPham);
            return CreatedAtAction("Get", new { id = kD_ThanhPham.Ma }, kD_ThanhPham);
        }

        // DELETE: api/KD_ThanhPham/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.KD_ThanhPham == null)
            {
                return NotFound();
            }
            var kD_ThanhPham = Vm.VmKD_ThanhPham.Find(id);
            if (kD_ThanhPham == null)
            {
                return NotFound();
            }

            Vm.VmKD_ThanhPham.Delete_Command.Execute(kD_ThanhPham);

            return NoContent();
        }

        private bool KD_ThanhPhamExists(string id)
        {
            return (_context.KD_ThanhPham?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
