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
    public class T_DonHangController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_DonHangController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_DonHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_DonHang>>> Gets()
        {
          if (_context.T_DonHang == null)
          {
              return NotFound();
          }
            return Vm.VmT_DonHang.Items;
        }

        // GET: api/T_DonHang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_DonHang>> Get(string id)
        {
          if (_context.T_DonHang == null)
          {
              return NotFound();
          }
            var t_DonHang = Vm.VmT_DonHang.Find(id);

            if (t_DonHang == null)
            {
                return NotFound();
            }

            return t_DonHang;
        }

        // PUT: api/T_DonHang/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_DonHang t_DonHang)
        {
            if (id != t_DonHang.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_DonHang.Exists(t_DonHang))
            {
                return NotFound();
            }
            Vm.VmT_DonHang.Update_Command.Execute(t_DonHang);
            return NoContent();
        }

        // POST: api/T_DonHang
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_DonHang>> Post(T_DonHang t_DonHang)
        {
          if (_context.T_DonHang == null)
          {
              return Problem("Entity set 'dbPMScontext.T_DonHang'  is null.");
          }
            if (Vm.VmT_DonHang.Exists(t_DonHang))
            {
                return Conflict();
            }
            Vm.VmT_DonHang.Insert_Command.Execute(t_DonHang);
            return CreatedAtAction("Get", new { id = t_DonHang.Ma }, t_DonHang);
        }

        // DELETE: api/T_DonHang/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_DonHang == null)
            {
                return NotFound();
            }
            var t_DonHang = Vm.VmT_DonHang.Find(id);
            if (t_DonHang == null)
            {
                return NotFound();
            }

            Vm.VmT_DonHang.Delete_Command.Execute(t_DonHang);

            return NoContent();
        }

        private bool T_DonHangExists(string id)
        {
            return (_context.T_DonHang?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
