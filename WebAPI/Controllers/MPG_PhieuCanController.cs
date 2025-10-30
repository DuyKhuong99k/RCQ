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
    public class MPG_PhieuCanController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MPG_PhieuCanController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MPG_PhieuCan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MPG_PhieuCan>>> Gets()
        {
          if (_context.MPG_PhieuCan == null)
          {
              return NotFound();
          }
            return Vm.VmMPG_PhieuCan.Items;
        }

        // GET: api/MPG_PhieuCan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MPG_PhieuCan>> Get(int id)
        {
          if (_context.MPG_PhieuCan == null)
          {
              return NotFound();
          }
            var mPG_PhieuCan = Vm.VmMPG_PhieuCan.Find(id);

            if (mPG_PhieuCan == null)
            {
                return NotFound();
            }

            return mPG_PhieuCan;
        }

        // PUT: api/MPG_PhieuCan/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, MPG_PhieuCan mPG_PhieuCan)
        {
            if (id != mPG_PhieuCan.STT)
            {
                return BadRequest();
            }

            if(!Vm.VmMPG_PhieuCan.Exists(mPG_PhieuCan))
            {
                return NotFound();
            }
            Vm.VmMPG_PhieuCan.Update_Command.Execute(mPG_PhieuCan);
            return NoContent();
        }

        // POST: api/MPG_PhieuCan
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MPG_PhieuCan>> Post(MPG_PhieuCan mPG_PhieuCan)
        {
          if (_context.MPG_PhieuCan == null)
          {
              return Problem("Entity set 'dbPMScontext.MPG_PhieuCan'  is null.");
          }
            if (!Vm.VmMPG_PhieuCan.Exists(mPG_PhieuCan))
            {
                return Conflict();
            }
            Vm.VmMPG_PhieuCan.Insert_Command.Execute(mPG_PhieuCan);
            return CreatedAtAction("Get", new { id = mPG_PhieuCan.STT }, mPG_PhieuCan);
        }

        // DELETE: api/MPG_PhieuCan/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.MPG_PhieuCan == null)
            {
                return NotFound();
            }
            var mPG_PhieuCan = Vm.VmMPG_PhieuCan.Find(id);
            if (mPG_PhieuCan == null)
            {
                return NotFound();
            }

            Vm.VmMPG_PhieuCan.Delete_Command.Execute(mPG_PhieuCan);

            return NoContent();
        }

        private bool MPG_PhieuCanExists(int id)
        {
            return (_context.MPG_PhieuCan?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
