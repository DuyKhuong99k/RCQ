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
    public class T_PhuongTienController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_PhuongTienController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_PhuongTien
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_PhuongTien>>> Gets()
        {
          if (_context.T_PhuongTien == null)
          {
              return NotFound();
          }
            return Vm.VmT_PhuongTien.Items;
        }

        // GET: api/T_PhuongTien/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_PhuongTien>> Get(string id)
        {
          if (_context.T_PhuongTien == null)
          {
              return NotFound();
          }
            var t_PhuongTien = Vm.VmT_PhuongTien.Find(id);

            if (t_PhuongTien == null)
            {
                return NotFound();
            }

            return t_PhuongTien;
        }

        // PUT: api/T_PhuongTien/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_PhuongTien t_PhuongTien)
        {
            if (id != t_PhuongTien.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_PhuongTien.Exists(t_PhuongTien))
            {
                return NotFound();
            }
            Vm.VmT_PhuongTien.Update_Command.Execute(t_PhuongTien);
            return NoContent();
        }

        // POST: api/T_PhuongTien
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_PhuongTien>> Post(T_PhuongTien t_PhuongTien)
        {
          if (_context.T_PhuongTien == null)
          {
              return Problem("Entity set 'dbPMScontext.T_PhuongTien'  is null.");
          }
            if (Vm.VmT_PhuongTien.Exists(t_PhuongTien))
            {
                return Conflict();
            }
            Vm.VmT_PhuongTien.Insert_Command.Execute(t_PhuongTien);
            return CreatedAtAction("Get", new { id = t_PhuongTien.Ma }, t_PhuongTien);
        }

        // DELETE: api/T_PhuongTien/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_PhuongTien == null)
            {
                return NotFound();
            }
            var t_PhuongTien = Vm.VmT_PhuongTien.Find(id);
            if (t_PhuongTien == null)
            {
                return NotFound();
            }

            Vm.VmT_PhuongTien.Delete_Command.Execute(t_PhuongTien);

            return NoContent();
        }

        private bool T_PhuongTienExists(string id)
        {
            return (_context.T_PhuongTien?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
