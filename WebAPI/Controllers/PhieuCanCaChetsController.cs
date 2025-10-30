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
    public class PhieuCanCaChetsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanCaChetsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PhieuCanCaChets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanCaChet>>> GetPhieuCanCaChet()
        {
          if (_context.PhieuCanCaChet == null)
          {
              return NotFound();
          }
            return await _context.PhieuCanCaChet.ToListAsync();
        }

        // GET: api/PhieuCanCaChets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanCaChet>> GetPhieuCanCaChet(int id)
        {
          if (_context.PhieuCanCaChet == null)
          {
              return NotFound();
          }
            var phieuCanCaChet = await _context.PhieuCanCaChet.FindAsync(id);

            if (phieuCanCaChet == null)
            {
                return NotFound();
            }

            return phieuCanCaChet;
        }

        // PUT: api/PhieuCanCaChets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanCaChet(int id, PhieuCanCaChet phieuCanCaChet)
        {
            if (id != phieuCanCaChet.STT)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanCaChet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanCaChetExists(id))
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

        // POST: api/PhieuCanCaChets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanCaChet>> PostPhieuCanCaChet(PhieuCanCaChet phieuCanCaChet)
        {
          if (_context.PhieuCanCaChet == null)
          {
              return Problem("Entity set 'dbPMScontext.PhieuCanCaChet'  is null.");
          }
            _context.PhieuCanCaChet.Add(phieuCanCaChet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanCaChetExists(phieuCanCaChet.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanCaChet", new { id = phieuCanCaChet.STT }, phieuCanCaChet);
        }

        // DELETE: api/PhieuCanCaChets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanCaChet(int id)
        {
            if (_context.PhieuCanCaChet == null)
            {
                return NotFound();
            }
            var phieuCanCaChet = await _context.PhieuCanCaChet.FindAsync(id);
            if (phieuCanCaChet == null)
            {
                return NotFound();
            }

            _context.PhieuCanCaChet.Remove(phieuCanCaChet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanCaChetExists(int id)
        {
            return (_context.PhieuCanCaChet?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
