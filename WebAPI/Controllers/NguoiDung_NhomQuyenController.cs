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
    public class NguoiDung_NhomQuyenController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NguoiDung_NhomQuyenController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NguoiDung_NhomQuyen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDung_NhomQuyen>>> Gets()
        {
          if (_context.NguoiDung_NhomQuyen == null)
          {
              return NotFound();
          }
            return Vm.VmNguoiDungNhomQuyen.Items;
        }

        // GET: api/NguoiDung_NhomQuyen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NguoiDung_NhomQuyen>> Get(string id)
        {
          if (_context.NguoiDung_NhomQuyen == null)
          {
              return NotFound();
          }
            var nguoiDung_NhomQuyen = Vm.VmNguoiDungNhomQuyen.Find(id);

            if (nguoiDung_NhomQuyen == null)
            {
                return NotFound();
            }

            return nguoiDung_NhomQuyen;
        }

        // PUT: api/NguoiDung_NhomQuyen/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NguoiDung_NhomQuyen nguoiDung_NhomQuyen)
        {
            if (id != nguoiDung_NhomQuyen.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmNguoiDungNhomQuyen.Exists(nguoiDung_NhomQuyen))
            {
                return NotFound();
            }
            Vm.VmNguoiDungNhomQuyen.Update_Command.Execute(nguoiDung_NhomQuyen);
            return NoContent();
        }

        // POST: api/NguoiDung_NhomQuyen
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NguoiDung_NhomQuyen>> Post(NguoiDung_NhomQuyen nguoiDung_NhomQuyen)
        {
          if (_context.NguoiDung_NhomQuyen == null)
          {
              return Problem("Entity set 'dbPMScontext.NguoiDung_NhomQuyen'  is null.");
          }
            if (Vm.VmNguoiDungNhomQuyen.Exists(nguoiDung_NhomQuyen))
            {
                return Conflict();
            }
            Vm.VmNguoiDungNhomQuyen.Insert_Command.Execute(nguoiDung_NhomQuyen);
            return CreatedAtAction("Get", new { id = nguoiDung_NhomQuyen.Ma }, nguoiDung_NhomQuyen);
        }

        // DELETE: api/NguoiDung_NhomQuyen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NguoiDung_NhomQuyen == null)
            {
                return NotFound();
            }
            var nguoiDung_NhomQuyen = Vm.VmNguoiDungNhomQuyen.Find(id);
            if (nguoiDung_NhomQuyen == null)
            {
                return NotFound();
            }

            Vm.VmNguoiDungNhomQuyen.Delete_Command.Execute(nguoiDung_NhomQuyen);

            return NoContent();
        }

        private bool NguoiDung_NhomQuyenExists(string id)
        {
            return (_context.NguoiDung_NhomQuyen?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
