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
    public class MaThanhPhamSoCheDinhHinh_ColorController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamSoCheDinhHinh_ColorController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThanhPhamSoCheDinhHinh_Color
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamSoCheDinhHinh_Color>>> Gets()
        {
          if (_context.MaThanhPhamSoCheDinhHinh_Color == null)
          {
              return NotFound();
          }
            return Vm.VmThanhPhamSoCheDinhHinh_Color.Items;
        }

        // GET: api/MaThanhPhamSoCheDinhHinh_Color/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamSoCheDinhHinh_Color>> Get(string id)
        {
          if (_context.MaThanhPhamSoCheDinhHinh_Color == null)
          {
              return NotFound();
          }
            var maThanhPhamSoCheDinhHinh_Color = Vm.VmThanhPhamSoCheDinhHinh_Color.Find(id);

            if (maThanhPhamSoCheDinhHinh_Color == null)
            {
                return NotFound();
            }

            return maThanhPhamSoCheDinhHinh_Color;
        }

        // PUT: api/MaThanhPhamSoCheDinhHinh_Color/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThanhPhamSoCheDinhHinh_Color maThanhPhamSoCheDinhHinh_Color)
        {
            if (id != maThanhPhamSoCheDinhHinh_Color.ColorCode)
            {
                return BadRequest();
            }

            if (!Vm.VmThanhPhamSoCheDinhHinh_Color.Exists(maThanhPhamSoCheDinhHinh_Color))
            {
                return NotFound();
            }
            Vm.VmThanhPhamSoCheDinhHinh_Color.Update_Command.Execute(maThanhPhamSoCheDinhHinh_Color);
            return NoContent();
        }

        // POST: api/MaThanhPhamSoCheDinhHinh_Color
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamSoCheDinhHinh_Color>> Post(MaThanhPhamSoCheDinhHinh_Color maThanhPhamSoCheDinhHinh_Color)
        {
          if (_context.MaThanhPhamSoCheDinhHinh_Color == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamSoCheDinhHinh_Color'  is null.");
          }
            if (Vm.VmThanhPhamSoCheDinhHinh_Color.Exists(maThanhPhamSoCheDinhHinh_Color))
            {
                return Conflict();
            }
            Vm.VmThanhPhamSoCheDinhHinh_Color.Insert_Command.Execute(maThanhPhamSoCheDinhHinh_Color);
            return CreatedAtAction("Get", new { id = maThanhPhamSoCheDinhHinh_Color.ColorCode }, maThanhPhamSoCheDinhHinh_Color);
        }

        // DELETE: api/MaThanhPhamSoCheDinhHinh_Color/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThanhPhamSoCheDinhHinh_Color == null)
            {
                return NotFound();
            }
            var maThanhPhamSoCheDinhHinh_Color = Vm.VmThanhPhamSoCheDinhHinh_Color.Find(id);
            if (maThanhPhamSoCheDinhHinh_Color == null)
            {
                return NotFound();
            }

            Vm.VmThanhPhamSoCheDinhHinh_Color.Delete_Command.Execute(maThanhPhamSoCheDinhHinh_Color);

            return NoContent();
        }

        private bool MaThanhPhamSoCheDinhHinh_ColorExists(string id)
        {
            return (_context.MaThanhPhamSoCheDinhHinh_Color?.Any(e => e.ColorCode == id)).GetValueOrDefault();
        }
    }
}
