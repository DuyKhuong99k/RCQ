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
    public class NguyenLieu_TyLeNuocController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NguyenLieu_TyLeNuocController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NguyenLieu_TyLeNuoc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguyenLieu_TyLeNuoc>>> Gets()
        {
          if (_context.NguyenLieu_TyLeNuoc == null)
          {
              return NotFound();
          }
            return Vm.VmNguyenLieu_TyLeNuoc.Items;
        }

        // GET: api/NguyenLieu_TyLeNuoc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NguyenLieu_TyLeNuoc>> Get(int id)
        {
          if (_context.NguyenLieu_TyLeNuoc == null)
          {
              return NotFound();
          }
            var nguyenLieu_TyLeNuoc = Vm.VmNguyenLieu_TyLeNuoc.Find(id);

            if (nguyenLieu_TyLeNuoc == null)
            {
                return NotFound();
            }

            return nguyenLieu_TyLeNuoc;
        }

        // PUT: api/NguyenLieu_TyLeNuoc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, NguyenLieu_TyLeNuoc nguyenLieu_TyLeNuoc)
        {
            if (id != nguyenLieu_TyLeNuoc.STT)
            {
                return BadRequest();
            }

            if (!Vm.VmNguyenLieu_TyLeNuoc.Exists(nguyenLieu_TyLeNuoc))
            {
                return NotFound();
            }
            Vm.VmNguyenLieu_TyLeNuoc.Update_Command.Execute(nguyenLieu_TyLeNuoc);
            return NoContent();
        }

        // POST: api/NguyenLieu_TyLeNuoc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NguyenLieu_TyLeNuoc>> Post(NguyenLieu_TyLeNuoc nguyenLieu_TyLeNuoc)
        {
          if (_context.NguyenLieu_TyLeNuoc == null)
          {
              return Problem("Entity set 'dbPMScontext.NguyenLieu_TyLeNuoc'  is null.");
          }
            if (Vm.VmNguyenLieu_TyLeNuoc.Exists(nguyenLieu_TyLeNuoc))
            {
                return NotFound();
            }
            Vm.VmNguyenLieu_TyLeNuoc.Insert_Command.Execute(nguyenLieu_TyLeNuoc);
            return CreatedAtAction("Get", new { id = nguyenLieu_TyLeNuoc.STT }, nguyenLieu_TyLeNuoc);
        }

        // DELETE: api/NguyenLieu_TyLeNuoc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.NguyenLieu_TyLeNuoc == null)
            {
                return NotFound();
            }
            var nguyenLieu_TyLeNuoc = Vm.VmNguyenLieu_TyLeNuoc.Find(id);
            if (nguyenLieu_TyLeNuoc == null)
            {
                return NotFound();
            }

            Vm.VmNguyenLieu_TyLeNuoc.Delete_Command.Execute(nguyenLieu_TyLeNuoc);

            return NoContent();
        }

        private bool NguyenLieu_TyLeNuocExists(int id)
        {
            return (_context.NguyenLieu_TyLeNuoc?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
