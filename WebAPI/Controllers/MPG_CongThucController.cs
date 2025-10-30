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
    public class MPG_CongThucController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MPG_CongThucController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MPG_CongThuc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MPG_CongThuc>>> Gets()
        {
          if (_context.MPG_CongThuc == null)
          {
              return NotFound();
          }
            return Vm.VmMPG_CongThuc.Items;
        }

        // GET: api/MPG_CongThuc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MPG_CongThuc>> Get(string id)
        {
          if (_context.MPG_CongThuc == null)
          {
              return NotFound();
          }
            var mPG_CongThuc = Vm.VmMPG_CongThuc.Find(id);

            if (mPG_CongThuc == null)
            {
                return NotFound();
            }

            return mPG_CongThuc;
        }

        // PUT: api/MPG_CongThuc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MPG_CongThuc mPG_CongThuc)
        {
            if (id != mPG_CongThuc.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmMPG_CongThuc.Exists(mPG_CongThuc))
            {
                return NotFound();
            }
            Vm.VmMPG_CongThuc.Update_Command.Execute(mPG_CongThuc);
            return NoContent();
        }

        // POST: api/MPG_CongThuc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MPG_CongThuc>> Post(MPG_CongThuc mPG_CongThuc)
        {
          if (_context.MPG_CongThuc == null)
          {
              return Problem("Entity set 'dbPMScontext.MPG_CongThuc'  is null.");
          }
            if (Vm.VmMPG_CongThuc.Exists(mPG_CongThuc))
            {
                return Conflict();
            }
            Vm.VmMPG_CongThuc.Insert_Command.Execute(mPG_CongThuc);
            return CreatedAtAction("Get", new { id = mPG_CongThuc.Ma }, mPG_CongThuc);
        }

        // DELETE: api/MPG_CongThuc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MPG_CongThuc == null)
            {
                return NotFound();
            }
            var mPG_CongThuc = Vm.VmMPG_CongThuc.Find(id);
            if (mPG_CongThuc == null)
            {
                return NotFound();
            }

            Vm.VmMPG_CongThuc.Delete_Command.Execute(mPG_CongThuc);

            return NoContent();
        }

        private bool MPG_CongThucExists(string id)
        {
            return (_context.MPG_CongThuc?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
