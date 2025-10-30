using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;

namespace WebAPI.Controllers
{
     [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanPhaLocsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanPhaLocsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PhieuCanPhaLocs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanPhaLoc>>> GetPhieuCanPhaLoc()
        {
          if (_context.PhieuCanPhaLoc == null)
          {
              return NotFound();
          }
            return await _context.PhieuCanPhaLoc.ToListAsync();
        }

        // GET: api/PhieuCanPhaLocs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanPhaLoc>> GetPhieuCanPhaLoc(string id)
        {
          if (_context.PhieuCanPhaLoc == null)
          {
              return NotFound();
          }
            var phieuCanPhaLoc = await _context.PhieuCanPhaLoc.FindAsync(id);

            if (phieuCanPhaLoc == null)
            {
                return NotFound();
            }

            return phieuCanPhaLoc;
        }

        // PUT: api/PhieuCanPhaLocs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanPhaLoc(string id, PhieuCanPhaLoc phieuCanPhaLoc)
        {
            if (id != phieuCanPhaLoc.MaMayTinhCan)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanPhaLoc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanPhaLocExists(id))
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

        // POST: api/PhieuCanPhaLocs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanPhaLoc>> PostPhieuCanPhaLoc(PhieuCanPhaLoc phieuCanPhaLoc)
        {
          if (_context.PhieuCanPhaLoc == null)
          {
              return Problem("Entity set 'dbPMScontext.PhieuCanPhaLoc'  is null.");
          }
            _context.PhieuCanPhaLoc.Add(phieuCanPhaLoc);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanPhaLocExists(phieuCanPhaLoc.MaMayTinhCan))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanPhaLoc", new { id = phieuCanPhaLoc.MaMayTinhCan }, phieuCanPhaLoc);
        }

        // DELETE: api/PhieuCanPhaLocs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanPhaLoc(string id)
        {
            if (_context.PhieuCanPhaLoc == null)
            {
                return NotFound();
            }
            var phieuCanPhaLoc = await _context.PhieuCanPhaLoc.FindAsync(id);
            if (phieuCanPhaLoc == null)
            {
                return NotFound();
            }

            _context.PhieuCanPhaLoc.Remove(phieuCanPhaLoc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanPhaLocExists(string id)
        {
            return (_context.PhieuCanPhaLoc?.Any(e => e.MaMayTinhCan == id)).GetValueOrDefault();
        }
    }
}
