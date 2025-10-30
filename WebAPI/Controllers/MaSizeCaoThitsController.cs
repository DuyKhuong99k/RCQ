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
    public class MaSizeCaoThitsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaSizeCaoThitsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaSizeCaoThits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaSizeCaoThit>>> Gets()
        {
          if (_context.MaSizeCaoThit == null)
          {
              return NotFound();
          }
            return Vm.VmSizeCaoThit.Items;
        }

        // GET: api/MaSizeCaoThits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaSizeCaoThit>> Get(string id)
        {
          if (_context.MaSizeCaoThit == null)
          {
              return NotFound();
          }
            var maSizeCaoThit = Vm.VmSizeCaoThit.Find(id);

            if (maSizeCaoThit == null)
            {
                return NotFound();
            }

            return maSizeCaoThit;
        }

        // PUT: api/MaSizeCaoThits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaSizeCaoThit maSizeCaoThit)
        {
            if (id != maSizeCaoThit.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmSizeCaoThit.Exists(maSizeCaoThit))
            {
                return NotFound();
            }
            Vm.VmSizeCaoThit.Update_Command.Execute(maSizeCaoThit);
            return NoContent();
        }

        // POST: api/MaSizeCaoThits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaSizeCaoThit>> Post(MaSizeCaoThit maSizeCaoThit)
        {
          if (_context.MaSizeCaoThit == null)
          {
              return Problem("Entity set 'dbPMScontext.MaSizeCaoThit'  is null.");
          }
            if (Vm.VmSizeCaoThit.Exists(maSizeCaoThit))
            {
                return Conflict();
            }
            Vm.VmSizeCaoThit.Insert_Command.Execute(maSizeCaoThit);
            return CreatedAtAction("Get", new { id = maSizeCaoThit.Ma }, maSizeCaoThit);
        }

        // DELETE: api/MaSizeCaoThits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaSizeCaoThit == null)
            {
                return NotFound();
            }
            var maSizeCaoThit = Vm.VmSizeCaoThit.Find(id);
            if (maSizeCaoThit == null)
            {
                return NotFound();
            }

            Vm.VmSizeCaoThit.Delete_Command.Execute(maSizeCaoThit);

            return NoContent();
        }

        private bool MaSizeCaoThitExists(string id)
        {
            return (_context.MaSizeCaoThit?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
