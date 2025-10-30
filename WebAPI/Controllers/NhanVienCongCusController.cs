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
    public class NhanVienCongCusController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhanVienCongCusController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NhanVienCongCus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienCongCu>>> Gets()
        {
          if (_context.NhanVienCongCu == null)
          {
              return NotFound();
          }
            return Vm.VmNhanVienCongCu.Items;
        }

        // GET: api/NhanVienCongCus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienCongCu>> Get(string id)
        {
          if (_context.NhanVienCongCu == null)
          {
              return NotFound();
          }
            var nhanVienCongCu = Vm.VmNhanVienCongCu.Find(id);

            if (nhanVienCongCu == null)
            {
                return NotFound();
            }

            return nhanVienCongCu;
        }

        // PUT: api/NhanVienCongCus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NhanVienCongCu nhanVienCongCu)
        {
            if (id != nhanVienCongCu.MaNhanVien)
            {
                return BadRequest();
            }

            if (!Vm.VmNhanVienCongCu.Exists(nhanVienCongCu))
            {
                return NotFound();
            }
            Vm.VmNhanVienCongCu.Update_Command.Execute(nhanVienCongCu);
            return NoContent();
        }

        // POST: api/NhanVienCongCus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhanVienCongCu>> Post(NhanVienCongCu nhanVienCongCu)
        {
          if (_context.NhanVienCongCu == null)
          {
              return Problem("Entity set 'dbPMScontext.NhanVienCongCu'  is null.");
          }
            if (Vm.VmNhanVienCongCu.Exists(nhanVienCongCu))
            {
                return Conflict();
            }
            Vm.VmNhanVienCongCu.Insert_Command.Execute(nhanVienCongCu);
            return CreatedAtAction("Get", new { id = nhanVienCongCu.MaNhanVien }, nhanVienCongCu);
        }

        // DELETE: api/NhanVienCongCus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NhanVienCongCu == null)
            {
                return NotFound();
            }
            var nhanVienCongCu = Vm.VmNhanVienCongCu.Find(id);
            if (nhanVienCongCu == null)
            {
                return NotFound();
            }

            Vm.VmNhanVienCongCu.Delete_Command.Execute(nhanVienCongCu);

            return NoContent();
        }

        private bool NhanVienCongCuExists(DateTime id)
        {
            return (_context.NhanVienCongCu?.Any(e => e.Ngay == id)).GetValueOrDefault();
        }
    }
}
