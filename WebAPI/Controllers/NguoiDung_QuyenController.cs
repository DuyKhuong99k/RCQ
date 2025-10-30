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
    public class NguoiDung_QuyenController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NguoiDung_QuyenController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NguoiDung_Quyen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDung_Quyen>>> Gets()
        {
          if (_context.NguoiDung_Quyen == null)
          {
              return NotFound();
          }
            return Vm.VmNguoiDung_Quyen.Items;
        }

        // GET: api/NguoiDung_Quyen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NguoiDung_Quyen>> Get(string id)
        {
          if (_context.NguoiDung_Quyen == null)
          {
              return NotFound();
          }
            var nguoiDung_Quyen = Vm.VmNguoiDung_Quyen.Find(id);

            if (nguoiDung_Quyen == null)
            {
                return NotFound();
            }

            return nguoiDung_Quyen;
        }

        // PUT: api/NguoiDung_Quyen/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NguoiDung_Quyen nguoiDung_Quyen)
        {
            if (id != nguoiDung_Quyen.TenNguoiDung)
            {
                return BadRequest();
            }

            if (!Vm.VmNguoiDung_Quyen.Exists(nguoiDung_Quyen))
            {
                return NotFound();
            }
            Vm.VmNguoiDung_Quyen.Update_Command.Execute(nguoiDung_Quyen);
            return NoContent();
        }

        // POST: api/NguoiDung_Quyen
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NguoiDung_Quyen>> Post(NguoiDung_Quyen nguoiDung_Quyen)
        {
          if (_context.NguoiDung_Quyen == null)
          {
              return Problem("Entity set 'dbPMScontext.NguoiDung_Quyen'  is null.");
          }
            if (Vm.VmNguoiDung_Quyen.Exists(nguoiDung_Quyen))
            {
                return Conflict();
            }
            Vm.VmNguoiDung_Quyen.Insert_Command.Execute(nguoiDung_Quyen);
            return CreatedAtAction("Get", new { id = nguoiDung_Quyen.TenNguoiDung }, nguoiDung_Quyen);
        }

        // DELETE: api/NguoiDung_Quyen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NguoiDung_Quyen == null)
            {
                return NotFound();
            }
            var nguoiDung_Quyen = Vm.VmNguoiDung_Quyen.Find(id);
            if (nguoiDung_Quyen == null)
            {
                return NotFound();
            }

            Vm.VmNguoiDung_Quyen.Delete_Command.Execute(nguoiDung_Quyen);

            return NoContent();
        }

        private bool NguoiDung_QuyenExists(string id)
        {
            return (_context.NguoiDung_Quyen?.Any(e => e.TenNguoiDung == id)).GetValueOrDefault();
        }
    }
}
