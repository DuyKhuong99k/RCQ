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
    public class MaKhachHangXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaKhachHangXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaKhachHangXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaKhachHangXepKhuon>>> Gets()
        {
          if (_context.MaKhachHangXepKhuon == null)
          {
              return NotFound();
          }
            return Vm.VmKhachHangXepKhuon.Items;
        }

        // GET: api/MaKhachHangXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaKhachHangXepKhuon>> Get(string id)
        {
          if (_context.MaKhachHangXepKhuon == null)
          {
              return NotFound();
          }
            var maKhachHangXepKhuon = Vm.VmKhachHangXepKhuon.Find(id);

            if (maKhachHangXepKhuon == null)
            {
                return NotFound();
            }

            return maKhachHangXepKhuon;
        }

        // PUT: api/MaKhachHangXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaKhachHangXepKhuon maKhachHangXepKhuon)
        {
            if (id != maKhachHangXepKhuon.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmKhachHangXepKhuon.Exists(maKhachHangXepKhuon))
            {
                return NotFound();
            }
            Vm.VmKhachHangXepKhuon.Update_Command.Execute(maKhachHangXepKhuon);
            return NoContent();
        }

        // POST: api/MaKhachHangXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaKhachHangXepKhuon>> Post(MaKhachHangXepKhuon maKhachHangXepKhuon)
        {
          if (_context.MaKhachHangXepKhuon == null)
          {
              return Problem("Entity set 'dbPMScontext.MaKhachHangXepKhuon'  is null.");
          }
            if (Vm.VmKhachHangXepKhuon.Exists(maKhachHangXepKhuon))
            {
                return Conflict();
            }
            Vm.VmKhachHangXepKhuon.Insert_Command.Execute(maKhachHangXepKhuon);
            return CreatedAtAction("Get", new { id = maKhachHangXepKhuon.Ma }, maKhachHangXepKhuon);
        }

        // DELETE: api/MaKhachHangXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaKhachHangXepKhuon == null)
            {
                return NotFound();
            }
            var maKhachHangXepKhuon = Vm.VmKhachHangXepKhuon.Find(id);
            if (maKhachHangXepKhuon == null)
            {
                return NotFound();
            }

            Vm.VmKhachHangXepKhuon.Delete_Command.Execute(maKhachHangXepKhuon);

            return NoContent();
        }

        private bool MaKhachHangXepKhuonExists(string id)
        {
            return (_context.MaKhachHangXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaKhachHangXepKhuon.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
    }
}
