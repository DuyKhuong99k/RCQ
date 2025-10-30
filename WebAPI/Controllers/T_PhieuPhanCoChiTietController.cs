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
    public class T_PhieuPhanCoChiTietController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_PhieuPhanCoChiTietController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_PhieuPhanCoChiTiet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_PhieuPhanCoChiTiet>>> Gets()
        {
          if (_context.T_PhieuPhanCoChiTiet == null)
          {
              return NotFound();
          }
            return Vm.VmT_PhieuPhanCoChiTiet.Items;
        }

        // GET: api/T_PhieuPhanCoChiTiet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_PhieuPhanCoChiTiet>> Get(int id)
        {
          if (_context.T_PhieuPhanCoChiTiet == null)
          {
              return NotFound();
          }
            var t_PhieuPhanCoChiTiet = Vm.VmT_PhieuPhanCoChiTiet.Find(id);

            if (t_PhieuPhanCoChiTiet == null)
            {
                return NotFound();
            }

            return t_PhieuPhanCoChiTiet;
        }

        // PUT: api/T_PhieuPhanCoChiTiet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, T_PhieuPhanCoChiTiet t_PhieuPhanCoChiTiet)
        {
            if (id != t_PhieuPhanCoChiTiet.STT)
            {
                return BadRequest();
            }

            if (!Vm.VmT_PhieuPhanCoChiTiet.Exists(t_PhieuPhanCoChiTiet))
            {
                return NotFound();
            }
            Vm.VmT_PhieuPhanCoChiTiet.Update_Command.Execute(t_PhieuPhanCoChiTiet);
            return NoContent();
        }

        // POST: api/T_PhieuPhanCoChiTiet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_PhieuPhanCoChiTiet>> Post(T_PhieuPhanCoChiTiet t_PhieuPhanCoChiTiet)
        {
          if (_context.T_PhieuPhanCoChiTiet == null)
          {
              return Problem("Entity set 'dbPMScontext.T_PhieuPhanCoChiTiet'  is null.");
          }
            if (Vm.VmT_PhieuPhanCoChiTiet.Exists(t_PhieuPhanCoChiTiet))
            {
                return Conflict();
            }
            Vm.VmT_PhieuPhanCoChiTiet.Insert_Command.Execute(t_PhieuPhanCoChiTiet);
            return CreatedAtAction("Get", new { id = t_PhieuPhanCoChiTiet.STT }, t_PhieuPhanCoChiTiet);
        }

        // DELETE: api/T_PhieuPhanCoChiTiet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.T_PhieuPhanCoChiTiet == null)
            {
                return NotFound();
            }
            var t_PhieuPhanCoChiTiet = Vm.VmT_PhieuPhanCoChiTiet.Find(id);
            if (t_PhieuPhanCoChiTiet == null)
            {
                return NotFound();
            }

            Vm.VmT_PhieuPhanCoChiTiet.Delete_Command.Execute(t_PhieuPhanCoChiTiet);

            return NoContent();
        }

        private bool T_PhieuPhanCoChiTietExists(int id)
        {
            return (_context.T_PhieuPhanCoChiTiet?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
