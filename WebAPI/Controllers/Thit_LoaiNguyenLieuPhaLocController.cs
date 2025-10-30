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
    public class Thit_LoaiNguyenLieuPhaLocController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public Thit_LoaiNguyenLieuPhaLocController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/Thit_LoaiNguyenLieuPhaLoc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Thit_LoaiNguyenLieuPhaLoc>>> Gets()
        {
          if (_context.Thit_LoaiNguyenLieuPhaLoc == null)
          {
              return NotFound();
          }
            return Vm.VmThit_LoaiNguyenLieuPhaLoc.Items;
        }

        // GET: api/Thit_LoaiNguyenLieuPhaLoc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Thit_LoaiNguyenLieuPhaLoc>> Get(string id)
        {
          if (_context.Thit_LoaiNguyenLieuPhaLoc == null)
          {
              return NotFound();
          }
            var thit_LoaiNguyenLieuPhaLoc = Vm.VmThit_LoaiNguyenLieuPhaLoc.Find(id);

            if (thit_LoaiNguyenLieuPhaLoc == null)
            {
                return NotFound();
            }

            return thit_LoaiNguyenLieuPhaLoc;
        }

        // PUT: api/Thit_LoaiNguyenLieuPhaLoc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, Thit_LoaiNguyenLieuPhaLoc thit_LoaiNguyenLieuPhaLoc)
        {
            if (id != thit_LoaiNguyenLieuPhaLoc.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmThit_LoaiNguyenLieuPhaLoc.Exists(thit_LoaiNguyenLieuPhaLoc))
            {
                return NotFound();
            }
            Vm.VmThit_LoaiNguyenLieuPhaLoc.Update_Command.Execute(thit_LoaiNguyenLieuPhaLoc);
            return NoContent();
        }

        // POST: api/Thit_LoaiNguyenLieuPhaLoc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Thit_LoaiNguyenLieuPhaLoc>> Post(Thit_LoaiNguyenLieuPhaLoc thit_LoaiNguyenLieuPhaLoc)
        {
          if (_context.Thit_LoaiNguyenLieuPhaLoc == null)
          {
              return Problem("Entity set 'dbPMScontext.Thit_LoaiNguyenLieuPhaLoc'  is null.");
          }
            if (Vm.VmThit_LoaiNguyenLieuPhaLoc.Exists(thit_LoaiNguyenLieuPhaLoc))
            {
                return Conflict();
            }
            Vm.VmThit_LoaiNguyenLieuPhaLoc.Insert_Command.Execute(thit_LoaiNguyenLieuPhaLoc);
            return CreatedAtAction("Get", new { id = thit_LoaiNguyenLieuPhaLoc.Ma }, thit_LoaiNguyenLieuPhaLoc);
        }

        // DELETE: api/Thit_LoaiNguyenLieuPhaLoc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Thit_LoaiNguyenLieuPhaLoc == null)
            {
                return NotFound();
            }
            var thit_LoaiNguyenLieuPhaLoc = Vm.VmThit_LoaiNguyenLieuPhaLoc.Find(id);
            if (thit_LoaiNguyenLieuPhaLoc == null)
            {
                return NotFound();
            }

            Vm.VmThit_LoaiNguyenLieuPhaLoc.Delete_Command.Execute(thit_LoaiNguyenLieuPhaLoc);

            return NoContent();
        }

        private bool Thit_LoaiNguyenLieuPhaLocExists(string id)
        {
            return (_context.Thit_LoaiNguyenLieuPhaLoc?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
