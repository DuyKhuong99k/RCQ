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
    public class T_PhuGiaController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_PhuGiaController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_PhuGia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_PhuGia>>> Gets()
        {
          if (_context.T_PhuGia == null)
          {
              return NotFound();
          }
            return Vm.VmT_PhuGia.Items;
        }

        // GET: api/T_PhuGia/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_PhuGia>> Get(string id)
        {
          if (_context.T_PhuGia == null)
          {
              return NotFound();
          }
            var t_PhuGia = Vm.VmT_PhuGia.Find(id);

            if (t_PhuGia == null)
            {
                return NotFound();
            }

            return t_PhuGia;
        }

        // PUT: api/T_PhuGia/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_PhuGia t_PhuGia)
        {
            if (id != t_PhuGia.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_PhuGia.Exists(t_PhuGia))
            {
                return NotFound();
            }
            Vm.VmT_PhuGia.Update_Command.Execute(t_PhuGia);
            return NoContent();
        }

        // POST: api/T_PhuGia
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_PhuGia>> Post(T_PhuGia t_PhuGia)
        {
          if (_context.T_PhuGia == null)
          {
              return Problem("Entity set 'dbPMScontext.T_PhuGia'  is null.");
          }
            if (Vm.VmT_PhuGia.Exists(t_PhuGia))
            {
                return Conflict();
            }
            Vm.VmT_PhuGia.Insert_Command.Execute(t_PhuGia); 
            return CreatedAtAction("Get", new { id = t_PhuGia.Ma }, t_PhuGia);
        }

        // DELETE: api/T_PhuGia/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_PhuGia == null)
            {
                return NotFound();
            }
            var t_PhuGia = Vm.VmT_PhuGia.Find(id);
            if (t_PhuGia == null)
            {
                return NotFound();
            }

            Vm.VmT_PhuGia.Delete_Command.Execute(t_PhuGia);

            return NoContent();
        }

        private bool T_PhuGiaExists(string id)
        {
            return (_context.T_PhuGia?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
