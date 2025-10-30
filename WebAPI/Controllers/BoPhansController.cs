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
    public class BoPhansController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BoPhansController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/BoPhans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoPhan>>> Gets()
        {
          if (_context.BoPhan == null)
          {
              return NotFound();
          }
            return await _context.BoPhan.ToListAsync();
        }

        // GET: api/BoPhans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoPhan>> Get(string id)
        {
          if (_context.BoPhan == null)
          {
              return NotFound();
          }
            var boPhan = await _context.BoPhan.FindAsync(id);

            if (boPhan == null)
            {
                return NotFound();
            }

            return boPhan;
        }

        // PUT: api/BoPhans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, BoPhan boPhan)
        {
            if (id != boPhan.Ma)
            {
                return BadRequest();
            }

            _context.Entry(boPhan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoPhanExists(id))
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

        // POST: api/BoPhans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BoPhan>> Post(BoPhan boPhan)
        {
          if (_context.BoPhan == null)
          {
              return Problem("Entity set 'dbPMScontext.BoPhan'  is null.");
          }
            _context.BoPhan.Add(boPhan);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BoPhanExists(boPhan.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBoPhan", new { id = boPhan.Ma }, boPhan);
        }

        // DELETE: api/BoPhans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.BoPhan == null)
            {
                return NotFound();
            }
            var boPhan = await _context.BoPhan.FindAsync(id);
            if (boPhan == null)
            {
                return NotFound();
            }

            _context.BoPhan.Remove(boPhan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoPhanExists(string id)
        {
            return (_context.BoPhan?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
