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
    public class MaThanhPhamDinhHinh_ColorController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamDinhHinh_ColorController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThanhPhamDinhHinh_Color
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamDinhHinh_Color>>> Gets()
        {
          if (_context.MaThanhPhamDinhHinh_Color == null)
          {
              return NotFound();
          }
            return Vm.VmThanhPhamDinhHinh_Color.Items;
        }

        // GET: api/MaThanhPhamDinhHinh_Color/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamDinhHinh_Color>> Get(string id)
        {
          if (_context.MaThanhPhamDinhHinh_Color == null)
          {
              return NotFound();
          }
            var maThanhPhamDinhHinh_Color = Vm.VmThanhPhamDinhHinh_Color.Find(id);

            if (maThanhPhamDinhHinh_Color == null)
            {
                return NotFound();
            }

            return maThanhPhamDinhHinh_Color;
        }

        // PUT: api/MaThanhPhamDinhHinh_Color/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThanhPhamDinhHinh_Color maThanhPhamDinhHinh_Color)
        {
            if (id != maThanhPhamDinhHinh_Color.ColorCode)
            {
                return BadRequest();
            }

            if (!Vm.VmThanhPhamDinhHinh_Color.Exists(maThanhPhamDinhHinh_Color))
            {
                return NotFound();
            }
            Vm.VmThanhPhamDinhHinh_Color.Update_Command.Execute(maThanhPhamDinhHinh_Color);
            return NoContent();
        }

        // POST: api/MaThanhPhamDinhHinh_Color
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamDinhHinh_Color>> Post(MaThanhPhamDinhHinh_Color maThanhPhamDinhHinh_Color)
        {
          if (_context.MaThanhPhamDinhHinh_Color == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamDinhHinh_Color'  is null.");
          }
            if (Vm.VmThanhPhamDinhHinh_Color.Exists(maThanhPhamDinhHinh_Color))
            {
                return Conflict();
            }
            Vm.VmThanhPhamDinhHinh_Color.Insert_Command.Execute(maThanhPhamDinhHinh_Color);
            return CreatedAtAction("Get", new { id = maThanhPhamDinhHinh_Color.ColorCode }, maThanhPhamDinhHinh_Color);
        }

        // DELETE: api/MaThanhPhamDinhHinh_Color/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThanhPhamDinhHinh_Color == null)
            {
                return NotFound();
            }
            var maThanhPhamDinhHinh_Color = Vm.VmThanhPhamDinhHinh_Color.Find(id);
            if (maThanhPhamDinhHinh_Color == null)
            {
                return NotFound();
            }

            Vm.VmThanhPhamDinhHinh_Color.Delete_Command.Execute(maThanhPhamDinhHinh_Color);

            return NoContent();
        }

        private bool MaThanhPhamDinhHinh_ColorExists(string id)
        {
            return (_context.MaThanhPhamDinhHinh_Color?.Any(e => e.ColorCode == id)).GetValueOrDefault();
        }
    }
}
