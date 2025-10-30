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
    public class T_NhaCungCap1Controller : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_NhaCungCap1Controller(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/T_NhaCungCap1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_NhaCungCap>>> GetT_NhaCungCap()
        {
          if (_context.T_NhaCungCap == null)
          {
              return NotFound();
          }
            return await _context.T_NhaCungCap.ToListAsync();
        }

        // GET: api/T_NhaCungCap1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_NhaCungCap>> GetT_NhaCungCap(string id)
        {
          if (_context.T_NhaCungCap == null)
          {
              return NotFound();
          }
            var t_NhaCungCap = await _context.T_NhaCungCap.FindAsync(id);

            if (t_NhaCungCap == null)
            {
                return NotFound();
            }

            return t_NhaCungCap;
        }

        // PUT: api/T_NhaCungCap1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutT_NhaCungCap(string id, T_NhaCungCap t_NhaCungCap)
        {
            if (id != t_NhaCungCap.Ma)
            {
                return BadRequest();
            }

            _context.Entry(t_NhaCungCap).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!T_NhaCungCapExists(id))
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

        // POST: api/T_NhaCungCap1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_NhaCungCap>> PostT_NhaCungCap(T_NhaCungCap t_NhaCungCap)
        {
          if (_context.T_NhaCungCap == null)
          {
              return Problem("Entity set 'dbPMScontext.T_NhaCungCap'  is null.");
          }
            _context.T_NhaCungCap.Add(t_NhaCungCap);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (T_NhaCungCapExists(t_NhaCungCap.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetT_NhaCungCap", new { id = t_NhaCungCap.Ma }, t_NhaCungCap);
        }

        // DELETE: api/T_NhaCungCap1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteT_NhaCungCap(string id)
        {
            if (_context.T_NhaCungCap == null)
            {
                return NotFound();
            }
            var t_NhaCungCap = await _context.T_NhaCungCap.FindAsync(id);
            if (t_NhaCungCap == null)
            {
                return NotFound();
            }

            _context.T_NhaCungCap.Remove(t_NhaCungCap);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool T_NhaCungCapExists(string id)
        {
            return (_context.T_NhaCungCap?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
