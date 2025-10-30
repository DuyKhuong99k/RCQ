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
    public class T_LoaiKhuonController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_LoaiKhuonController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_LoaiKhuon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_LoaiKhuon>>> Gets()
        {
          if (_context.T_LoaiKhuon == null)
          {
              return NotFound();
          }
            return Vm.VmT_LoaiKhuon.Items;
        }

        // GET: api/T_LoaiKhuon/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_LoaiKhuon>> GetT(string id)
        {
          if (_context.T_LoaiKhuon == null)
          {
              return NotFound();
          }
            var t_LoaiKhuon = Vm.VmT_LoaiKhuon.Find(id);

            if (t_LoaiKhuon == null)
            {
                return NotFound();
            }

            return t_LoaiKhuon;
        }

        // PUT: api/T_LoaiKhuon/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_LoaiKhuon t_LoaiKhuon)
        {
            if (id != t_LoaiKhuon.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_LoaiKhuon.Exists(t_LoaiKhuon))
            {
                return NotFound();
            }
            Vm.VmT_LoaiKhuon.Update_Command.Execute(t_LoaiKhuon);
            return NoContent();
        }

        // POST: api/T_LoaiKhuon
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_LoaiKhuon>> Post(T_LoaiKhuon t_LoaiKhuon)
        {
          if (_context.T_LoaiKhuon == null)
          {
              return Problem("Entity set 'dbPMScontext.T_LoaiKhuon'  is null.");
          }
            if (Vm.VmT_LoaiKhuon.Exists(t_LoaiKhuon))
            {
                return Conflict();
            }
            Vm.VmT_LoaiKhuon.Insert_Command.Execute(t_LoaiKhuon);
            return CreatedAtAction("Get", new { id = t_LoaiKhuon.Ma }, t_LoaiKhuon);
        }

        // DELETE: api/T_LoaiKhuon/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_LoaiKhuon == null)
            {
                return NotFound();
            }
            var t_LoaiKhuon = Vm.VmT_LoaiKhuon.Find(id);
            if (t_LoaiKhuon == null)
            {
                return NotFound();
            }

            Vm.VmT_LoaiKhuon.Delete_Command.Execute(t_LoaiKhuon);

            return NoContent();
        }

        private bool T_LoaiKhuonExists(string id)
        {
            return (_context.T_LoaiKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
