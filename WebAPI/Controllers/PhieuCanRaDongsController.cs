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
    public class PhieuCanRaDongsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanRaDongsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PhieuCanRaDongs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanRaDong>>> GetPhieuCanRaDong()
        {
          if (_context.PhieuCanRaDong == null)
          {
              return NotFound();
          }
            return await _context.PhieuCanRaDong.ToListAsync();
        }

        // GET: api/PhieuCanRaDongs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanRaDong>> GetPhieuCanRaDong(int id)
        {
          if (_context.PhieuCanRaDong == null)
          {
              return NotFound();
          }
            var phieuCanRaDong = await _context.PhieuCanRaDong.FindAsync(id);

            if (phieuCanRaDong == null)
            {
                return NotFound();
            }

            return phieuCanRaDong;
        }

        // PUT: api/PhieuCanRaDongs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanRaDong(int id, PhieuCanRaDong phieuCanRaDong)
        {
            if (id != phieuCanRaDong.STT)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanRaDong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanRaDongExists(id))
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

        // POST: api/PhieuCanRaDongs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanRaDong>> PostPhieuCanRaDong(PhieuCanRaDong phieuCanRaDong)
        {
          if (_context.PhieuCanRaDong == null)
          {
              return Problem("Entity set 'dbPMScontext.PhieuCanRaDong'  is null.");
          }
            _context.PhieuCanRaDong.Add(phieuCanRaDong);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanRaDongExists(phieuCanRaDong.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanRaDong", new { id = phieuCanRaDong.STT }, phieuCanRaDong);
        }

        // DELETE: api/PhieuCanRaDongs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanRaDong(int id)
        {
            if (_context.PhieuCanRaDong == null)
            {
                return NotFound();
            }
            var phieuCanRaDong = await _context.PhieuCanRaDong.FindAsync(id);
            if (phieuCanRaDong == null)
            {
                return NotFound();
            }

            _context.PhieuCanRaDong.Remove(phieuCanRaDong);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanRaDongExists(int id)
        {
            return (_context.PhieuCanRaDong?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
