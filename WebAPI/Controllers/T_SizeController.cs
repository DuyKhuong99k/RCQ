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
    public class T_SizeController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_SizeController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_Size
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_Size>>> Gets()
        {
          if (_context.T_Size == null)
          {
              return NotFound();
          }
            return Vm.VmT_Size.Items;
        }

        // GET: api/T_Size/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_Size>> Get(string id)
        {
          if (_context.T_Size == null)
          {
              return NotFound();
          }
            var t_Size = Vm.VmT_Size.Find(id);

            if (t_Size == null)
            {
                return NotFound();
            }

            return t_Size;
        }

        // PUT: api/T_Size/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_Size t_Size)
        {
            if (id != t_Size.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_Size.Exists(t_Size))
            {
                return NotFound();
            }
            Vm.VmT_Size.Update_Command.Execute(t_Size);
            return NoContent();
        }

        // POST: api/T_Size
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_Size>> Post(T_Size t_Size)
        {
          if (_context.T_Size == null)
          {
              return Problem("Entity set 'dbPMScontext.T_Size'  is null.");
          }
            if (Vm.VmT_Size.Exists(t_Size))
            {
                return Conflict();
            }
            Vm.VmT_Size.Insert_Command.Execute(t_Size);
            return CreatedAtAction("Get", new { id = t_Size.Ma }, t_Size);
        }

        // DELETE: api/T_Size/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_Size == null)
            {
                return NotFound();
            }
            var t_Size = Vm.VmT_Size.Find(id);
            if (t_Size == null)
            {
                return NotFound();
            }

            Vm.VmT_Size.Delete_Command.Execute(t_Size);

            return NoContent();
        }

        private bool T_SizeExists(string id)
        {
            return (_context.T_Size?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
