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
    public class KD_PhieuCanController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KD_PhieuCanController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KD_PhieuCan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KD_PhieuCan>>> Gets()
        {
          if (_context.KD_PhieuCan == null)
          {
              return NotFound();
          }
            return Vm.VmKD_PhieuCan.Items;
        }

        // GET: api/KD_PhieuCan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KD_PhieuCan>> Get(int id)
        {
          if (_context.KD_PhieuCan == null)
          {
              return NotFound();
          }
            var kD_PhieuCan = Vm.VmKD_PhieuCan.Find(id);

            if (kD_PhieuCan == null)
            {
                return NotFound();
            }

            return kD_PhieuCan;
        }

        // PUT: api/KD_PhieuCan/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, KD_PhieuCan kD_PhieuCan)
        {
            if (id != kD_PhieuCan.STT)
            {
                return BadRequest();
            }

            if (!Vm.VmKD_PhieuCan.Exists(kD_PhieuCan))
            {
                return NotFound();
            }
            Vm.VmKD_PhieuCan.Update_Command.Execute(kD_PhieuCan);
            return NoContent();
        }

        // POST: api/KD_PhieuCan
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KD_PhieuCan>> Post(KD_PhieuCan kD_PhieuCan)
        {
          if (_context.KD_PhieuCan == null)
          {
              return Problem("Entity set 'dbPMScontext.KD_PhieuCan'  is null.");
          }
            if (Vm.VmKD_PhieuCan.Exists(kD_PhieuCan))
            {
                return Conflict();
            }
            Vm.VmKD_PhieuCan.Insert_Command.Execute(kD_PhieuCan);
            return CreatedAtAction("Get", new { id = kD_PhieuCan.STT }, kD_PhieuCan);
        }

        // DELETE: api/KD_PhieuCan/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.KD_PhieuCan == null)
            {
                return NotFound();
            }
            var kD_PhieuCan = Vm.VmKD_PhieuCan.Find(id);
            if (kD_PhieuCan == null)
            {
                return NotFound();
            }

            Vm.VmKD_PhieuCan.Delete_Command.Execute(kD_PhieuCan);

            return NoContent();
        }

        private bool KD_PhieuCanExists(int id)
        {
            return (_context.KD_PhieuCan?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
