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
    public class MaThongKeCaChetsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThongKeCaChetsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThongKeCaChets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThongKeCaChet>>> Gets()
        {
          if (_context.MaThongKeCaChet == null)
          {
              return NotFound();
          }
            return Vm.VmThongKeCaChet.Items;
        }

        // GET: api/MaThongKeCaChets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThongKeCaChet>> Get(string id)
        {
          if (_context.MaThongKeCaChet == null)
          {
              return NotFound();
          }
            var maThongKeCaChet = Vm.VmThongKeCaChet.Find(id);

            if (maThongKeCaChet == null)
            {
                return NotFound();
            }

            return maThongKeCaChet;
        }

        // PUT: api/MaThongKeCaChets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThongKeCaChet maThongKeCaChet)
        {
            if (id != maThongKeCaChet.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmThongKeCaChet.Exists(maThongKeCaChet))
            {
                return NotFound();
            }
            Vm.VmThongKeCaChet.Update_Command.Execute(maThongKeCaChet);
            return NoContent();
        }

        // POST: api/MaThongKeCaChets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThongKeCaChet>> Post(MaThongKeCaChet maThongKeCaChet)
        {
          if (_context.MaThongKeCaChet == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThongKeCaChet'  is null.");
          }
            if (Vm.VmThongKeCaChet.Exists(maThongKeCaChet))
            {
                return Conflict();
            }
            Vm.VmThongKeCaChet.Insert_Command.Execute(maThongKeCaChet);
            return CreatedAtAction("Get", new { id = maThongKeCaChet.Ma }, maThongKeCaChet);
        }

        // DELETE: api/MaThongKeCaChets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThongKeCaChet == null)
            {
                return NotFound();
            }
            var maThongKeCaChet = Vm.VmThongKeCaChet.Find(id);
            if (maThongKeCaChet == null)
            {
                return NotFound();
            }

            Vm.VmThongKeCaChet.Delete_Command.Execute(maThongKeCaChet);

            return NoContent();
        }

        private bool MaThongKeCaChetExists(string id)
        {
            return (_context.MaThongKeCaChet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
