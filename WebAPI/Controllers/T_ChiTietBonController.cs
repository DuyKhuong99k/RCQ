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
    public class T_ChiTietBonController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_ChiTietBonController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_ChiTietBon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_ChiTietBon>>> Gets()
        {
          if (_context.T_ChiTietBon == null)
          {
              return NotFound();
          }
            return Vm.VmT_ChiTietBon.Items;
        }

        // GET: api/T_ChiTietBon/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_ChiTietBon>> Get(string id)
        {
          if (_context.T_ChiTietBon == null)
          {
              return NotFound();
          }
            var t_ChiTietBon = Vm.VmT_ChiTietBon.Find(id);

            if (t_ChiTietBon == null)
            {
                return NotFound();
            }

            return t_ChiTietBon;
        }

        // PUT: api/T_ChiTietBon/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_ChiTietBon t_ChiTietBon)
        {
            if (id != t_ChiTietBon.Id)
            {
                return BadRequest();
            }

            if (!Vm.VmT_ChiTietBon.Exists(t_ChiTietBon))
            {
                return NotFound();
            }
            Vm.VmT_ChiTietBon.Update_Command.Execute(t_ChiTietBon);
            return NoContent();
        }

        // POST: api/T_ChiTietBon
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_ChiTietBon>> Post(T_ChiTietBon t_ChiTietBon)
        {
          if (_context.T_ChiTietBon == null)
          {
              return Problem("Entity set 'dbPMScontext.T_ChiTietBon'  is null.");
          }
            if (Vm.VmT_ChiTietBon.Exists(t_ChiTietBon))
            {
                return Conflict();
            }
            Vm.VmT_ChiTietBon.Insert_Command.Execute(t_ChiTietBon);
            return CreatedAtAction("Get", new { id = t_ChiTietBon.Id }, t_ChiTietBon);
        }

        // DELETE: api/T_ChiTietBon/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_ChiTietBon == null)
            {
                return NotFound();
            }
            var t_ChiTietBon = Vm.VmT_ChiTietBon.Find(id);
            if (t_ChiTietBon == null)
            {
                return NotFound();
            }

            Vm.VmT_ChiTietBon.Delete_Command.Execute(t_ChiTietBon);

            return NoContent();
        }

        private bool T_ChiTietBonExists(string id)
        {
            return (_context.T_ChiTietBon?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
