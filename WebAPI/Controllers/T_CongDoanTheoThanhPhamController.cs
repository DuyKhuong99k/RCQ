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
    public class T_CongDoanTheoThanhPhamController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_CongDoanTheoThanhPhamController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_CongDoanTheoThanhPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_CongDoanTheoThanhPham>>> Gets()
        {
          if (_context.T_CongDoanTheoThanhPham == null)
          {
              return NotFound();
          }
            return Vm.VmT_CongDoanTheoThanhPham.Items;
        }

        // GET: api/T_CongDoanTheoThanhPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_CongDoanTheoThanhPham>> Get(string id)
        {
          if (_context.T_CongDoanTheoThanhPham == null)
          {
              return NotFound();
          }
            var t_CongDoanTheoThanhPham = Vm.VmT_CongDoanTheoThanhPham.Find(id);

            if (t_CongDoanTheoThanhPham == null)
            {
                return NotFound();
            }

            return t_CongDoanTheoThanhPham;
        }

        // PUT: api/T_CongDoanTheoThanhPham/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_CongDoanTheoThanhPham t_CongDoanTheoThanhPham)
        {
            if (id != t_CongDoanTheoThanhPham.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_CongDoanTheoThanhPham.Exists(t_CongDoanTheoThanhPham))
            {
                return NotFound();
            }
            Vm.VmT_CongDoanTheoThanhPham.Update_Command.Execute(t_CongDoanTheoThanhPham);
            return NoContent();
        }

        // POST: api/T_CongDoanTheoThanhPham
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_CongDoanTheoThanhPham>> Post(T_CongDoanTheoThanhPham t_CongDoanTheoThanhPham)
        {
          if (_context.T_CongDoanTheoThanhPham == null)
          {
              return Problem("Entity set 'dbPMScontext.T_CongDoanTheoThanhPham'  is null.");
          }
            if (Vm.VmT_CongDoanTheoThanhPham.Exists(t_CongDoanTheoThanhPham))
            {
                return Conflict();
            }
            Vm.VmT_CongDoanTheoThanhPham.Insert_Command.Execute(t_CongDoanTheoThanhPham);
            return CreatedAtAction("Get", new { id = t_CongDoanTheoThanhPham.Ma }, t_CongDoanTheoThanhPham);
        }

        // DELETE: api/T_CongDoanTheoThanhPham/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_CongDoanTheoThanhPham == null)
            {
                return NotFound();
            }
            var t_CongDoanTheoThanhPham = Vm.VmT_CongDoanTheoThanhPham.Find(id);
            if (t_CongDoanTheoThanhPham == null)
            {
                return NotFound();
            }

            Vm.VmT_CongDoanTheoThanhPham.Delete_Command.Execute(t_CongDoanTheoThanhPham);

            return NoContent();
        }

        private bool T_CongDoanTheoThanhPhamExists(string id)
        {
            return (_context.T_CongDoanTheoThanhPham?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
