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
    public class LoaiCaXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public LoaiCaXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/LoaiCaXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaXepKhuon>>> Get()
        {
          if (_context.MaLoaiCaXepKhuon == null)
          {
              return NotFound();
          }
            return Vm.VmLoaiCaXepKhuon.Items;
        }

        // GET: api/LoaiCaXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaXepKhuon>> Get(string id)
        {
          if (_context.MaLoaiCaXepKhuon == null)
          {
              return NotFound();
          }
            var loaiCaXepKhuon = Vm.VmLoaiCaXepKhuon.Find(id);

            if (loaiCaXepKhuon == null)
            {
                return NotFound();
            }

            return loaiCaXepKhuon;
        }

        // PUT: api/LoaiCaXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaLoaiCaXepKhuon loaiCaXepKhuon)
        {
            if (id != loaiCaXepKhuon.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmLoaiCaXepKhuon.Exists(loaiCaXepKhuon))
            {
                return NotFound();
            }
            Vm.VmLoaiCaXepKhuon.Update_Command.Execute(loaiCaXepKhuon);
            return NoContent();
        }

        // POST: api/LoaiCaXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaXepKhuon>> Post(MaLoaiCaXepKhuon loaiCaXepKhuon)
        {
          if (_context.MaLoaiCaXepKhuon == null)
          {
              return Problem("Entity set 'dbPMScontext.LoaiCaXepKhuon'  is null.");
          }
            if (Vm.VmLoaiCaXepKhuon.Exists(loaiCaXepKhuon))
            {
                return Conflict();
            }
            Vm.VmLoaiCaXepKhuon.Insert_Command.Execute(loaiCaXepKhuon);
            return CreatedAtAction("Get", new { id = loaiCaXepKhuon.Ma }, loaiCaXepKhuon);
        }

        // DELETE: api/LoaiCaXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoaiCaXepKhuon == null)
            {
                return NotFound();
            }
            var loaiCaXepKhuon = Vm.VmLoaiCaXepKhuon.Find(id);
            if (loaiCaXepKhuon == null)
            {
                return NotFound();
            }

            Vm.VmLoaiCaXepKhuon.Delete_Command.Execute(loaiCaXepKhuon);

            return NoContent();
        }

        private bool LoaiCaXepKhuonExists(string id)
        {
            return (_context.MaLoaiCaXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
