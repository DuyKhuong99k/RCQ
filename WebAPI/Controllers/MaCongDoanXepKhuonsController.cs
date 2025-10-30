using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaCongDoanXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaCongDoanXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaCongDoanXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaCongDoanXepKhuon>>> Gets()
        {
            if (_context.MaCongDoanXepKhuon == null)
            {
                return NotFound();
            }
            return Vm.VmCongDoanXepKhuon.Items;
        }

        // GET: api/MaCongDoanXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaCongDoanXepKhuon>> Get(string id)
        {
            if (_context.MaCongDoanXepKhuon == null)
            {
                return NotFound();
            }
            var maCongDoanXepKhuon = Vm.VmCongDoanXepKhuon.Find(id);

            if (maCongDoanXepKhuon == null)
            {
                return NotFound();
            }

            return maCongDoanXepKhuon;
        }

        // PUT: api/MaCongDoanXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaCongDoanXepKhuon maCongDoanXepKhuon)
        {
            if (id != maCongDoanXepKhuon.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmCongDoanXepKhuon.Exists(maCongDoanXepKhuon))
            {
                return NotFound();
            }
            Vm.VmCongDoanXepKhuon.Update_Command.Execute(maCongDoanXepKhuon);
            return NoContent();
        }

        // POST: api/MaCongDoanXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaCongDoanXepKhuon>> Post(MaCongDoanXepKhuon maCongDoanXepKhuon)
        {
            if (_context.MaCongDoanXepKhuon == null)
            {
                return Problem("Entity set 'dbPMScontext.MaCongDoanXepKhuon'  is null.");
            }
            if (Vm.VmCongDoanXepKhuon.Exists(maCongDoanXepKhuon))
            {
                return Conflict();
            }
            Vm.VmCongDoanXepKhuon.Insert_Command.Execute(maCongDoanXepKhuon);
            return CreatedAtAction("GetMaCongDoanXepKhuon", new { id = maCongDoanXepKhuon.Ma }, maCongDoanXepKhuon);
        }

        // DELETE: api/MaCongDoanXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaCongDoanXepKhuon == null)
            {
                return NotFound();
            }
            var maCongDoanXepKhuon = Vm.VmCongDoanXepKhuon.Find(id);
            if (maCongDoanXepKhuon == null)
            {
                return NotFound();
            }

            Vm.VmCongDoanXepKhuon.Delete_Command.Execute(maCongDoanXepKhuon);

            return NoContent();
        }

        private bool MaCongDoanXepKhuonExists(string id)
        {
            return (_context.MaCongDoanXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaCongDoanXepKhuon.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
    }
}
