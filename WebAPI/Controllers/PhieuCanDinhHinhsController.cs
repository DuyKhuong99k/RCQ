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
    public class PhieuCanDinhHinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanDinhHinhsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PhieuCanDinhHinhs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanDinhHinh>>> GetPhieuCanDinhHinh()
        {
          if (_context.PhieuCanDinhHinh == null)
          {
              return NotFound();
          }
            return await _context.PhieuCanDinhHinh.ToListAsync();
        }

        // GET: api/PhieuCanDinhHinhs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanDinhHinh>> GetPhieuCanDinhHinh(int id)
        {
          if (_context.PhieuCanDinhHinh == null)
          {
              return NotFound();
          }
            var phieuCanDinhHinh = await _context.PhieuCanDinhHinh.FindAsync(id);

            if (phieuCanDinhHinh == null)
            {
                return NotFound();
            }

            return phieuCanDinhHinh;
        }

        // PUT: api/PhieuCanDinhHinhs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanDinhHinh(int id, PhieuCanDinhHinh phieuCanDinhHinh)
        {
            if (id != phieuCanDinhHinh.Id)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanDinhHinh).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanDinhHinhExists(id))
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

        // POST: api/PhieuCanDinhHinhs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanDinhHinh>> PostPhieuCanDinhHinh(PhieuCanDinhHinh phieuCanDinhHinh)
        {
          if (_context.PhieuCanDinhHinh == null)
          {
              return Problem("Entity set 'dbPMScontext.PhieuCanDinhHinh'  is null.");
          }
            _context.PhieuCanDinhHinh.Add(phieuCanDinhHinh);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPhieuCanDinhHinh", new { id = phieuCanDinhHinh.Id }, phieuCanDinhHinh);
        }

        // DELETE: api/PhieuCanDinhHinhs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuCanDinhHinh(int id)
        {
            if (_context.PhieuCanDinhHinh == null)
            {
                return NotFound();
            }
            var phieuCanDinhHinh = await _context.PhieuCanDinhHinh.FindAsync(id);
            if (phieuCanDinhHinh == null)
            {
                return NotFound();
            }

            _context.PhieuCanDinhHinh.Remove(phieuCanDinhHinh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanDinhHinhExists(int id)
        {
            return (_context.PhieuCanDinhHinh?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
