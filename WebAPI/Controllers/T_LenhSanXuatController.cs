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
    public class T_LenhSanXuatController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_LenhSanXuatController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_LenhSanXuat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_LenhSanXuat>>> Gets()
        {
          if (_context.T_LenhSanXuat == null)
          {
              return NotFound();
          }
            return Vm.VmT_LenhSanXuat.Items;
        }

        // GET: api/T_LenhSanXuat/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_LenhSanXuat>> Get(string id)
        {
          if (_context.T_LenhSanXuat == null)
          {
              return NotFound();
          }
            var t_LenhSanXuat = Vm.VmT_LenhSanXuat.Find(id);

            if (t_LenhSanXuat == null)
            {
                return NotFound();
            }

            return t_LenhSanXuat;
        }

        // PUT: api/T_LenhSanXuat/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_LenhSanXuat t_LenhSanXuat)
        {
            if (id != t_LenhSanXuat.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_LenhSanXuat.Exists(t_LenhSanXuat))
            {
                return NotFound();
            }
            Vm.VmT_LenhSanXuat.Update_Command.Execute(t_LenhSanXuat);
            return NoContent();
        }

        // POST: api/T_LenhSanXuat
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_LenhSanXuat>> Post(T_LenhSanXuat t_LenhSanXuat)
        {
          if (_context.T_LenhSanXuat == null)
          {
              return Problem("Entity set 'dbPMScontext.T_LenhSanXuat'  is null.");
          }
            if (Vm.VmT_LenhSanXuat.Exists(t_LenhSanXuat))
            {
                return Conflict();
            }
            Vm.VmT_LenhSanXuat.Insert_Command.Execute(t_LenhSanXuat);
            return CreatedAtAction("Get", new { id = t_LenhSanXuat.Ma }, t_LenhSanXuat);
        }

        // DELETE: api/T_LenhSanXuat/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_LenhSanXuat == null)
            {
                return NotFound();
            }
            var t_LenhSanXuat = Vm.VmT_LenhSanXuat.Find(id);
            if (t_LenhSanXuat == null)
            {
                return NotFound();
            }

            Vm.VmT_LenhSanXuat.Delete_Command.Execute(t_LenhSanXuat);

            return NoContent();
        }

        private bool T_LenhSanXuatExists(string id)
        {
            return (_context.T_LenhSanXuat?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
