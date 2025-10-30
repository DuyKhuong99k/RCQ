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
    public class PhieuCanVungNuoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanVungNuoisController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PhieuCanVungNuois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanVungNuoi>>> GetPhieuCanVungNuoi()
        {
          if (_context.PhieuCanVungNuoi == null)
          {
              return NotFound();
          }
            return await _context.PhieuCanVungNuoi.ToListAsync();
        }

        // GET: api/PhieuCanVungNuois/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanVungNuoi>> GetPhieuCanVungNuoi(int id)
        {
          if (_context.PhieuCanVungNuoi == null)
          {
              return NotFound();
          }
            var phieuCanVungNuoi = await _context.PhieuCanVungNuoi.FindAsync(id);

            if (phieuCanVungNuoi == null)
            {
                return NotFound();
            }

            return phieuCanVungNuoi;
        }

        // PUT: api/PhieuCanVungNuois/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanVungNuoi(int id, PhieuCanVungNuoi phieuCanVungNuoi)
        {
            if (id != phieuCanVungNuoi.STT)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanVungNuoi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanVungNuoiExists(id))
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

        // POST: api/PhieuCanVungNuois
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanVungNuoi>> PostPhieuCanVungNuoi(PhieuCanVungNuoi phieuCanVungNuoi)
        {
          if (_context.PhieuCanVungNuoi == null)
          {
              return Problem("Entity set 'dbPMScontext.PhieuCanVungNuoi'  is null.");
          }
            _context.PhieuCanVungNuoi.Add(phieuCanVungNuoi);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanVungNuoiExists(phieuCanVungNuoi.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanVungNuoi", new { id = phieuCanVungNuoi.STT }, phieuCanVungNuoi);
        }

        // DELETE: api/PhieuCanVungNuois/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanVungNuoi(int id)
        {
            if (_context.PhieuCanVungNuoi == null)
            {
                return NotFound();
            }
            var phieuCanVungNuoi = await _context.PhieuCanVungNuoi.FindAsync(id);
            if (phieuCanVungNuoi == null)
            {
                return NotFound();
            }

            _context.PhieuCanVungNuoi.Remove(phieuCanVungNuoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanVungNuoiExists(int id)
        {
            return (_context.PhieuCanVungNuoi?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
