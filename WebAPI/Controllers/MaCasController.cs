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
    public class MaCasController : ControllerBase
    {
        private readonly dbPMScontext _context;
        private MainViewModel Vm => MainViewModel.Instance;
        public MaCasController(dbPMScontext context)
        {
            _context = context;
        }
        // GET: api/MaCas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaCa>>> Gets()
        {
            if (_context.MaCa == null)
            {
                return NotFound();
            }
            return Vm.VmCa.Items;
        }

        // GET: api/MaCas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaCa>> Get(string id)
        {
            if (_context.MaCa == null)
            {
                return NotFound();
            }
            var maCa = Vm.VmCa.Find(id);

            if (maCa == null)
            {
                return NotFound();
            }

            return maCa;
        }

        // PUT: api/MaCas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaCa maCa)
        {
            if (id != maCa.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmCa.Exists(maCa))
            {
                return NotFound();
            }
            Vm.VmCa.Update_Command.Execute(maCa);
            return NoContent();
        }

        // POST: api/MaCas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaCa>> Post(MaCa maCa)
        {
            if (_context.MaCa == null)
            {
                return Problem("Entity set 'dbPMScontext.MaCa'  is null.");
            }
            if (Vm.VmCa.Exists(maCa))
            {
                return Conflict();
            }
            Vm.VmCa.Insert_Command.Execute(maCa);
            return CreatedAtAction("Get", new { id = maCa.Ma }, maCa);
        }

        // DELETE: api/MaCas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaCa == null)
            {
                return NotFound();
            }
            var maCa = Vm.VmCa.Find(id);
            if (maCa == null)
            {
                return NotFound();
            }

            Vm.VmCa.Delete_Command.Execute(maCa);

            return NoContent();
        }

        private bool MaCaExists(string id)
        {
            return (_context.MaCa?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetCas()
        {
            var items = Vm.VmCa.Gets();

            return Ok(items);
        }
    }
}
