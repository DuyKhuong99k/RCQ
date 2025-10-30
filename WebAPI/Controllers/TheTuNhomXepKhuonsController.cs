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
    public class TheTuNhomXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public TheTuNhomXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/TheTuNhomXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheTuNhomXepKhuon>>> Gets()
        {
          if (_context.TheTuNhomXepKhuon == null)
          {
              return NotFound();
          }
            return await _context.TheTuNhomXepKhuon.ToListAsync();
        }

        // GET: api/TheTuNhomXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TheTuNhomXepKhuon>> Get(string id)
        {
          if (_context.TheTuNhomXepKhuon == null)
          {
              return NotFound();
          }
            var theTuNhomXepKhuon = await _context.TheTuNhomXepKhuon.FindAsync(id);

            if (theTuNhomXepKhuon == null)
            {
                return NotFound();
            }

            return theTuNhomXepKhuon;
        }

        // PUT: api/TheTuNhomXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, TheTuNhomXepKhuon theTuNhomXepKhuon)
        {
            if (id != theTuNhomXepKhuon.MaTheTu)
            {
                return BadRequest();
            }

            _context.Entry(theTuNhomXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TheTuNhomXepKhuonExists(id))
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

        // POST: api/TheTuNhomXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TheTuNhomXepKhuon>> Post(TheTuNhomXepKhuon theTuNhomXepKhuon)
        {
          if (_context.TheTuNhomXepKhuon == null)
          {
              return Problem("Entity set 'dbPMScontext.TheTuNhomXepKhuon'  is null.");
          }
            _context.TheTuNhomXepKhuon.Add(theTuNhomXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TheTuNhomXepKhuonExists(theTuNhomXepKhuon.MaTheTu))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTheTuNhomXepKhuon", new { id = theTuNhomXepKhuon.MaTheTu }, theTuNhomXepKhuon);
        }

        // DELETE: api/TheTuNhomXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.TheTuNhomXepKhuon == null)
            {
                return NotFound();
            }
            var theTuNhomXepKhuon = await _context.TheTuNhomXepKhuon.FindAsync(id);
            if (theTuNhomXepKhuon == null)
            {
                return NotFound();
            }

            _context.TheTuNhomXepKhuon.Remove(theTuNhomXepKhuon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TheTuNhomXepKhuonExists(string id)
        {
            return (_context.TheTuNhomXepKhuon?.Any(e => e.MaTheTu == id)).GetValueOrDefault();
        }
    }
}
