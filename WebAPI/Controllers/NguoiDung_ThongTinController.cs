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
    public class NguoiDung_ThongTinController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NguoiDung_ThongTinController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NguoiDung_ThongTin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDung_ThongTin>>> Gets()
        {
          if (_context.NguoiDung_ThongTin == null)
          {
              return NotFound();
          }
            return Vm.VmNguoiDungThongTin.Items;
        }

        // GET: api/NguoiDung_ThongTin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NguoiDung_ThongTin>> Get(string id)
        {
          if (_context.NguoiDung_ThongTin == null)
          {
              return NotFound();
          }
            var nguoiDung_ThongTin = Vm.VmNguoiDungThongTin.Find(id);

            if (nguoiDung_ThongTin == null)
            {
                return NotFound();
            }

            return nguoiDung_ThongTin;
        }

        // PUT: api/NguoiDung_ThongTin/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NguoiDung_ThongTin nguoiDung_ThongTin)
        {
            if (id != nguoiDung_ThongTin.MaNhanVien)
            {
                return BadRequest();
            }

            if (!Vm.VmNguoiDungThongTin.Exists(nguoiDung_ThongTin))
            {
                return NotFound();
            }
            Vm.VmNguoiDungThongTin.Update_Command.Execute(nguoiDung_ThongTin);
            return NoContent();
        }

        // POST: api/NguoiDung_ThongTin
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NguoiDung_ThongTin>> Post(NguoiDung_ThongTin nguoiDung_ThongTin)
        {
          if (_context.NguoiDung_ThongTin == null)
          {
              return Problem("Entity set 'dbPMScontext.NguoiDung_ThongTin'  is null.");
          }
            if (Vm.VmNguoiDungThongTin.Exists(nguoiDung_ThongTin))
            {
                return Conflict();
            }
            Vm.VmNguoiDungThongTin.Insert_Command.Execute(nguoiDung_ThongTin);
            return CreatedAtAction("Get", new { id = nguoiDung_ThongTin.TenNguoiDung }, nguoiDung_ThongTin);
        }

        // DELETE: api/NguoiDung_ThongTin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NguoiDung_ThongTin == null)
            {
                return NotFound();
            }
            var nguoiDung_ThongTin = Vm.VmNguoiDungThongTin.Find(id);
            if (nguoiDung_ThongTin == null)
            {
                return NotFound();
            }

            Vm.VmNguoiDungThongTin.Delete_Command.Execute(nguoiDung_ThongTin);

            return NoContent();
        }

        private bool NguoiDung_ThongTinExists(string id)
        {
            return (_context.NguoiDung_ThongTin?.Any(e => e.TenNguoiDung == id)).GetValueOrDefault();
        }
    }
}
