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
    public class RP_MaThanhPhamNLController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public RP_MaThanhPhamNLController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/RP_MaThanhPhamNL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RP_MaThanhPhamNL>>> GetRP_MaThanhPhamNL()
        {
          if (_context.RP_MaThanhPhamNL == null)
          {
              return NotFound();
          }
            return await _context.RP_MaThanhPhamNL.ToListAsync();
        }

        // GET: api/RP_MaThanhPhamNL/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RP_MaThanhPhamNL>> GetRP_MaThanhPhamNL(string id)
        {
          if (_context.RP_MaThanhPhamNL == null)
          {
              return NotFound();
          }
            var rP_MaThanhPhamNL = await _context.RP_MaThanhPhamNL.FindAsync(id);

            if (rP_MaThanhPhamNL == null)
            {
                return NotFound();
            }

            return rP_MaThanhPhamNL;
        }

        // PUT: api/RP_MaThanhPhamNL/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRP_MaThanhPhamNL(string id, RP_MaThanhPhamNL rP_MaThanhPhamNL)
        {
            if (id != rP_MaThanhPhamNL.MaThanhPhamNL)
            {
                return BadRequest();
            }

            _context.Entry(rP_MaThanhPhamNL).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RP_MaThanhPhamNLExists(id))
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

        // POST: api/RP_MaThanhPhamNL
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RP_MaThanhPhamNL>> PostRP_MaThanhPhamNL(RP_MaThanhPhamNL rP_MaThanhPhamNL)
        {
          if (_context.RP_MaThanhPhamNL == null)
          {
              return Problem("Entity set 'dbPMScontext.RP_MaThanhPhamNL'  is null.");
          }
            _context.RP_MaThanhPhamNL.Add(rP_MaThanhPhamNL);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RP_MaThanhPhamNLExists(rP_MaThanhPhamNL.MaThanhPhamNL))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRP_MaThanhPhamNL", new { id = rP_MaThanhPhamNL.MaThanhPhamNL }, rP_MaThanhPhamNL);
        }

        // DELETE: api/RP_MaThanhPhamNL/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRP_MaThanhPhamNL(string id)
        {
            if (_context.RP_MaThanhPhamNL == null)
            {
                return NotFound();
            }
            var rP_MaThanhPhamNL = await _context.RP_MaThanhPhamNL.FindAsync(id);
            if (rP_MaThanhPhamNL == null)
            {
                return NotFound();
            }

            _context.RP_MaThanhPhamNL.Remove(rP_MaThanhPhamNL);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RP_MaThanhPhamNLExists(string id)
        {
            return (_context.RP_MaThanhPhamNL?.Any(e => e.MaThanhPhamNL == id)).GetValueOrDefault();
        }
    }
}
