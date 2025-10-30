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
    public class T_KhachHangController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_KhachHangController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_KhachHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_KhachHang>>> Gets()
        {
          if (_context.T_KhachHang == null)
          {
              return NotFound();
          }
            return Vm.VmT_KhachHang.Items;
        }

        // GET: api/T_KhachHang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_KhachHang>> Get(string id)
        {
          if (_context.T_KhachHang == null)
          {
              return NotFound();
          }
            var t_KhachHang = Vm.VmT_KhachHang.Find(id);

            if (t_KhachHang == null)
            {
                return NotFound();
            }

            return t_KhachHang;
        }

        // PUT: api/T_KhachHang/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_KhachHang t_KhachHang)
        {
            if (id != t_KhachHang.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_KhachHang.Exists(t_KhachHang))
            {
                return NotFound();
            }
            Vm.VmT_KhachHang.Update_Command.Execute(t_KhachHang);
            return NoContent();
        }

        // POST: api/T_KhachHang
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_KhachHang>> Post(T_KhachHang t_KhachHang)
        {
          if (_context.T_KhachHang == null)
          {
              return Problem("Entity set 'dbPMScontext.T_KhachHang'  is null.");
          }
            if (Vm.VmT_KhachHang.Exists(t_KhachHang))
            {
                return Conflict();
            }
            Vm.VmT_KhachHang.Insert_Command.Execute(t_KhachHang);
            return CreatedAtAction("Get", new { id = t_KhachHang.Ma }, t_KhachHang);
        }

        // DELETE: api/T_KhachHang/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_KhachHang == null)
            {
                return NotFound();
            }
            var t_KhachHang = Vm.VmT_KhachHang.Find(id);
            if (t_KhachHang == null)
            {
                return NotFound();
            }

            Vm.VmT_KhachHang.Delete_Command.Execute(t_KhachHang);

            return NoContent();
        }

        private bool T_KhachHangExists(string id)
        {
            return (_context.T_KhachHang?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
