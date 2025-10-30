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
    public class T_LoNguyenLieuController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_LoNguyenLieuController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_LoNguyenLieu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_LoNguyenLieu>>> Gets()
        {
          if (_context.T_LoNguyenLieu == null)
          {
              return NotFound();
          }
            return Vm.VmT_LoNguyenLieu.Items;
        }

        // GET: api/T_LoNguyenLieu/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_LoNguyenLieu>> Get(string id)
        {
          if (_context.T_LoNguyenLieu == null)
          {
              return NotFound();
          }
            var t_LoNguyenLieu = Vm.VmT_LoNguyenLieu.Find(id);

            if (t_LoNguyenLieu == null)
            {
                return NotFound();
            }

            return t_LoNguyenLieu;
        }

        // PUT: api/T_LoNguyenLieu/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_LoNguyenLieu t_LoNguyenLieu)
        {
            if (id != t_LoNguyenLieu.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_LoNguyenLieu.Exists(t_LoNguyenLieu))
            {
                return NotFound();
            }
            Vm.VmT_LoNguyenLieu.Update_Command.Execute(t_LoNguyenLieu);
            return NoContent();
        }

        // POST: api/T_LoNguyenLieu
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_LoNguyenLieu>> Post(T_LoNguyenLieu t_LoNguyenLieu)
        {
          if (_context.T_LoNguyenLieu == null)
          {
              return Problem("Entity set 'dbPMScontext.T_LoNguyenLieu'  is null.");
          }
            if (Vm.VmT_LoNguyenLieu.Exists(t_LoNguyenLieu))
            {
                return Conflict();
            }
            Vm.VmT_LoNguyenLieu.Insert_Command.Execute(t_LoNguyenLieu);
            return CreatedAtAction("Get", new { id = t_LoNguyenLieu.Ma }, t_LoNguyenLieu);
        }

        // DELETE: api/T_LoNguyenLieu/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_LoNguyenLieu == null)
            {
                return NotFound();
            }
            var t_LoNguyenLieu = Vm.VmT_LoNguyenLieu.Find(id);
            if (t_LoNguyenLieu == null)
            {
                return NotFound();
            }

            Vm.VmT_LoNguyenLieu.Delete_Command.Execute(t_LoNguyenLieu);

            return NoContent();
        }

        private bool T_LoNguyenLieuExists(string id)
        {
            return (_context.T_LoNguyenLieu?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
