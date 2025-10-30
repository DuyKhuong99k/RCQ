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
    public class GioVaoRaTinhLuongXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public GioVaoRaTinhLuongXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/GioVaoRaTinhLuongXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GioVaoRaTinhLuongXepKhuon>>> GetGioVaoRaTinhLuongXepKhuon()
        {
          if (_context.GioVaoRaTinhLuongXepKhuon == null)
          {
              return NotFound();
          }
            return await _context.GioVaoRaTinhLuongXepKhuon.ToListAsync();
        }

        // GET: api/GioVaoRaTinhLuongXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GioVaoRaTinhLuongXepKhuon>> GetGioVaoRaTinhLuongXepKhuon(int id)
        {
          if (_context.GioVaoRaTinhLuongXepKhuon == null)
          {
              return NotFound();
          }
            var gioVaoRaTinhLuongXepKhuon = await _context.GioVaoRaTinhLuongXepKhuon.FindAsync(id);

            if (gioVaoRaTinhLuongXepKhuon == null)
            {
                return NotFound();
            }

            return gioVaoRaTinhLuongXepKhuon;
        }

        // PUT: api/GioVaoRaTinhLuongXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGioVaoRaTinhLuongXepKhuon(int id, GioVaoRaTinhLuongXepKhuon gioVaoRaTinhLuongXepKhuon)
        {
            if (id != gioVaoRaTinhLuongXepKhuon.STT)
            {
                return BadRequest();
            }

            _context.Entry(gioVaoRaTinhLuongXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GioVaoRaTinhLuongXepKhuonExists(id))
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

        // POST: api/GioVaoRaTinhLuongXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GioVaoRaTinhLuongXepKhuon>> PostGioVaoRaTinhLuongXepKhuon(GioVaoRaTinhLuongXepKhuon gioVaoRaTinhLuongXepKhuon)
        {
          if (_context.GioVaoRaTinhLuongXepKhuon == null)
          {
              return Problem("Entity set 'dbPMScontext.GioVaoRaTinhLuongXepKhuon'  is null.");
          }
            _context.GioVaoRaTinhLuongXepKhuon.Add(gioVaoRaTinhLuongXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GioVaoRaTinhLuongXepKhuonExists(gioVaoRaTinhLuongXepKhuon.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetGioVaoRaTinhLuongXepKhuon", new { id = gioVaoRaTinhLuongXepKhuon.STT }, gioVaoRaTinhLuongXepKhuon);
        }

        // DELETE: api/GioVaoRaTinhLuongXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGioVaoRaTinhLuongXepKhuon(int id)
        {
            if (_context.GioVaoRaTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            var gioVaoRaTinhLuongXepKhuon = await _context.GioVaoRaTinhLuongXepKhuon.FindAsync(id);
            if (gioVaoRaTinhLuongXepKhuon == null)
            {
                return NotFound();
            }

            _context.GioVaoRaTinhLuongXepKhuon.Remove(gioVaoRaTinhLuongXepKhuon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GioVaoRaTinhLuongXepKhuonExists(int id)
        {
            return (_context.GioVaoRaTinhLuongXepKhuon?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
