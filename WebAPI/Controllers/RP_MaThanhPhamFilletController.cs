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
    public class RP_MaThanhPhamFilletController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public RP_MaThanhPhamFilletController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/RP_MaThanhPhamFillet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RP_MaThanhPhamFillet>>> GetRP_MaThanhPhamFillet()
        {
          if (_context.RP_MaThanhPhamFillet == null)
          {
              return NotFound();
          }
            return await _context.RP_MaThanhPhamFillet.ToListAsync();
        }

        // GET: api/RP_MaThanhPhamFillet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RP_MaThanhPhamFillet>> GetRP_MaThanhPhamFillet(string id)
        {
          if (_context.RP_MaThanhPhamFillet == null)
          {
              return NotFound();
          }
            var rP_MaThanhPhamFillet = await _context.RP_MaThanhPhamFillet.FindAsync(id);

            if (rP_MaThanhPhamFillet == null)
            {
                return NotFound();
            }

            return rP_MaThanhPhamFillet;
        }

        // PUT: api/RP_MaThanhPhamFillet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRP_MaThanhPhamFillet(string id, RP_MaThanhPhamFillet rP_MaThanhPhamFillet)
        {
            if (id != rP_MaThanhPhamFillet.Ma)
            {
                return BadRequest();
            }

            _context.Entry(rP_MaThanhPhamFillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RP_MaThanhPhamFilletExists(id))
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

        // POST: api/RP_MaThanhPhamFillet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RP_MaThanhPhamFillet>> PostRP_MaThanhPhamFillet(RP_MaThanhPhamFillet rP_MaThanhPhamFillet)
        {
          if (_context.RP_MaThanhPhamFillet == null)
          {
              return Problem("Entity set 'dbPMScontext.RP_MaThanhPhamFillet'  is null.");
          }
            _context.RP_MaThanhPhamFillet.Add(rP_MaThanhPhamFillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RP_MaThanhPhamFilletExists(rP_MaThanhPhamFillet.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRP_MaThanhPhamFillet", new { id = rP_MaThanhPhamFillet.Ma }, rP_MaThanhPhamFillet);
        }

        // DELETE: api/RP_MaThanhPhamFillet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRP_MaThanhPhamFillet(string id)
        {
            if (_context.RP_MaThanhPhamFillet == null)
            {
                return NotFound();
            }
            var rP_MaThanhPhamFillet = await _context.RP_MaThanhPhamFillet.FindAsync(id);
            if (rP_MaThanhPhamFillet == null)
            {
                return NotFound();
            }

            _context.RP_MaThanhPhamFillet.Remove(rP_MaThanhPhamFillet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RP_MaThanhPhamFilletExists(string id)
        {
            return (_context.RP_MaThanhPhamFillet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
