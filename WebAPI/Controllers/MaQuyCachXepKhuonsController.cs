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
    public class MaQuyCachXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaQuyCachXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaQuyCachXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaQuyCachXepKhuon>>> Gets()
        {
          if (_context.MaQuyCachXepKhuon == null)
          {
              return NotFound();
          }
            return await _context.MaQuyCachXepKhuon.ToListAsync();
        }

        // GET: api/MaQuyCachXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaQuyCachXepKhuon>> Get(string id)
        {
          if (_context.MaQuyCachXepKhuon == null)
          {
              return NotFound();
          }
            var maQuyCachXepKhuon = await _context.MaQuyCachXepKhuon.FindAsync(id);

            if (maQuyCachXepKhuon == null)
            {
                return NotFound();
            }

            return maQuyCachXepKhuon;
        }

        // PUT: api/MaQuyCachXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaQuyCachXepKhuon maQuyCachXepKhuon)
        {
            if (id != maQuyCachXepKhuon.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maQuyCachXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaQuyCachXepKhuonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MaQuyCachXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaQuyCachXepKhuon>> Post(MaQuyCachXepKhuon maQuyCachXepKhuon)
        {
          if (_context.MaQuyCachXepKhuon == null)
          {
              return Problem("Entity set 'dbPMScontext.MaQuyCachXepKhuon'  is null.");
          }
            _context.MaQuyCachXepKhuon.Add(maQuyCachXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaQuyCachXepKhuonExists(maQuyCachXepKhuon.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maQuyCachXepKhuon.Ma }, maQuyCachXepKhuon);
        }

        // DELETE: api/MaQuyCachXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaQuyCachXepKhuon == null)
            {
                return NotFound();
            }
            var maQuyCachXepKhuon = await _context.MaQuyCachXepKhuon.FindAsync(id);
            if (maQuyCachXepKhuon == null)
            {
                return NotFound();
            }

            _context.MaQuyCachXepKhuon.Remove(maQuyCachXepKhuon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaQuyCachXepKhuonExists(string id)
        {
            return (_context.MaQuyCachXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
