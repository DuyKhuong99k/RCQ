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
    public class PD_PhieuNhapController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PD_PhieuNhapController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PD_PhieuNhap
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PD_PhieuNhap>>> Gets()
        {
          if (_context.PD_PhieuNhap == null)
          {
              return NotFound();
          }
            return await _context.PD_PhieuNhap.ToListAsync();
        }

        // GET: api/PD_PhieuNhap/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PD_PhieuNhap>> Get(int id)
        {
          if (_context.PD_PhieuNhap == null)
          {
              return NotFound();
          }
            var pD_PhieuNhap = await _context.PD_PhieuNhap.FindAsync(id);

            if (pD_PhieuNhap == null)
            {
                return NotFound();
            }

            return pD_PhieuNhap;
        }

        // PUT: api/PD_PhieuNhap/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PD_PhieuNhap pD_PhieuNhap)
        {
            if (id != pD_PhieuNhap.STT)
            {
                return BadRequest();
            }

            _context.Entry(pD_PhieuNhap).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PD_PhieuNhapExists(id))
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

        // POST: api/PD_PhieuNhap
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PD_PhieuNhap>> Post(PD_PhieuNhap pD_PhieuNhap)
        {
          if (_context.PD_PhieuNhap == null)
          {
              return Problem("Entity set 'dbPMScontext.PD_PhieuNhap'  is null.");
          }
            _context.PD_PhieuNhap.Add(pD_PhieuNhap);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PD_PhieuNhapExists(pD_PhieuNhap.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = pD_PhieuNhap.STT }, pD_PhieuNhap);
        }

        // DELETE: api/PD_PhieuNhap/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.PD_PhieuNhap == null)
            {
                return NotFound();
            }
            var pD_PhieuNhap = await _context.PD_PhieuNhap.FindAsync(id);
            if (pD_PhieuNhap == null)
            {
                return NotFound();
            }

            _context.PD_PhieuNhap.Remove(pD_PhieuNhap);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PD_PhieuNhapExists(int id)
        {
            return (_context.PD_PhieuNhap?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
