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
    public class MaThanhPhamLangDa_ColorController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamLangDa_ColorController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThanhPhamLangDa_Color
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamLangDa_Color>>> Gets()
        {
          if (_context.MaThanhPhamLangDa_Color == null)
          {
              return NotFound();
          }
            return Vm.VmThanhPhamLangDa_Color.Items;
        }

        // GET: api/MaThanhPhamLangDa_Color/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamLangDa_Color>> Get(string id)
        {
          if (_context.MaThanhPhamLangDa_Color == null)
          {
              return NotFound();
          }
            var maThanhPhamLangDa_Color = Vm.VmThanhPhamLangDa_Color.Find(id);

            if (maThanhPhamLangDa_Color == null)
            {
                return NotFound();
            }

            return maThanhPhamLangDa_Color;
        }

        // PUT: api/MaThanhPhamLangDa_Color/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThanhPhamLangDa_Color maThanhPhamLangDa_Color)
        {
            if (id != maThanhPhamLangDa_Color.ColorCode)
            {
                return BadRequest();
            }

            if (!Vm.VmThanhPhamLangDa_Color.Exists(maThanhPhamLangDa_Color))
            {
                return NotFound();
            }
            Vm.VmThanhPhamLangDa_Color.Update_Command.Execute(maThanhPhamLangDa_Color);
            return NoContent();
        }

        // POST: api/MaThanhPhamLangDa_Color
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamLangDa_Color>> Post(MaThanhPhamLangDa_Color maThanhPhamLangDa_Color)
        {
          if (_context.MaThanhPhamLangDa_Color == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamLangDa_Color'  is null.");
          }
            if (Vm.VmThanhPhamLangDa_Color.Exists(maThanhPhamLangDa_Color))
            {
                return Conflict();
            }
            Vm.VmThanhPhamLangDa_Color.Insert_Command.Execute(maThanhPhamLangDa_Color);
            return CreatedAtAction("Get", new { id = maThanhPhamLangDa_Color.ColorCode }, maThanhPhamLangDa_Color);
        }

        // DELETE: api/MaThanhPhamLangDa_Color/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThanhPhamLangDa_Color == null)
            {
                return NotFound();
            }
            var maThanhPhamLangDa_Color = Vm.VmThanhPhamLangDa_Color.Find(id);
            if (maThanhPhamLangDa_Color == null)
            {
                return NotFound();
            }

            Vm.VmThanhPhamLangDa_Color.Delete_Command.Execute(maThanhPhamLangDa_Color);

            return NoContent();
        }

        private bool MaThanhPhamLangDa_ColorExists(string id)
        {
            return (_context.MaThanhPhamLangDa_Color?.Any(e => e.ColorCode == id)).GetValueOrDefault();
        }
    }
}
