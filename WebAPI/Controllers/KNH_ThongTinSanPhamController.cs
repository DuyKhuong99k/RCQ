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
    public class KNH_ThongTinSanPhamController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KNH_ThongTinSanPhamController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KNH_ThongTinSanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KNH_ThongTinSanPham>>> Get()
        {
          if (_context.KNH_ThongTinSanPham == null)
          {
              return NotFound();
          }
            return Vm.VmKNH_ThongTinSanPham.Items;
        }

        // GET: api/KNH_ThongTinSanPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KNH_ThongTinSanPham>> Get(string id)
        {
          if (_context.KNH_ThongTinSanPham == null)
          {
              return NotFound();
          }
            var kNH_ThongTinSanPham = Vm.VmKNH_ThongTinSanPham.Find(id);

            if (kNH_ThongTinSanPham == null)
            {
                return NotFound();
            }

            return kNH_ThongTinSanPham;
        }

        // PUT: api/KNH_ThongTinSanPham/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KNH_ThongTinSanPham kNH_ThongTinSanPham)
        {
            if (id != kNH_ThongTinSanPham.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKNH_ThongTinSanPham.Exists(kNH_ThongTinSanPham))
            {
                return NotFound();
            }
            Vm.VmKNH_ThongTinSanPham.Update_Command.Execute(kNH_ThongTinSanPham);
            return NoContent();
        }

        // POST: api/KNH_ThongTinSanPham
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KNH_ThongTinSanPham>> Post(KNH_ThongTinSanPham kNH_ThongTinSanPham)
        {
          if (_context.KNH_ThongTinSanPham == null)
          {
              return Problem("Entity set 'dbPMScontext.KNH_ThongTinSanPham'  is null.");
          }
            if (Vm.VmKNH_ThongTinSanPham.Exists(kNH_ThongTinSanPham))
            {
                return Conflict();
            }
            Vm.VmKNH_ThongTinSanPham.Insert_Command.Execute(kNH_ThongTinSanPham);
            return CreatedAtAction("Get", new { id = kNH_ThongTinSanPham.Ma }, kNH_ThongTinSanPham);
        }

        // DELETE: api/KNH_ThongTinSanPham/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.KNH_ThongTinSanPham == null)
            {
                return NotFound();
            }
            var kNH_ThongTinSanPham = Vm.VmKNH_ThongTinSanPham.Find(id);
            if (kNH_ThongTinSanPham == null)
            {
                return NotFound();
            }

            Vm.VmKNH_ThongTinSanPham.Delete_Command.Execute(kNH_ThongTinSanPham);

            return NoContent();
        }

        private bool KNH_ThongTinSanPhamExists(string id)
        {
            return (_context.KNH_ThongTinSanPham?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
