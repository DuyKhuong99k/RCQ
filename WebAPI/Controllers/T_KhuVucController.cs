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
    public class T_KhuVucController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_KhuVucController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_KhuVuc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_KhuVuc>>> Gets()
        {
          if (_context.T_KhuVuc == null)
          {
              return NotFound();
          }
            return Vm.VmT_KhuVuc.Items;
        }

        // GET: api/T_KhuVuc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_KhuVuc>> Get(string id)
        {
          if (_context.T_KhuVuc == null)
          {
              return NotFound();
          }
            var t_KhuVuc = Vm.VmT_KhuVuc.Find(id);

            if (t_KhuVuc == null)
            {
                return NotFound();
            }

            return t_KhuVuc;
        }

        // PUT: api/T_KhuVuc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_KhuVuc t_KhuVuc)
        {
            if (id != t_KhuVuc.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_KhuVuc.Exists(t_KhuVuc))
            {
                return NotFound();
            }
            Vm.VmT_KhuVuc.Update_Command.Execute(t_KhuVuc);
            return NoContent();
        }

        // POST: api/T_KhuVuc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_KhuVuc>> Post(T_KhuVuc t_KhuVuc)
        {
          if (_context.T_KhuVuc == null)
          {
              return Problem("Entity set 'dbPMScontext.T_KhuVuc'  is null.");
          }
            if (Vm.VmT_KhuVuc.Exists(t_KhuVuc))
            {
                return Conflict();
            }
            Vm.VmT_KhuVuc.Insert_Command.Execute(t_KhuVuc);
            return CreatedAtAction("Get", new { id = t_KhuVuc.Ma }, t_KhuVuc);
        }

        // DELETE: api/T_KhuVuc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_KhuVuc == null)
            {
                return NotFound();
            }
            var t_KhuVuc = Vm.VmT_KhuVuc.Find(id);
            if (t_KhuVuc == null)
            {
                return NotFound();
            }

            Vm.VmT_KhuVuc.Delete_Command.Execute(t_KhuVuc);

            return NoContent();
        }

        private bool T_KhuVucExists(string id)
        {
            return (_context.T_KhuVuc?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
