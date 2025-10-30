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
    public class PhieuCanCaoThitsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanCaoThitsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PhieuCanCaoThits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanCaoThit>>> GetPhieuCanCaoThit()
        {
          if (_context.PhieuCanCaoThit == null)
          {
              return NotFound();
          }
            return await _context.PhieuCanCaoThit.ToListAsync();
        }

        // GET: api/PhieuCanCaoThits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanCaoThit>> GetPhieuCanCaoThit(int id)
        {
          if (_context.PhieuCanCaoThit == null)
          {
              return NotFound();
          }
            var phieuCanCaoThit = await _context.PhieuCanCaoThit.FindAsync(id);

            if (phieuCanCaoThit == null)
            {
                return NotFound();
            }

            return phieuCanCaoThit;
        }

        // PUT: api/PhieuCanCaoThits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanCaoThit(int id, PhieuCanCaoThit phieuCanCaoThit)
        {
            if (id != phieuCanCaoThit.STT)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanCaoThit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanCaoThitExists(id))
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

        // POST: api/PhieuCanCaoThits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanCaoThit>> PostPhieuCanCaoThit(PhieuCanCaoThit phieuCanCaoThit)
        {
          if (_context.PhieuCanCaoThit == null)
          {
              return Problem("Entity set 'dbPMScontext.PhieuCanCaoThit'  is null.");
          }
            _context.PhieuCanCaoThit.Add(phieuCanCaoThit);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanCaoThitExists(phieuCanCaoThit.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanCaoThit", new { id = phieuCanCaoThit.STT }, phieuCanCaoThit);
        }

        // DELETE: api/PhieuCanCaoThits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanCaoThit(int id)
        {
            if (_context.PhieuCanCaoThit == null)
            {
                return NotFound();
            }
            var phieuCanCaoThit = await _context.PhieuCanCaoThit.FindAsync(id);
            if (phieuCanCaoThit == null)
            {
                return NotFound();
            }

            _context.PhieuCanCaoThit.Remove(phieuCanCaoThit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanCaoThitExists(int id)
        {
            return (_context.PhieuCanCaoThit?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
