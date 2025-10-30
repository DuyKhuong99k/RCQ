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
    public class MaThanhPhamPhuPham_ColorController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamPhuPham_ColorController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThanhPhamPhuPham_Color
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamPhuPham_Color>>> Gets()
        {
          if (_context.MaThanhPhamPhuPham_Color == null)
          {
              return NotFound();
          }
            return Vm.VmThanhPhamPhuPham_Color.Items;
        }

        // GET: api/MaThanhPhamPhuPham_Color/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamPhuPham_Color>> Get(string id)
        {
          if (_context.MaThanhPhamPhuPham_Color == null)
          {
              return NotFound();
          }
            var maThanhPhamPhuPham_Color = Vm.VmThanhPhamPhuPham_Color.Find(id);

            if (maThanhPhamPhuPham_Color == null)
            {
                return NotFound();
            }

            return maThanhPhamPhuPham_Color;
        }

        // PUT: api/MaThanhPhamPhuPham_Color/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThanhPhamPhuPham_Color maThanhPhamPhuPham_Color)
        {
            if (id != maThanhPhamPhuPham_Color.ColorCode)
            {
                return BadRequest();
            }

            if (!Vm.VmThanhPhamPhuPham_Color.Exists(maThanhPhamPhuPham_Color))
            {
                return NotFound();
            }
            Vm.VmThanhPhamPhuPham_Color.Update_Command.Execute(maThanhPhamPhuPham_Color);
            return NoContent();
        }

        // POST: api/MaThanhPhamPhuPham_Color
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamPhuPham_Color>> Post(MaThanhPhamPhuPham_Color maThanhPhamPhuPham_Color)
        {
          if (_context.MaThanhPhamPhuPham_Color == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamPhuPham_Color'  is null.");
          }
            if (Vm.VmThanhPhamPhuPham_Color.Exists(maThanhPhamPhuPham_Color))
            {
                return Conflict();
            }
            Vm.VmThanhPhamPhuPham_Color.Insert_Command.Execute(maThanhPhamPhuPham_Color);
            return CreatedAtAction("Get", new { id = maThanhPhamPhuPham_Color.ColorCode }, maThanhPhamPhuPham_Color);
        }

        // DELETE: api/MaThanhPhamPhuPham_Color/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThanhPhamPhuPham_Color == null)
            {
                return NotFound();
            }
            var maThanhPhamPhuPham_Color = Vm.VmThanhPhamPhuPham_Color.Find(id);
            if (maThanhPhamPhuPham_Color == null)
            {
                return NotFound();
            }

            Vm.VmThanhPhamPhuPham_Color.Delete_Command.Execute(maThanhPhamPhuPham_Color);

            return NoContent();
        }

        private bool MaThanhPhamPhuPham_ColorExists(string id)
        {
            return (_context.MaThanhPhamPhuPham_Color?.Any(e => e.ColorCode == id)).GetValueOrDefault();
        }
    }
}
