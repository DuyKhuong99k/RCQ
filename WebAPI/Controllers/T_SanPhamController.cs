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
    public class T_SanPhamController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_SanPhamController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_SanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_SanPham>>> Gets()
        {
          if (_context.T_SanPham == null)
          {
              return NotFound();
          }
            return Vm.VmT_SanPham.Items;
        }

        // GET: api/T_SanPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_SanPham>> Get(string id)
        {
          if (_context.T_SanPham == null)
          {
              return NotFound();
          }
            var t_SanPham = Vm.VmT_SanPham.Find(id);

            if (t_SanPham == null)
            {
                return NotFound();
            }

            return t_SanPham;
        }

        // PUT: api/T_SanPham/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_SanPham t_SanPham)
        {
            if (id != t_SanPham.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_SanPham.Exists(t_SanPham))
            {
                return NotFound();
            }
            Vm.VmT_SanPham.Update_Command.Execute(t_SanPham);
            return NoContent();
        }

        // POST: api/T_SanPham
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_SanPham>> Post(T_SanPham t_SanPham)
        {
          if (_context.T_SanPham == null)
          {
              return Problem("Entity set 'dbPMScontext.T_SanPham'  is null.");
          }
            if (!Vm.VmT_SanPham.Exists(t_SanPham))
            {
                return Conflict();
            }
            Vm.VmT_SanPham.Insert_Command.Execute(t_SanPham);
            return CreatedAtAction("Get", new { id = t_SanPham.Ma }, t_SanPham);
        }

        // DELETE: api/T_SanPham/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_SanPham == null)
            {
                return NotFound();
            }
            var t_SanPham = Vm.VmT_SanPham.Find(id);
            if (t_SanPham == null)
            {
                return NotFound();
            }

            Vm.VmT_SanPham.Delete_Command.Execute(t_SanPham);

            return NoContent();
        }

        private bool T_SanPhamExists(string id)
        {
            return (_context.T_SanPham?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
