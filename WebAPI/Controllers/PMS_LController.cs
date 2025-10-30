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
    public class PMS_LController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PMS_LController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PMS_L
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PMS_L>>> Gets()
        {
          if (_context.PMS_L == null)
          {
              return NotFound();
          }
            return Vm.VmPMS_L.Items;
        }

        // GET: api/PMS_L/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PMS_L>> Get(int id)
        {
          if (_context.PMS_L == null)
          {
              return NotFound();
          }
            var pMS_L = Vm.VmPMS_L.Find(id);

            if (pMS_L == null)
            {
                return NotFound();
            }

            return pMS_L;
        }

        // PUT: api/PMS_L/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PMS_L pMS_L)
        {
            if (id != pMS_L.STT)
            {
                return BadRequest();
            }

            if (!Vm.VmPMS_L.Exists(pMS_L))
            {
                return NotFound();
            }
            Vm.VmPMS_L.Update_Command.Execute(pMS_L);
            return NoContent();
        }

        // POST: api/PMS_L
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PMS_L>> Post(PMS_L pMS_L)
        {
          if (_context.PMS_L == null)
          {
              return Problem("Entity set 'dbPMScontext.PMS_L'  is null.");
          }
            if (Vm.VmPMS_L.Exists(pMS_L))
            {
                return Conflict();
            }
            Vm.VmPMS_L.Insert_Command.Execute(pMS_L);
            return CreatedAtAction("Get", new { id = pMS_L.STT }, pMS_L);
        }

        // DELETE: api/PMS_L/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.PMS_L == null)
            {
                return NotFound();
            }
            var pMS_L = Vm.VmPMS_L.Find(id);
            if (pMS_L == null)
            {
                return NotFound();
            }

            Vm.VmPMS_L.Delete_Command.Execute(pMS_L);

            return NoContent();
        }

        private bool PMS_LExists(int id)
        {
            return (_context.PMS_L?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
