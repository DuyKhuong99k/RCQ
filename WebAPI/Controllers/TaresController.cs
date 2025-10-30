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
    public class TaresController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public TaresController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/Tares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tare>>> Gets()
        {
          if (_context.Tare == null)
          {
              return NotFound();
          }
            return Vm.VmTare.Items;
        }

        // GET: api/Tares/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tare>> Get(decimal id)
        {
          if (_context.Tare == null)
          {
              return NotFound();
          }
            var tare = Vm.VmTare.Find(id);

            if (tare == null)
            {
                return NotFound();
            }

            return tare;
        }

        // PUT: api/Tares/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(decimal id, Tare tare)
        {
            if (id != tare.TrongLuong)
            {
                return BadRequest();
            }

            if (!Vm.VmTare.Exists(tare))
            {
                return NotFound();
            }
            Vm.VmTare.Update_Command.Execute(tare);
            return NoContent();
        }

        // POST: api/Tares
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tare>> Post(Tare tare)
        {
          if (_context.Tare == null)
          {
              return Problem("Entity set 'dbPMScontext.Tare'  is null.");
          }
            if (Vm.VmTare.Exists(tare))
            {
                return Conflict();
            }
            Vm.VmTare.Insert_Command.Execute(tare);
            return CreatedAtAction("Get", new { id = tare.TrongLuong }, tare);
        }

        // DELETE: api/Tares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            if (_context.Tare == null)
            {
                return NotFound();
            }
            var tare = Vm.VmTare.Find(id);
            if (tare == null)
            {
                return NotFound();
            }

            Vm.VmTare.Delete_Command.Execute(tare);

            return NoContent();
        }

        private bool TareExists(decimal id)
        {
            return (_context.Tare?.Any(e => e.TrongLuong == id)).GetValueOrDefault();
        }
    }
}
