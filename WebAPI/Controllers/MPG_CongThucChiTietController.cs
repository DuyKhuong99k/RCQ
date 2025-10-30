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
    public class MPG_CongThucChiTietController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MPG_CongThucChiTietController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MPG_CongThucChiTiet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MPG_CongThucChiTiet>>> Gets()
        {
          if (_context.MPG_CongThucChiTiet == null)
          {
              return NotFound();
          }
            return Vm.VmMPG_CongThucChiTiet.Items;
        }

        // GET: api/MPG_CongThucChiTiet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MPG_CongThucChiTiet>> Get(string id)
        {
          if (_context.MPG_CongThucChiTiet == null)
          {
              return NotFound();
          }
            var mPG_CongThucChiTiet = Vm.VmMPG_CongThucChiTiet.Find(id);

            if (mPG_CongThucChiTiet == null)
            {
                return NotFound();
            }

            return mPG_CongThucChiTiet;
        }

        // PUT: api/MPG_CongThucChiTiet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MPG_CongThucChiTiet mPG_CongThucChiTiet)
        {
            if (id != mPG_CongThucChiTiet.Ma)
            {
                return BadRequest();
            }

            if(!Vm.VmMPG_CongThucChiTiet.Exists(mPG_CongThucChiTiet))
            {
                return NotFound();
            }
            Vm.VmMPG_CongThucChiTiet.Update_Command.Execute(mPG_CongThucChiTiet);
            return NoContent();
        }

        // POST: api/MPG_CongThucChiTiet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MPG_CongThucChiTiet>> Post(MPG_CongThucChiTiet mPG_CongThucChiTiet)
        {
          if (_context.MPG_CongThucChiTiet == null)
          {
              return Problem("Entity set 'dbPMScontext.MPG_CongThucChiTiet'  is null.");
          }
            if (Vm.VmMPG_CongThucChiTiet.Exists(mPG_CongThucChiTiet))
            {
                return Conflict();
            }
            Vm.VmMPG_CongThucChiTiet.Insert_Command.Execute(mPG_CongThucChiTiet);
            return CreatedAtAction("Get", new { id = mPG_CongThucChiTiet.Ma }, mPG_CongThucChiTiet);
        }

        // DELETE: api/MPG_CongThucChiTiet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MPG_CongThucChiTiet == null)
            {
                return NotFound();
            }
            var mPG_CongThucChiTiet = Vm.VmMPG_CongThucChiTiet.Find(id);
            if (mPG_CongThucChiTiet == null)
            {
                return NotFound();
            }

            Vm.VmMPG_CongThucChiTiet.Delete_Command.Execute(mPG_CongThucChiTiet);

            return NoContent();
        }

        private bool MPG_CongThucChiTietExists(string id)
        {
            return (_context.MPG_CongThucChiTiet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
