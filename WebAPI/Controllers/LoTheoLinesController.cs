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
    public class LoTheoLinesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public LoTheoLinesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/LoTheoLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoTheoLine>>> Gets()
        {
          if (_context.LoTheoLine == null)
          {
              return NotFound();
          }
            return Vm.VmLoTheoLine.Items;
        }

        // GET: api/LoTheoLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoTheoLine>> Get(int id)
        {
          if (_context.LoTheoLine == null)
          {
              return NotFound();
          }
            var loTheoLine = Vm.VmLoTheoLine.Find(id);

            if (loTheoLine == null)
            {
                return NotFound();
            }

            return loTheoLine;
        }

        // PUT: api/LoTheoLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, LoTheoLine loTheoLine)
        {
            if (id != loTheoLine.Id)
            {
                return BadRequest();
            }

            if (!Vm.VmLoTheoLine.Exists(loTheoLine))
            {
                return NotFound();
            }
            Vm.VmLoTheoLine.Update_Command.Execute(loTheoLine);
            return NoContent();
        }

        // POST: api/LoTheoLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoTheoLine>> Post(LoTheoLine loTheoLine)
        {
          if (_context.LoTheoLine == null)
          {
              return Problem("Entity set 'dbPMScontext.LoTheoLine'  is null.");
          }
            if (Vm.VmLoTheoLine.Exists(loTheoLine))
            {
                return Conflict();
            }
            Vm.VmLoTheoLine.Insert_Command.Execute(loTheoLine);

            return CreatedAtAction("GetLoTheoLine", new { id = loTheoLine.Id }, loTheoLine);
        }

        // DELETE: api/LoTheoLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.LoTheoLine == null)
            {
                return NotFound();
            }
            var loTheoLine = Vm.VmLoTheoLine.Find(id);
            if (loTheoLine == null)
            {
                return NotFound();
            }

            Vm.VmLoTheoLine.Delete_Command.Execute(loTheoLine);

            return NoContent();
        }

        private bool LoTheoLineExists(int id)
        {
            return (_context.LoTheoLine?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
