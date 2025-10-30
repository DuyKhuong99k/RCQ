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
    public class KNH_PhieuCanController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KNH_PhieuCanController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KNH_PhieuCan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KNH_PhieuCan>>> Gets()
        {
          if (_context.KNH_PhieuCan == null)
          {
              return NotFound();
          }
            return Vm.VmKNH_PhieuCan.Items;
        }

        // GET: api/KNH_PhieuCan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KNH_PhieuCan>> Get(int id)
        {
          if (_context.KNH_PhieuCan == null)
          {
              return NotFound();
          }
            var kNH_PhieuCan = Vm.VmKNH_PhieuCan.Find(id);

            if (kNH_PhieuCan == null)
            {
                return NotFound();
            }

            return kNH_PhieuCan;
        }

        // PUT: api/KNH_PhieuCan/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, KNH_PhieuCan kNH_PhieuCan)
        {
            if (id != kNH_PhieuCan.STT)
            {
                return BadRequest();
            }

            if (!Vm.VmKNH_PhieuCan.Exists(kNH_PhieuCan))
            {
                return NotFound();
            }
            Vm.VmKNH_PhieuCan.Update_Command.Execute(kNH_PhieuCan);
            return NoContent();
        }

        // POST: api/KNH_PhieuCan
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KNH_PhieuCan>> Post(KNH_PhieuCan kNH_PhieuCan)
        {
          if (_context.KNH_PhieuCan == null)
          {
              return Problem("Entity set 'dbPMScontext.KNH_PhieuCan'  is null.");
          }
            if (Vm.VmKNH_PhieuCan.Exists(kNH_PhieuCan))
            {
                return Conflict();
            }
            Vm.VmKNH_PhieuCan.Insert_Command.Execute(kNH_PhieuCan);
            return CreatedAtAction("Get", new { id = kNH_PhieuCan.STT }, kNH_PhieuCan);
        }

        // DELETE: api/KNH_PhieuCan/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.KNH_PhieuCan == null)
            {
                return NotFound();
            }
            var kNH_PhieuCan = Vm.VmKNH_PhieuCan.Find(id);
            if (kNH_PhieuCan == null)
            {
                return NotFound();
            }

            Vm.VmKNH_PhieuCan.Delete_Command.Execute(kNH_PhieuCan);

            return NoContent();
        }

        private bool KNH_PhieuCanExists(int id)
        {
            return (_context.KNH_PhieuCan?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
