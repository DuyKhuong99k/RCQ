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
    public class T_CongDoanController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_CongDoanController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_CongDoan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_CongDoan>>> Gets()
        {
          if (_context.T_CongDoan == null)
          {
              return NotFound();
          }
            return Vm.VmT_CongDoan.Items;
        }

        // GET: api/T_CongDoan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_CongDoan>> Get(string id)
        {
          if (_context.T_CongDoan == null)
          {
              return NotFound();
          }
            var t_CongDoan = Vm.VmT_CongDoan.Find(id);

            if (t_CongDoan == null)
            {
                return NotFound();
            }

            return t_CongDoan;
        }

        // PUT: api/T_CongDoan/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_CongDoan t_CongDoan)
        {
            if (id != t_CongDoan.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_CongDoan.Exists(t_CongDoan))
            {
                return NotFound();
            }
            Vm.VmT_CongDoan.Update_Command.Execute(t_CongDoan);
            return NoContent();
        }

        // POST: api/T_CongDoan
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_CongDoan>> Post(T_CongDoan t_CongDoan)
        {
          if (_context.T_CongDoan == null)
          {
              return Problem("Entity set 'dbPMScontext.T_CongDoan'  is null.");
          }
            if (Vm.VmT_CongDoan.Exists(t_CongDoan))
            {
                return Conflict();
            }
            Vm.VmT_CongDoan.Insert_Command.Execute(t_CongDoan);
            return CreatedAtAction("Get", new { id = t_CongDoan.Ma }, t_CongDoan);
        }

        // DELETE: api/T_CongDoan/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_CongDoan == null)
            {
                return NotFound();
            }
            var t_CongDoan = Vm.VmT_CongDoan.Find(id);
            if (t_CongDoan == null)
            {
                return NotFound();
            }

            Vm.VmT_CongDoan.Delete_Command.Execute(t_CongDoan);

            return NoContent();
        }

        private bool T_CongDoanExists(string id)
        {
            return (_context.T_CongDoan?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
