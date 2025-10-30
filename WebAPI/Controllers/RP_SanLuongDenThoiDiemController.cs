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
    public class RP_SanLuongDenThoiDiemController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public RP_SanLuongDenThoiDiemController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/RP_SanLuongDenThoiDiem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RP_SanLuongDenThoiDiem>>> GetRP_SanLuongDenThoiDiem()
        {
          if (_context.RP_SanLuongDenThoiDiem == null)
          {
              return NotFound();
          }
            return await _context.RP_SanLuongDenThoiDiem.ToListAsync();
        }

        // GET: api/RP_SanLuongDenThoiDiem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RP_SanLuongDenThoiDiem>> GetRP_SanLuongDenThoiDiem(string id)
        {
          if (_context.RP_SanLuongDenThoiDiem == null)
          {
              return NotFound();
          }
            var rP_SanLuongDenThoiDiem = await _context.RP_SanLuongDenThoiDiem.FindAsync(id);

            if (rP_SanLuongDenThoiDiem == null)
            {
                return NotFound();
            }

            return rP_SanLuongDenThoiDiem;
        }

        // PUT: api/RP_SanLuongDenThoiDiem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRP_SanLuongDenThoiDiem(string id, RP_SanLuongDenThoiDiem rP_SanLuongDenThoiDiem)
        {
            if (id != rP_SanLuongDenThoiDiem.KhuVuc)
            {
                return BadRequest();
            }

            _context.Entry(rP_SanLuongDenThoiDiem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RP_SanLuongDenThoiDiemExists(id))
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

        // POST: api/RP_SanLuongDenThoiDiem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RP_SanLuongDenThoiDiem>> PostRP_SanLuongDenThoiDiem(RP_SanLuongDenThoiDiem rP_SanLuongDenThoiDiem)
        {
          if (_context.RP_SanLuongDenThoiDiem == null)
          {
              return Problem("Entity set 'dbPMScontext.RP_SanLuongDenThoiDiem'  is null.");
          }
            _context.RP_SanLuongDenThoiDiem.Add(rP_SanLuongDenThoiDiem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RP_SanLuongDenThoiDiemExists(rP_SanLuongDenThoiDiem.KhuVuc))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRP_SanLuongDenThoiDiem", new { id = rP_SanLuongDenThoiDiem.KhuVuc }, rP_SanLuongDenThoiDiem);
        }

        // DELETE: api/RP_SanLuongDenThoiDiem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRP_SanLuongDenThoiDiem(string id)
        {
            if (_context.RP_SanLuongDenThoiDiem == null)
            {
                return NotFound();
            }
            var rP_SanLuongDenThoiDiem = await _context.RP_SanLuongDenThoiDiem.FindAsync(id);
            if (rP_SanLuongDenThoiDiem == null)
            {
                return NotFound();
            }

            _context.RP_SanLuongDenThoiDiem.Remove(rP_SanLuongDenThoiDiem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RP_SanLuongDenThoiDiemExists(string id)
        {
            return (_context.RP_SanLuongDenThoiDiem?.Any(e => e.KhuVuc == id)).GetValueOrDefault();
        }
    }
}
