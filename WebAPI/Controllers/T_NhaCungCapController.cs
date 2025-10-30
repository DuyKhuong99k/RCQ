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
    public class T_NhaCungCapController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_NhaCungCapController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_NhaCungCap
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_NhaCungCap>>> Gets()
        {
          if (_context.T_NhaCungCap == null)
          {
              return NotFound();
          }
            return Vm.VmT_NhaCungCap.Items;
        }

        // GET: api/T_NhaCungCap/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_NhaCungCap>> Get(string id)
        {
          if (_context.T_NhaCungCap == null)
          {
              return NotFound();
          }
            var t_NhaCungCap = Vm.VmT_NhaCungCap.Find(id);

            if (t_NhaCungCap == null)
            {
                return NotFound();
            }

            return t_NhaCungCap;
        }

        // PUT: api/T_NhaCungCap/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_NhaCungCap t_NhaCungCap)
        {
            if (id != t_NhaCungCap.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_NhaCungCap.Exists(t_NhaCungCap))
            {
                return NotFound();
            }
            Vm.VmT_NhaCungCap.Update_Command.Execute(t_NhaCungCap);
            return NoContent();
        }

        // POST: api/T_NhaCungCap
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_NhaCungCap>> Post(T_NhaCungCap t_NhaCungCap)
        {
          if (_context.T_NhaCungCap == null)
          {
              return Problem("Entity set 'dbPMScontext.T_NhaCungCap'  is null.");
          }
            if (Vm.VmT_NhaCungCap.Exists(t_NhaCungCap))
            {
                return Conflict();
            }
            Vm.VmT_NhaCungCap.Insert_Command.Execute(t_NhaCungCap);
            return CreatedAtAction("Get", new { id = t_NhaCungCap.Ma }, t_NhaCungCap);
        }

        // DELETE: api/T_NhaCungCap/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_NhaCungCap == null)
            {
                return NotFound();
            }
            var t_NhaCungCap = Vm.VmT_NhaCungCap.Find(id);
            if (t_NhaCungCap == null)
            {
                return NotFound();
            }

            Vm.VmT_NhaCungCap.Delete_Command.Execute(t_NhaCungCap);

            return NoContent();
        }

        private bool T_NhaCungCapExists(string id)
        {
            return (_context.T_NhaCungCap?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
