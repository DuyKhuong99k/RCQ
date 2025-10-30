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
    public class T_LoaiCanController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_LoaiCanController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_LoaiCan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_LoaiCan>>> Gets()
        {
          if (_context.T_LoaiCan == null)
          {
              return NotFound();
          }
            return Vm.VmT_LoaiCan.Items;
        }

        // GET: api/T_LoaiCan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_LoaiCan>> Get(string id)
        {
          if (_context.T_LoaiCan == null)
          {
              return NotFound();
          }
            var t_LoaiCan = Vm.VmT_LoaiCan.Find(id);

            if (t_LoaiCan == null)
            {
                return NotFound();
            }

            return t_LoaiCan;
        }

        // PUT: api/T_LoaiCan/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_LoaiCan t_LoaiCan)
        {
            if (id != t_LoaiCan.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_LoaiCan.Exists(t_LoaiCan))
            {
                return NotFound();
            }
            Vm.VmT_LoaiCan.Update_Command.Execute(t_LoaiCan);
            return NoContent();
        }

        // POST: api/T_LoaiCan
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_LoaiCan>> Post(T_LoaiCan t_LoaiCan)
        {
          if (_context.T_LoaiCan == null)
          {
              return Problem("Entity set 'dbPMScontext.T_LoaiCan'  is null.");
          }
            if (Vm.VmT_LoaiCan.Exists(t_LoaiCan))
            {
                return Conflict();
            }
            Vm.VmT_LoaiCan.Insert_Command.Execute(t_LoaiCan);
            return CreatedAtAction("Get", new { id = t_LoaiCan.Ma }, t_LoaiCan);
        }

        // DELETE: api/T_LoaiCan/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_LoaiCan == null)
            {
                return NotFound();
            }
            var t_LoaiCan = Vm.VmT_LoaiCan.Find(id);
            if (t_LoaiCan == null)
            {
                return NotFound();
            }

            Vm.VmT_LoaiCan.Delete_Command.Execute(t_LoaiCan);

            return NoContent();
        }

        private bool T_LoaiCanExists(string id)
        {
            return (_context.T_LoaiCan?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
