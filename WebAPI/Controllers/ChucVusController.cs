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
    public class ChucVusController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public ChucVusController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/ChucVus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChucVu>>> Gets()
        {
          if (_context.ChucVu == null)
          {
              return NotFound();
          }
            return await _context.ChucVu.ToListAsync();
        }

        // GET: api/ChucVus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChucVu>> GetChucVu(string id)
        {
          if (_context.ChucVu == null)
          {
              return NotFound();
          }
            var chucVu = await _context.ChucVu.FindAsync(id);

            if (chucVu == null)
            {
                return NotFound();
            }

            return chucVu;
        }

        // PUT: api/ChucVus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChucVu(string id, ChucVu chucVu)
        {
            if (id != chucVu.Ma)
            {
                return BadRequest();
            }

            _context.Entry(chucVu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChucVuExists(id))
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

        // POST: api/ChucVus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChucVu>> PostChucVu(ChucVu chucVu)
        {
          if (_context.ChucVu == null)
          {
              return Problem("Entity set 'dbPMScontext.ChucVu'  is null.");
          }
            _context.ChucVu.Add(chucVu);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ChucVuExists(chucVu.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetChucVu", new { id = chucVu.Ma }, chucVu);
        }

        // DELETE: api/ChucVus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChucVu(string id)
        {
            if (_context.ChucVu == null)
            {
                return NotFound();
            }
            var chucVu = await _context.ChucVu.FindAsync(id);
            if (chucVu == null)
            {
                return NotFound();
            }

            _context.ChucVu.Remove(chucVu);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChucVuExists(string id)
        {
            return (_context.ChucVu?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
