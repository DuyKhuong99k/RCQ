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
    public class TheRoesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public TheRoesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/TheRoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheRo>>> Gets()
        {
          if (_context.TheRo == null)
          {
              return NotFound();
          }
            return Vm.VmTheRo.Items;
        }

        // GET: api/TheRoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TheRo>> Get(string id)
        {
          if (_context.TheRo == null)
          {
              return NotFound();
          }
            var theRo = Vm.VmTheRo.Find(id);

            if (theRo == null)
            {
                return NotFound();
            }

            return theRo;
        }

        // PUT: api/TheRoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, TheRo theRo)
        {
            if (id != theRo.MaThe)
            {
                return BadRequest();
            }

            if (!Vm.VmTheRo.Exists(theRo))
            {
                return NotFound();
            }
            Vm.VmTheRo.Update_Command.Execute(theRo);
            return NoContent();
        }

        // POST: api/TheRoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TheRo>> Post(TheRo theRo)
        {
          if (_context.TheRo == null)
          {
              return Problem("Entity set 'dbPMScontext.TheRo'  is null.");
          }
            if (Vm.VmTheRo.Exists(theRo))
            {
                return Conflict();
            }
            Vm.VmTheRo.Insert_Command.Execute(theRo);
            return CreatedAtAction("Get", new { id = theRo.MaThe }, theRo);
        }

        // DELETE: api/TheRoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.TheRo == null)
            {
                return NotFound();
            }
            var theRo = Vm.VmTheRo.Find(id);
            if (theRo == null)
            {
                return NotFound();
            }

            Vm.VmTheRo.Delete_Command.Execute(theRo);

            return NoContent();
        }

        private bool TheRoExists(string id)
        {
            return (_context.TheRo?.Any(e => e.MaThe == id)).GetValueOrDefault();
        }
    }
}
