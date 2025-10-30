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
    public class T_QuyCachController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_QuyCachController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_QuyCach
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_QuyCach>>> Gets()
        {
          if (_context.T_QuyCach == null)
          {
              return NotFound();
          }
            return Vm.VmT_QuyCach.Items;
        }

        // GET: api/T_QuyCach/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_QuyCach>> Get(string id)
        {
          if (_context.T_QuyCach == null)
          {
              return NotFound();
          }
            var t_QuyCach = Vm.VmT_QuyCach.Find(id);

            if (t_QuyCach == null)
            {
                return NotFound();
            }

            return t_QuyCach;
        }

        // PUT: api/T_QuyCach/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_QuyCach t_QuyCach)
        {
            if (id != t_QuyCach.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_QuyCach.Exists(t_QuyCach))
            {
                return NotFound();
            }
            Vm.VmT_QuyCach.Update_Command.Execute(t_QuyCach);
            return NoContent();
        }

        // POST: api/T_QuyCach
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_QuyCach>> Post(T_QuyCach t_QuyCach)
        {
          if (_context.T_QuyCach == null)
          {
              return Problem("Entity set 'dbPMScontext.T_QuyCach'  is null.");
          }
            if (Vm.VmT_QuyCach.Exists(t_QuyCach))
            {
                return Conflict();
            }
            Vm.VmT_QuyCach.Insert_Command.Execute(t_QuyCach);
            return CreatedAtAction("Get", new { id = t_QuyCach.Ma }, t_QuyCach);
        }

        // DELETE: api/T_QuyCach/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_QuyCach == null)
            {
                return NotFound();
            }
            var t_QuyCach = Vm.VmT_QuyCach.Find(id);
            if (t_QuyCach == null)
            {
                return NotFound();
            }

            Vm.VmT_QuyCach.Insert_Command.Execute(t_QuyCach);

            return NoContent();
        }

        private bool T_QuyCachExists(string id)
        {
            return (_context.T_QuyCach?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
