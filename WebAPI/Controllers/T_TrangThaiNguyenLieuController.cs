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
    public class T_TrangThaiNguyenLieuController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_TrangThaiNguyenLieuController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_TrangThaiNguyenLieu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_TrangThaiNguyenLieu>>> Gets()
        {
          if (_context.T_TrangThaiNguyenLieu == null)
          {
              return NotFound();
          }
            return Vm.VmT_TrangThaiNguyenLieu.Items;
        }

        // GET: api/T_TrangThaiNguyenLieu/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_TrangThaiNguyenLieu>> Get(string id)
        {
          if (_context.T_TrangThaiNguyenLieu == null)
          {
              return NotFound();
          }
            var t_TrangThaiNguyenLieu = Vm.VmT_TrangThaiNguyenLieu.Find(id);

            if (t_TrangThaiNguyenLieu == null)
            {
                return NotFound();
            }

            return t_TrangThaiNguyenLieu;
        }

        // PUT: api/T_TrangThaiNguyenLieu/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_TrangThaiNguyenLieu t_TrangThaiNguyenLieu)
        {
            if (id != t_TrangThaiNguyenLieu.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_TrangThaiNguyenLieu.Exists(t_TrangThaiNguyenLieu))
            {
                return NotFound();
            }
            Vm.VmT_TrangThaiNguyenLieu.Update_Command.Execute(t_TrangThaiNguyenLieu);
            return NoContent();
        }

        // POST: api/T_TrangThaiNguyenLieu
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_TrangThaiNguyenLieu>> Post(T_TrangThaiNguyenLieu t_TrangThaiNguyenLieu)
        {
          if (_context.T_TrangThaiNguyenLieu == null)
          {
              return Problem("Entity set 'dbPMScontext.T_TrangThaiNguyenLieu'  is null.");
          }
            if (Vm.VmT_TrangThaiNguyenLieu.Exists(t_TrangThaiNguyenLieu))
            {
                return Conflict();
            }
            Vm.VmT_TrangThaiNguyenLieu.Insert_Command.Execute(t_TrangThaiNguyenLieu);
            return CreatedAtAction("Get", new { id = t_TrangThaiNguyenLieu.Ma }, t_TrangThaiNguyenLieu);
        }

        // DELETE: api/T_TrangThaiNguyenLieu/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_TrangThaiNguyenLieu == null)
            {
                return NotFound();
            }
            var t_TrangThaiNguyenLieu = Vm.VmT_TrangThaiNguyenLieu.Find(id);
            if (t_TrangThaiNguyenLieu == null)
            {
                return NotFound();
            }

            Vm.VmT_TrangThaiNguyenLieu.Delete_Command.Execute(t_TrangThaiNguyenLieu);

            return NoContent();
        }

        private bool T_TrangThaiNguyenLieuExists(string id)
        {
            return (_context.T_TrangThaiNguyenLieu?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
