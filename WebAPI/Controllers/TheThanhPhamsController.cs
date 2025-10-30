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
    public class TheThanhPhamsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public TheThanhPhamsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/TheThanhPhams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheThanhPham>>> Gets()
        {
          if (_context.TheThanhPham == null)
          {
              return NotFound();
          }
            return Vm.VmTheThanhPham.Items;
        }

        // GET: api/TheThanhPhams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TheThanhPham>> Get(string id)
        {
          if (_context.TheThanhPham == null)
          {
              return NotFound();
          }
            var theThanhPham = Vm.VmTheThanhPham.Find(id);

            if (theThanhPham == null)
            {
                return NotFound();
            }

            return theThanhPham;
        }

        // PUT: api/TheThanhPhams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, TheThanhPham theThanhPham)
        {
            if (id != theThanhPham.MaThe)
            {
                return BadRequest();
            }

            if (!Vm.VmTheThanhPham.Exists(theThanhPham))
            {
                return NotFound();
            }
            Vm.VmTheThanhPham.Update_Command.Execute(theThanhPham);
            return NoContent();
        }

        // POST: api/TheThanhPhams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TheThanhPham>> Post(TheThanhPham theThanhPham)
        {
          if (_context.TheThanhPham == null)
          {
              return Problem("Entity set 'dbPMScontext.TheThanhPham'  is null.");
          }
            if (Vm.VmTheThanhPham.Exists(theThanhPham))
            {
                return Conflict();
            }
            Vm.VmTheThanhPham.Insert_Command.Execute(theThanhPham);
            return CreatedAtAction("Get", new { id = theThanhPham.MaThe }, theThanhPham);
        }

        // DELETE: api/TheThanhPhams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.TheThanhPham == null)
            {
                return NotFound();
            }
            var theThanhPham = Vm.VmTheThanhPham.Find(id);
            if (theThanhPham == null)
            {
                return NotFound();
            }

            Vm.VmTheThanhPham.Delete_Command.Execute(theThanhPham);

            return NoContent();
        }

        private bool TheThanhPhamExists(string id)
        {
            return (_context.TheThanhPham?.Any(e => e.MaThe == id)).GetValueOrDefault();
        }
    }
}
