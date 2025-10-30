using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class MaLoaiCaXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoaiCaXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLoaiCaXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaXepKhuon>>> Gets()
        {
            if (_context.MaLoaiCaXepKhuon == null)
            {
                return NotFound();
            }
            return Vm.VmLoaiCaXepKhuon.Items;
        }

        // GET: api/MaLoaiCaXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaXepKhuon>> Get(string id)
        {
            if (_context.MaLoaiCaXepKhuon == null)
            {
                return NotFound();
            }
            var maLoaiCaXepKhuon = Vm.VmLoaiCaXepKhuon.Find(id);

            if (maLoaiCaXepKhuon == null)
            {
                return NotFound();
            }

            return maLoaiCaXepKhuon;
        }

        // PUT: api/MaLoaiCaXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaLoaiCaXepKhuon maLoaiCaXepKhuon)
        {
            if (id != maLoaiCaXepKhuon.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmLoaiCaXepKhuon.Exists(maLoaiCaXepKhuon))
            {
                return NotFound(id);
            }
            Vm.VmLoaiCaXepKhuon.Update_Command.Execute(maLoaiCaXepKhuon);
            return NoContent();
        }

        // POST: api/MaLoaiCaXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaXepKhuon>> Post(MaLoaiCaXepKhuon maLoaiCaXepKhuon)
        {
            if (_context.MaLoaiCaXepKhuon == null)
            {
                return Problem("Entity set 'dbPMScontext.MaLoaiCaXepKhuon'  is null.");
            }
            if (Vm.VmLoaiCaXepKhuon.Exists(maLoaiCaXepKhuon))
            {
                return Conflict();
            }
            Vm.VmLoaiCaXepKhuon.Insert_Command.Execute(maLoaiCaXepKhuon);
            return CreatedAtAction("Get", new { id = maLoaiCaXepKhuon.Ma }, maLoaiCaXepKhuon);
        }

        // DELETE: api/MaLoaiCaXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoaiCaXepKhuon == null)
            {
                return NotFound();
            }
            var maLoaiCaXepKhuon = Vm.VmLoaiCaXepKhuon.Find(id);
            if (maLoaiCaXepKhuon == null)
            {
                return NotFound();
            }

            Vm.VmLoaiCaXepKhuon.Delete_Command.Execute(maLoaiCaXepKhuon);

            return NoContent();
        }

        private bool MaLoaiCaXepKhuonExists(string id)
        {
            return (_context.MaLoaiCaXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaLoaiCaXepKhuon.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
    }
}
