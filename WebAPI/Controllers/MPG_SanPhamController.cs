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
    public class MPG_SanPhamController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MPG_SanPhamController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MPG_SanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MPG_SanPham>>> Gets()
        {
          if (_context.MPG_SanPham == null)
          {
              return NotFound();
          }
            return Vm.VmMPG_SanPham.Items;
        }

        // GET: api/MPG_SanPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MPG_SanPham>> Get(string id)
        {
          if (_context.MPG_SanPham == null)
          {
              return NotFound();
          }
            var mPG_SanPham = Vm.VmMPG_SanPham.Find(id);

            if (mPG_SanPham == null)
            {
                return NotFound();
            }

            return mPG_SanPham;
        }

        // PUT: api/MPG_SanPham/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MPG_SanPham mPG_SanPham)
        {
            if (id != mPG_SanPham.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmMPG_SanPham.Exists(mPG_SanPham))
            {
                return NotFound();
            }
            Vm.VmMPG_SanPham.Update_Command.Execute(mPG_SanPham);
            return NoContent();
        }

        // POST: api/MPG_SanPham
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MPG_SanPham>> Post(MPG_SanPham mPG_SanPham)
        {
          if (_context.MPG_SanPham == null)
          {
              return Problem("Entity set 'dbPMScontext.MPG_SanPham'  is null.");
          }
            if (Vm.VmMPG_SanPham.Exists(mPG_SanPham))
            {
                return Conflict();
            }
            Vm.VmMPG_SanPham.Insert_Command.Execute(mPG_SanPham);
            return CreatedAtAction("Get", new { id = mPG_SanPham.Ma }, mPG_SanPham);
        }

        // DELETE: api/MPG_SanPham/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MPG_SanPham == null)
            {
                return NotFound();
            }
            var mPG_SanPham = Vm.VmMPG_SanPham.Find(id);
            if (mPG_SanPham == null)
            {
                return NotFound();
            }

            Vm.VmMPG_SanPham.Delete_Command.Execute(mPG_SanPham);

            return NoContent();
        }

        private bool MPG_SanPhamExists(string id)
        {
            return (_context.MPG_SanPham?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
