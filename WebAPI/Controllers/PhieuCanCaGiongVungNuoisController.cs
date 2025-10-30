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
    public class PhieuCanCaGiongVungNuoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanCaGiongVungNuoisController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PhieuCanCaGiongVungNuois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanCaGiongVungNuoi>>> GetPhieuCanCaGiongVungNuoi()
        {
          if (_context.PhieuCanCaGiongVungNuoi == null)
          {
              return NotFound();
          }
            return await _context.PhieuCanCaGiongVungNuoi.ToListAsync();
        }

        // GET: api/PhieuCanCaGiongVungNuois/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanCaGiongVungNuoi>> GetPhieuCanCaGiongVungNuoi(int id)
        {
          if (_context.PhieuCanCaGiongVungNuoi == null)
          {
              return NotFound();
          }
            var phieuCanCaGiongVungNuoi = await _context.PhieuCanCaGiongVungNuoi.FindAsync(id);

            if (phieuCanCaGiongVungNuoi == null)
            {
                return NotFound();
            }

            return phieuCanCaGiongVungNuoi;
        }

        // PUT: api/PhieuCanCaGiongVungNuois/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanCaGiongVungNuoi(int id, PhieuCanCaGiongVungNuoi phieuCanCaGiongVungNuoi)
        {
            if (id != phieuCanCaGiongVungNuoi.STT)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanCaGiongVungNuoi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanCaGiongVungNuoiExists(id))
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

        // POST: api/PhieuCanCaGiongVungNuois
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanCaGiongVungNuoi>> PostPhieuCanCaGiongVungNuoi(PhieuCanCaGiongVungNuoi phieuCanCaGiongVungNuoi)
        {
          if (_context.PhieuCanCaGiongVungNuoi == null)
          {
              return Problem("Entity set 'dbPMScontext.PhieuCanCaGiongVungNuoi'  is null.");
          }
            _context.PhieuCanCaGiongVungNuoi.Add(phieuCanCaGiongVungNuoi);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanCaGiongVungNuoiExists(phieuCanCaGiongVungNuoi.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanCaGiongVungNuoi", new { id = phieuCanCaGiongVungNuoi.STT }, phieuCanCaGiongVungNuoi);
        }

        // DELETE: api/PhieuCanCaGiongVungNuois/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanCaGiongVungNuoi(int id)
        {
            if (_context.PhieuCanCaGiongVungNuoi == null)
            {
                return NotFound();
            }
            var phieuCanCaGiongVungNuoi = await _context.PhieuCanCaGiongVungNuoi.FindAsync(id);
            if (phieuCanCaGiongVungNuoi == null)
            {
                return NotFound();
            }

            _context.PhieuCanCaGiongVungNuoi.Remove(phieuCanCaGiongVungNuoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanCaGiongVungNuoiExists(int id)
        {
            return (_context.PhieuCanCaGiongVungNuoi?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
