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
    public class T_LoaiNguyenLieuController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_LoaiNguyenLieuController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_LoaiNguyenLieu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_LoaiNguyenLieu>>> Gets()
        {
          if (_context.T_LoaiNguyenLieu == null)
          {
              return NotFound();
          }
            return Vm.VmT_LoaiNguyenLieu.Items;
        }

        // GET: api/T_LoaiNguyenLieu/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_LoaiNguyenLieu>> Get(string id)
        {
          if (_context.T_LoaiNguyenLieu == null)
          {
              return NotFound();
          }
            var t_LoaiNguyenLieu = Vm.VmT_LoaiNguyenLieu.Find(id);

            if (t_LoaiNguyenLieu == null)
            {
                return NotFound();
            }

            return t_LoaiNguyenLieu;
        }

        // PUT: api/T_LoaiNguyenLieu/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_LoaiNguyenLieu t_LoaiNguyenLieu)
        {
            if (id != t_LoaiNguyenLieu.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_LoaiNguyenLieu.Exists(t_LoaiNguyenLieu))
            {
                return NotFound();
            }
            Vm.VmT_LoaiNguyenLieu.Update_Command.Execute(t_LoaiNguyenLieu);
            return NoContent();
        }

        // POST: api/T_LoaiNguyenLieu
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_LoaiNguyenLieu>> Post(T_LoaiNguyenLieu t_LoaiNguyenLieu)
        {
          if (_context.T_LoaiNguyenLieu == null)
          {
              return Problem("Entity set 'dbPMScontext.T_LoaiNguyenLieu'  is null.");
          }
            if (Vm.VmT_LoaiNguyenLieu.Exists(t_LoaiNguyenLieu))
            {
                return Conflict();
            }
            Vm.VmT_LoaiNguyenLieu.Insert_Command.Execute(t_LoaiNguyenLieu);
            return CreatedAtAction("Get", new { id = t_LoaiNguyenLieu.Ma }, t_LoaiNguyenLieu);
        }

        // DELETE: api/T_LoaiNguyenLieu/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_LoaiNguyenLieu == null)
            {
                return NotFound();
            }
            var t_LoaiNguyenLieu = Vm.VmT_LoaiNguyenLieu.Find(id);
            if (t_LoaiNguyenLieu == null)
            {
                return NotFound();
            }

            Vm.VmT_LoaiNguyenLieu.Delete_Command.Execute(t_LoaiNguyenLieu);

            return NoContent();
        }

        private bool T_LoaiNguyenLieuExists(string id)
        {
            return (_context.T_LoaiNguyenLieu?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
