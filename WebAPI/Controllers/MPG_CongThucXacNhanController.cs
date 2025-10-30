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
    public class MPG_CongThucXacNhanController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MPG_CongThucXacNhanController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MPG_CongThucXacNhan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MPG_CongThucXacNhan>>> Gets()
        {
          if (_context.MPG_CongThucXacNhan == null)
          {
              return NotFound();
          }
            return Vm.VmMPG_CongThucXacNhan.Items;
        }

        // GET: api/MPG_CongThucXacNhan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MPG_CongThucXacNhan>> Get(string id)
        {
          if (_context.MPG_CongThucXacNhan == null)
          {
              return NotFound();
          }
            var mPG_CongThucXacNhan = Vm.VmMPG_CongThucXacNhan.Find(id);

            if (mPG_CongThucXacNhan == null)
            {
                return NotFound();
            }

            return mPG_CongThucXacNhan;
        }

        // PUT: api/MPG_CongThucXacNhan/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MPG_CongThucXacNhan mPG_CongThucXacNhan)
        {
            if (id != mPG_CongThucXacNhan.MaCongThuc)
            {
                return BadRequest();
            }

            if (!Vm.VmMPG_CongThucXacNhan.Exists(mPG_CongThucXacNhan))
            {
                return NotFound();
            }
            Vm.VmMPG_CongThucXacNhan.Update_Command.Execute(mPG_CongThucXacNhan);
            return NoContent();
        }

        // POST: api/MPG_CongThucXacNhan
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MPG_CongThucXacNhan>> Post(MPG_CongThucXacNhan mPG_CongThucXacNhan)
        {
          if (_context.MPG_CongThucXacNhan == null)
          {
              return Problem("Entity set 'dbPMScontext.MPG_CongThucXacNhan'  is null.");
          }
            if (Vm.VmMPG_CongThucXacNhan.Exists(mPG_CongThucXacNhan))
            {
                return Conflict();
            }
            Vm.VmMPG_CongThucXacNhan.Insert_Command.Execute(mPG_CongThucXacNhan);
            return CreatedAtAction("Get", new { id = mPG_CongThucXacNhan.MaCongThuc }, mPG_CongThucXacNhan);
        }

        // DELETE: api/MPG_CongThucXacNhan/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MPG_CongThucXacNhan == null)
            {
                return NotFound();
            }
            var mPG_CongThucXacNhan = Vm.VmMPG_CongThucXacNhan.Find(id);
            if (mPG_CongThucXacNhan == null)
            {
                return NotFound();
            }

            Vm.VmMPG_CongThucXacNhan.Delete_Command.Execute(mPG_CongThucXacNhan);

            return NoContent();
        }

        private bool MPG_CongThucXacNhanExists(string id)
        {
            return (_context.MPG_CongThucXacNhan?.Any(e => e.MaCongThuc == id)).GetValueOrDefault();
        }
    }
}
