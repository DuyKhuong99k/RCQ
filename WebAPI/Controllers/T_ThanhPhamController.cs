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
    public class T_ThanhPhamController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_ThanhPhamController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_ThanhPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_ThanhPham>>> Gets()
        {
          if (_context.T_ThanhPham == null)
          {
              return NotFound();
          }
            return Vm.VmT_ThanhPham.Items;
        }

        // GET: api/T_ThanhPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_ThanhPham>> GetT_ThanhPham(string id)
        {
          if (_context.T_ThanhPham == null)
          {
              return NotFound();
          }
            var t_ThanhPham = Vm.VmT_ThanhPham.Find(id);

            if (t_ThanhPham == null)
            {
                return NotFound();
            }

            return t_ThanhPham;
        }

        // PUT: api/T_ThanhPham/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_ThanhPham t_ThanhPham)
        {
            if (id != t_ThanhPham.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_ThanhPham.Exists(t_ThanhPham))
            {
                return NotFound();

            }
            Vm.VmT_ThanhPham.Update_Command.Execute(t_ThanhPham);
            return NoContent();
        }

        // POST: api/T_ThanhPham
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_ThanhPham>> Post(T_ThanhPham t_ThanhPham)
        {
          if (_context.T_ThanhPham == null)
          {
              return Problem("Entity set 'dbPMScontext.T_ThanhPham'  is null.");
          }
            if (Vm.VmT_ThanhPham.Exists(t_ThanhPham))
            {
                return Conflict();
            }
            Vm.VmT_ThanhPham.Insert_Command.Execute(t_ThanhPham);
            return CreatedAtAction("Get", new { id = t_ThanhPham.Ma }, t_ThanhPham);
        }

        // DELETE: api/T_ThanhPham/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteT_ThanhPham(string id)
        {
            if (_context.T_ThanhPham == null)
            {
                return NotFound();
            }
            var t_ThanhPham = Vm.VmT_ThanhPham.Find(id);
            if (t_ThanhPham == null)
            {
                return NotFound();
            }

            Vm.VmT_ThanhPham.Delete_Command.Execute(t_ThanhPham);

            return NoContent();
        }

        private bool T_ThanhPhamExists(string id)
        {
            return (_context.T_ThanhPham?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
