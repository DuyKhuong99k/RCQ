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
    public class LoNguyenLieuxController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public LoNguyenLieuxController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/LoNguyenLieux
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoNguyenLieu>>> Get()
        {
          if (_context.LoNguyenLieu == null)
          {
              return NotFound();
          }
            return await _context.LoNguyenLieu.ToListAsync();
        }

        // GET: api/LoNguyenLieux/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoNguyenLieu>> GetLoNguyenLieu(string id)
        {
          if (_context.LoNguyenLieu == null)
          {
              return NotFound();
          }
            var loNguyenLieu = await _context.LoNguyenLieu.FindAsync(id);

            if (loNguyenLieu == null)
            {
                return NotFound();
            }

            return loNguyenLieu;
        }

        // PUT: api/LoNguyenLieux/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoNguyenLieu(string id, LoNguyenLieu loNguyenLieu)
        {
            if (id != loNguyenLieu.Id)
            {
                return BadRequest();
            }

            _context.Entry(loNguyenLieu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoNguyenLieuExists(id))
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

        // POST: api/LoNguyenLieux
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoNguyenLieu>> PostLoNguyenLieu(LoNguyenLieu loNguyenLieu)
        {
          if (_context.LoNguyenLieu == null)
          {
              return Problem("Entity set 'dbPMScontext.LoNguyenLieu'  is null.");
          }
            _context.LoNguyenLieu.Add(loNguyenLieu);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LoNguyenLieuExists(loNguyenLieu.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLoNguyenLieu", new { id = loNguyenLieu.Id }, loNguyenLieu);
        }

        // DELETE: api/LoNguyenLieux/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoNguyenLieu(string id)
        {
            if (_context.LoNguyenLieu == null)
            {
                return NotFound();
            }
            var loNguyenLieu = await _context.LoNguyenLieu.FindAsync(id);
            if (loNguyenLieu == null)
            {
                return NotFound();
            }

            _context.LoNguyenLieu.Remove(loNguyenLieu);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoNguyenLieuExists(string id)
        {
            return (_context.LoNguyenLieu?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
