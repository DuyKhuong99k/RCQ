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
    public class MaLoaiCaTaiChesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoaiCaTaiChesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLoaiCaTaiChes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaTaiChe>>> Gets()
        {
            if (_context.MaLoaiCaTaiChe == null)
            {
                return NotFound();
            }
            return Vm.VmLoaiCaTaiChe.Items;
        }

        // GET: api/MaLoaiCaTaiChes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaTaiChe>> Get(string id)
        {
            if (_context.MaLoaiCaTaiChe == null)
            {
                return NotFound();
            }
            var maLoaiCaTaiChe = Vm.VmLoaiCaTaiChe.Find(id);

            if (maLoaiCaTaiChe == null)
            {
                return NotFound();
            }

            return maLoaiCaTaiChe;
        }

        // PUT: api/MaLoaiCaTaiChes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaLoaiCaTaiChe maLoaiCaTaiChe)
        {
            if (id != maLoaiCaTaiChe.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmLoaiCaTaiChe.Exists(maLoaiCaTaiChe))
            {
                return NotFound();
            }
            Vm.VmLoaiCaTaiChe.Update_Command.Execute(maLoaiCaTaiChe);
            return NoContent();
        }

        // POST: api/MaLoaiCaTaiChes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaTaiChe>> Post(MaLoaiCaTaiChe maLoaiCaTaiChe)
        {
            if (_context.MaLoaiCaTaiChe == null)
            {
                return Problem("Entity set 'dbPMScontext.MaLoaiCaTaiChe'  is null.");
            }
            if (Vm.VmLoaiCaTaiChe.Exists(maLoaiCaTaiChe))
            {
                return Conflict();
            }
            Vm.VmLoaiCaTaiChe.Insert_Command.Execute(maLoaiCaTaiChe);
            return CreatedAtAction("Get", new { id = maLoaiCaTaiChe.Ma }, maLoaiCaTaiChe);
        }

        // DELETE: api/MaLoaiCaTaiChes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoaiCaTaiChe == null)
            {
                return NotFound();
            }
            var maLoaiCaTaiChe = Vm.VmLoaiCaTaiChe.Find(id);
            if (maLoaiCaTaiChe == null)
            {
                return NotFound();
            }

            Vm.VmLoaiCaTaiChe.Delete_Command.Execute(maLoaiCaTaiChe);

            return NoContent();
        }

        private bool MaLoaiCaTaiCheExists(string id)
        {
            return (_context.MaLoaiCaTaiChe?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaLoaiCaTaiChe.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
    }
}
