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
    public class PD_PhieuXuatController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PD_PhieuXuatController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PD_PhieuXuat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PD_PhieuXuat>>> Gets()
        {
          if (_context.PD_PhieuXuat == null)
          {
              return NotFound();
          }
            return await _context.PD_PhieuXuat.ToListAsync();
        }

        // GET: api/PD_PhieuXuat/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PD_PhieuXuat>> Get(int id)
        {
          if (_context.PD_PhieuXuat == null)
          {
              return NotFound();
          }
            var pD_PhieuXuat = await _context.PD_PhieuXuat.FindAsync(id);

            if (pD_PhieuXuat == null)
            {
                return NotFound();
            }

            return pD_PhieuXuat;
        }

        // PUT: api/PD_PhieuXuat/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PD_PhieuXuat pD_PhieuXuat)
        {
            if (id != pD_PhieuXuat.STT)
            {
                return BadRequest();
            }

            _context.Entry(pD_PhieuXuat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PD_PhieuXuatExists(id))
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

        // POST: api/PD_PhieuXuat
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PD_PhieuXuat>> Post(PD_PhieuXuat pD_PhieuXuat)
        {
          if (_context.PD_PhieuXuat == null)
          {
              return Problem("Entity set 'dbPMScontext.PD_PhieuXuat'  is null.");
          }
            _context.PD_PhieuXuat.Add(pD_PhieuXuat);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PD_PhieuXuatExists(pD_PhieuXuat.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPD_PhieuXuat", new { id = pD_PhieuXuat.STT }, pD_PhieuXuat);
        }

        // DELETE: api/PD_PhieuXuat/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.PD_PhieuXuat == null)
            {
                return NotFound();
            }
            var pD_PhieuXuat = await _context.PD_PhieuXuat.FindAsync(id);
            if (pD_PhieuXuat == null)
            {
                return NotFound();
            }

            _context.PD_PhieuXuat.Remove(pD_PhieuXuat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PD_PhieuXuatExists(int id)
        {
            return (_context.PD_PhieuXuat?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
