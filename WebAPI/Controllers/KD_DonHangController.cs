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
    public class KD_DonHangController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public KD_DonHangController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/KD_DonHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KD_DonHang>>> Gets()
        {
          if (_context.KD_DonHang == null)
          {
              return NotFound();
          }
            return Vm.VmKD_DonHang.Items;
        }

        // GET: api/KD_DonHang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KD_DonHang>> Get(string id)
        {
          if (_context.KD_DonHang == null)
          {
              return NotFound();
          }
            var kD_DonHang = Vm.VmKD_DonHang.Find(id);

            if (kD_DonHang == null)
            {
                return NotFound();
            }

            return kD_DonHang;
        }

        // PUT: api/KD_DonHang/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, KD_DonHang kD_DonHang)
        {
            if (id != kD_DonHang.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKD_DonHang.Exists(kD_DonHang))
            {
                return NotFound();
            }
            Vm.VmKD_DonHang.Update_Command.Execute(kD_DonHang);
            return NoContent();
        }

        // POST: api/KD_DonHang
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KD_DonHang>> Post(KD_DonHang kD_DonHang)
        {
          if (_context.KD_DonHang == null)
          {
              return Problem("Entity set 'dbPMScontext.KD_DonHang'  is null.");
          }
            if (Vm.VmKD_DonHang.Exists(kD_DonHang))
            {
                return Conflict();
            }
            Vm.VmKD_DonHang.Insert_Command.Execute(kD_DonHang);
            return CreatedAtAction("Get", new { id = kD_DonHang.Ma }, kD_DonHang);
        }

        // DELETE: api/KD_DonHang/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.KD_DonHang == null)
            {
                return NotFound();
            }
            var kD_DonHang = Vm.VmKD_DonHang.Find(id);
            if (kD_DonHang == null)
            {
                return NotFound();
            }

            Vm.VmKD_DonHang.Delete_Command.Execute(kD_DonHang);

            return NoContent();
        }

        private bool KD_DonHangExists(string id)
        {
            return (_context.KD_DonHang?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
