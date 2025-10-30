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
    public class MaThanhPhamFillet_ColorController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamFillet_ColorController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThanhPhamFillet_Color
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamFillet_Color>>> Gets()
        {
          if (_context.MaThanhPhamFillet_Color == null)
          {
              return NotFound();
          }
            return Vm.VmThanhPhamFillet_Color.Items;
        }

        // GET: api/MaThanhPhamFillet_Color/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamFillet_Color>> Get(string id)
        {
          if (_context.MaThanhPhamFillet_Color == null)
          {
              return NotFound();
          }
            var maThanhPhamFillet_Color = Vm.VmThanhPhamFillet_Color.Find(id);

            if (maThanhPhamFillet_Color == null)
            {
                return NotFound();
            }

            return maThanhPhamFillet_Color;
        }

        // PUT: api/MaThanhPhamFillet_Color/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThanhPhamFillet_Color maThanhPhamFillet_Color)
        {
            if (id != maThanhPhamFillet_Color.ColorCode)
            {
                return BadRequest();
            }

            if (!Vm.VmThanhPhamFillet_Color.Exists(maThanhPhamFillet_Color))
            {
                return NotFound();
            }
            Vm.VmThanhPhamFillet_Color.Update_Command.Execute(maThanhPhamFillet_Color);
            return NoContent();
        }

        // POST: api/MaThanhPhamFillet_Color
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamFillet_Color>> Post(MaThanhPhamFillet_Color maThanhPhamFillet_Color)
        {
          if (_context.MaThanhPhamFillet_Color == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamFillet_Color'  is null.");
          }
            if (Vm.VmThanhPhamFillet_Color.Exists(maThanhPhamFillet_Color))
            {
                return Conflict();
            }
            Vm.VmThanhPhamFillet_Color.Insert_Command.Execute(maThanhPhamFillet_Color);
            return CreatedAtAction("Get", new { id = maThanhPhamFillet_Color.ColorCode }, maThanhPhamFillet_Color);
        }

        // DELETE: api/MaThanhPhamFillet_Color/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThanhPhamFillet_Color == null)
            {
                return NotFound();
            }
            var maThanhPhamFillet_Color = Vm.VmThanhPhamFillet_Color.Find(id);
            if (maThanhPhamFillet_Color == null)
            {
                return NotFound();
            }

            Vm.VmThanhPhamFillet_Color.Delete_Command.Execute(maThanhPhamFillet_Color);

            return NoContent();
        }

        private bool MaThanhPhamFillet_ColorExists(string id)
        {
            return (_context.MaThanhPhamFillet_Color?.Any(e => e.ColorCode == id)).GetValueOrDefault();
        }
    }
}
