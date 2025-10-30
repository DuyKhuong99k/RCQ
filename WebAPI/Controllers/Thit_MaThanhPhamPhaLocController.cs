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
    public class Thit_MaThanhPhamPhaLocController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public Thit_MaThanhPhamPhaLocController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/Thit_MaThanhPhamPhaLoc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Thit_MaThanhPhamPhaLoc>>> Gets()
        {
          if (_context.Thit_MaThanhPhamPhaLoc == null)
          {
              return NotFound();
          }
            return Vm.VmThit_MaThanhPham.Items;
        }

        // GET: api/Thit_MaThanhPhamPhaLoc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Thit_MaThanhPhamPhaLoc>> Gets(string id)
        {
          if (_context.Thit_MaThanhPhamPhaLoc == null)
          {
              return NotFound();
          }
            var thit_MaThanhPhamPhaLoc = Vm.VmThit_MaThanhPham.Find(id);

            if (thit_MaThanhPhamPhaLoc == null)
            {
                return NotFound();
            }

            return thit_MaThanhPhamPhaLoc;
        }

        // PUT: api/Thit_MaThanhPhamPhaLoc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, Thit_MaThanhPhamPhaLoc thit_MaThanhPhamPhaLoc)
        {
            if (id != thit_MaThanhPhamPhaLoc.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmThit_MaThanhPham.Exists(thit_MaThanhPhamPhaLoc))
            {
                return NotFound();
            }
            Vm.VmThit_MaThanhPham.Update_Command.Execute(thit_MaThanhPhamPhaLoc);
            return NoContent();
        }

        // POST: api/Thit_MaThanhPhamPhaLoc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Thit_MaThanhPhamPhaLoc>> Post(Thit_MaThanhPhamPhaLoc thit_MaThanhPhamPhaLoc)
        {
          if (_context.Thit_MaThanhPhamPhaLoc == null)
          {
              return Problem("Entity set 'dbPMScontext.Thit_MaThanhPhamPhaLoc'  is null.");
          }
            if (Vm.VmThit_MaThanhPham.Exists(thit_MaThanhPhamPhaLoc))
            {
                return Conflict();
            }
            Vm.VmThit_MaThanhPham.Insert_Command.Execute(thit_MaThanhPhamPhaLoc);
            return CreatedAtAction("Get", new { id = thit_MaThanhPhamPhaLoc.Ma }, thit_MaThanhPhamPhaLoc);
        }

        // DELETE: api/Thit_MaThanhPhamPhaLoc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Thit_MaThanhPhamPhaLoc == null)
            {
                return NotFound();
            }
            var thit_MaThanhPhamPhaLoc = Vm.VmThit_MaThanhPham.Find(id);
            if (thit_MaThanhPhamPhaLoc == null)
            {
                return NotFound();
            }

            Vm.VmThit_MaThanhPham.Delete_Command.Execute(thit_MaThanhPhamPhaLoc);

            return NoContent();
        }

        private bool Thit_MaThanhPhamPhaLocExists(string id)
        {
            return (_context.Thit_MaThanhPhamPhaLoc?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
