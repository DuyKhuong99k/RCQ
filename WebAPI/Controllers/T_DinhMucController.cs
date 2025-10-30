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
    public class T_DinhMucController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_DinhMucController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_DinhMuc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_DinhMuc>>> Gets()
        {
          if (_context.T_DinhMuc == null)
          {
              return NotFound();
          }
            return Vm.VmT_DinhMuc.Items;
        }

        // GET: api/T_DinhMuc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_DinhMuc>> Get(int id)
        {
          if (_context.T_DinhMuc == null)
          {
              return NotFound();
          }
            var t_DinhMuc = Vm.VmT_DinhMuc.Find(id);

            if (t_DinhMuc == null)
            {
                return NotFound();
            }

            return t_DinhMuc;
        }

        // PUT: api/T_DinhMuc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, T_DinhMuc t_DinhMuc)
        {
            if (id != t_DinhMuc.STT)
            {
                return BadRequest();
            }

            if (!Vm.VmT_DinhMuc.Exists(t_DinhMuc))
            {
                return NotFound();
            }
            Vm.VmT_DinhMuc.Update_Command.Execute(t_DinhMuc);
            return NoContent();
        }

        // POST: api/T_DinhMuc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_DinhMuc>> Post(T_DinhMuc t_DinhMuc)
        {
          if (_context.T_DinhMuc == null)
          {
              return Problem("Entity set 'dbPMScontext.T_DinhMuc'  is null.");
          }
            if (Vm.VmT_DinhMuc.Exists(t_DinhMuc))
            {
                return Conflict();
            }
            Vm.VmT_DinhMuc.Insert_Command.Execute(t_DinhMuc);
            return CreatedAtAction("Get", new { id = t_DinhMuc.STT }, t_DinhMuc);
        }

        // DELETE: api/T_DinhMuc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.T_DinhMuc == null)
            {
                return NotFound();
            }
            var t_DinhMuc = Vm.VmT_DinhMuc.Find(id);
            if (t_DinhMuc == null)
            {
                return NotFound();
            }

            Vm.VmT_DinhMuc.Delete_Command.Execute(t_DinhMuc);

            return NoContent();
        }

        private bool T_DinhMucExists(int id)
        {
            return (_context.T_DinhMuc?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
