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
    public class RP_NangXuatDinhMucController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public RP_NangXuatDinhMucController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/RP_NangXuatDinhMuc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RP_NangXuatDinhMuc>>> GetRP_NangXuatDinhMuc()
        {
          if (_context.RP_NangXuatDinhMuc == null)
          {
              return NotFound();
          }
            return await _context.RP_NangXuatDinhMuc.ToListAsync();
        }

        // GET: api/RP_NangXuatDinhMuc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RP_NangXuatDinhMuc>> GetRP_NangXuatDinhMuc(string id)
        {
          if (_context.RP_NangXuatDinhMuc == null)
          {
              return NotFound();
          }
            var rP_NangXuatDinhMuc = await _context.RP_NangXuatDinhMuc.FindAsync(id);

            if (rP_NangXuatDinhMuc == null)
            {
                return NotFound();
            }

            return rP_NangXuatDinhMuc;
        }

        // PUT: api/RP_NangXuatDinhMuc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRP_NangXuatDinhMuc(string id, RP_NangXuatDinhMuc rP_NangXuatDinhMuc)
        {
            if (id != rP_NangXuatDinhMuc.MaXuong)
            {
                return BadRequest();
            }

            _context.Entry(rP_NangXuatDinhMuc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RP_NangXuatDinhMucExists(id))
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

        // POST: api/RP_NangXuatDinhMuc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RP_NangXuatDinhMuc>> PostRP_NangXuatDinhMuc(RP_NangXuatDinhMuc rP_NangXuatDinhMuc)
        {
          if (_context.RP_NangXuatDinhMuc == null)
          {
              return Problem("Entity set 'dbPMScontext.RP_NangXuatDinhMuc'  is null.");
          }
            _context.RP_NangXuatDinhMuc.Add(rP_NangXuatDinhMuc);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RP_NangXuatDinhMucExists(rP_NangXuatDinhMuc.MaXuong))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRP_NangXuatDinhMuc", new { id = rP_NangXuatDinhMuc.MaXuong }, rP_NangXuatDinhMuc);
        }

        // DELETE: api/RP_NangXuatDinhMuc/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRP_NangXuatDinhMuc(string id)
        {
            if (_context.RP_NangXuatDinhMuc == null)
            {
                return NotFound();
            }
            var rP_NangXuatDinhMuc = await _context.RP_NangXuatDinhMuc.FindAsync(id);
            if (rP_NangXuatDinhMuc == null)
            {
                return NotFound();
            }

            _context.RP_NangXuatDinhMuc.Remove(rP_NangXuatDinhMuc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RP_NangXuatDinhMucExists(string id)
        {
            return (_context.RP_NangXuatDinhMuc?.Any(e => e.MaXuong == id)).GetValueOrDefault();
        }
    }
}
