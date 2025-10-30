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
    public class PD_PhieuNhapChiTietController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PD_PhieuNhapChiTietController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PD_PhieuNhapChiTiet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PD_PhieuNhapChiTiet>>> Gets()
        {
          if (_context.PD_PhieuNhapChiTiet == null)
          {
              return NotFound();
          }
            return await _context.PD_PhieuNhapChiTiet.ToListAsync();
        }

        // GET: api/PD_PhieuNhapChiTiet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PD_PhieuNhapChiTiet>> Get(string id)
        {
          if (_context.PD_PhieuNhapChiTiet == null)
          {
              return NotFound();
          }
            var pD_PhieuNhapChiTiet = await _context.PD_PhieuNhapChiTiet.FindAsync(id);

            if (pD_PhieuNhapChiTiet == null)
            {
                return NotFound();
            }

            return pD_PhieuNhapChiTiet;
        }

        // PUT: api/PD_PhieuNhapChiTiet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, PD_PhieuNhapChiTiet pD_PhieuNhapChiTiet)
        {
            if (id != pD_PhieuNhapChiTiet.SoPhieu)
            {
                return BadRequest();
            }

            _context.Entry(pD_PhieuNhapChiTiet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PD_PhieuNhapChiTietExists(id))
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

        // POST: api/PD_PhieuNhapChiTiet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PD_PhieuNhapChiTiet>> Post(PD_PhieuNhapChiTiet pD_PhieuNhapChiTiet)
        {
          if (_context.PD_PhieuNhapChiTiet == null)
          {
              return Problem("Entity set 'dbPMScontext.PD_PhieuNhapChiTiet'  is null.");
          }
            _context.PD_PhieuNhapChiTiet.Add(pD_PhieuNhapChiTiet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PD_PhieuNhapChiTietExists(pD_PhieuNhapChiTiet.SoPhieu))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = pD_PhieuNhapChiTiet.SoPhieu }, pD_PhieuNhapChiTiet);
        }

        // DELETE: api/PD_PhieuNhapChiTiet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.PD_PhieuNhapChiTiet == null)
            {
                return NotFound();
            }
            var pD_PhieuNhapChiTiet = await _context.PD_PhieuNhapChiTiet.FindAsync(id);
            if (pD_PhieuNhapChiTiet == null)
            {
                return NotFound();
            }

            _context.PD_PhieuNhapChiTiet.Remove(pD_PhieuNhapChiTiet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PD_PhieuNhapChiTietExists(string id)
        {
            return (_context.PD_PhieuNhapChiTiet?.Any(e => e.SoPhieu == id)).GetValueOrDefault();
        }
    }
}
