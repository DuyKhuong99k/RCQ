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
    public class ColorCodesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public ColorCodesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/ColorCodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColorCode>>> Gets()
        {
          if (_context.ColorCode == null)
          {
              return NotFound();
          }
            return Vm.VmColorCode.Items;
        }

        // GET: api/ColorCodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ColorCode>> Get(string id)
        {
          if (_context.ColorCode == null)
          {
              return NotFound();
          }
            var colorCode = Vm.VmColorCode.Find(id);

            if (colorCode == null)
            {
                return NotFound();
            }

            return colorCode;
        }

        // PUT: api/ColorCodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, ColorCode colorCode)
        {
            if (id != colorCode.Code)
            {
                return BadRequest();
            }
            if (!Vm.VmColorCode.Exists(colorCode))
            {
                return NotFound();
            }
            Vm.VmColorCode.Update_Command.Execute(colorCode);
            return NoContent();
        }

        // POST: api/ColorCodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ColorCode>> Post(ColorCode colorCode)
        {
          if (_context.ColorCode == null)
          {
              return Problem("Entity set 'dbPMScontext.ColorCode'  is null.");
          }
            if (Vm.VmColorCode.Exists(colorCode))
            {
                return Conflict();
            }
            Vm.VmColorCode.Insert_Command.Execute(colorCode);
            return CreatedAtAction("Get", new { id = colorCode.Code }, colorCode);
        }

        // DELETE: api/ColorCodes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.ColorCode == null)
            {
                return NotFound();
            }
            var colorCode = Vm.VmColorCode.Find(id);
            if (colorCode == null)
            {
                return NotFound();
            }

           Vm.VmColorCode.Delete_Command.Execute(colorCode);

            return NoContent();
        }

        private bool ColorCodeExists(string id)
        {
            return (_context.ColorCode?.Any(e => e.Code == id)).GetValueOrDefault();
        }
    }
}
