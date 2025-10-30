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
    public class ToKiemsController : ControllerBase
    {
        private readonly dbPMScontext _context;
        public ToKiemsController(dbPMScontext context)
        {
            _context = context;
            
        }

        // GET: api/ToKiems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToKiem>>> GetToKiem()
        {
          if (_context.ToKiem == null)
          {
              return NotFound();
          }
            return await _context.ToKiem.ToListAsync();
        }

        // GET: api/ToKiems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ToKiem>> GetToKiem(string id)
        {
          if (_context.ToKiem == null)
          {
              return NotFound();
          }
            var toKiem = await _context.ToKiem.FindAsync(id);

            if (toKiem == null)
            {
                return NotFound();
            }

            return toKiem;
        }

        // PUT: api/ToKiems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToKiem(string id, ToKiem toKiem)
        {
            if (id != toKiem.Ma)
            {
                return BadRequest();
            }

            _context.Entry(toKiem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToKiemExists(id))
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

        // POST: api/ToKiems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ToKiem>> PostToKiem(ToKiem toKiem)
        {
          if (_context.ToKiem == null)
          {
              return Problem("Entity set 'dbPMScontext.ToKiem'  is null.");
          }
            _context.ToKiem.Add(toKiem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ToKiemExists(toKiem.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetToKiem", new { id = toKiem.Ma }, toKiem);
        }

        // DELETE: api/ToKiems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToKiem(string id)
        {
            if (_context.ToKiem == null)
            {
                return NotFound();
            }
            var toKiem = await _context.ToKiem.FindAsync(id);
            if (toKiem == null)
            {
                return NotFound();
            }

            _context.ToKiem.Remove(toKiem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToKiemExists(string id)
        {
            return (_context.ToKiem?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
