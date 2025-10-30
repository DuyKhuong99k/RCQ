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
    public class KNH_ThanhPhamController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KNH_ThanhPhamController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KNH_ThanhPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KNH_ThanhPham>>> Gets()
        {
          if (_context.KNH_ThanhPham == null)
          {
              return NotFound();
          }
            return Vm.VmKNH_ThanhPham.Items;
        }

        // GET: api/KNH_ThanhPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KNH_ThanhPham>> Get(string id)
        {
          if (_context.KNH_ThanhPham == null)
          {
              return NotFound();
          }
            var kNH_ThanhPham = Vm.VmKNH_ThanhPham.Find(id);

            if (kNH_ThanhPham == null)
            {
                return NotFound();
            }

            return kNH_ThanhPham;
        }

        // PUT: api/KNH_ThanhPham/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KNH_ThanhPham kNH_ThanhPham)
        {
            if (id != kNH_ThanhPham.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKNH_ThanhPham.Exists(kNH_ThanhPham))
            {
                return NotFound();
            }
            Vm.VmKNH_ThanhPham.Update_Command.Execute(kNH_ThanhPham);
            return NoContent();
        }

        // POST: api/KNH_ThanhPham
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KNH_ThanhPham>> Post(KNH_ThanhPham kNH_ThanhPham)
        {
          if (_context.KNH_ThanhPham == null)
          {
              return Problem("Entity set 'dbPMScontext.KNH_ThanhPham'  is null.");
          }
            if (Vm.VmKNH_ThanhPham.Exists(kNH_ThanhPham))
            {
                return Conflict();
            }
            Vm.VmKNH_ThanhPham.Insert_Command.Execute(kNH_ThanhPham);
            return CreatedAtAction("Get", new { id = kNH_ThanhPham.Ma }, kNH_ThanhPham);
        }

        // DELETE: api/KNH_ThanhPham/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.KNH_ThanhPham == null)
            {
                return NotFound();
            }
            var kNH_ThanhPham = Vm.VmKNH_ThanhPham.Find(id);
            if (kNH_ThanhPham == null)
            {
                return NotFound();
            }

            Vm.VmKNH_ThanhPham.Delete_Command.Execute(kNH_ThanhPham);

            return NoContent();
        }

        private bool KNH_ThanhPhamExists(string id)
        {
            return (_context.KNH_ThanhPham?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
