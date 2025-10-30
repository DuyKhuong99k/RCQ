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
    public class NhanViensController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhanViensController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/NhanViens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVien>>> Gets()
        {
          if (_context.NhanVien == null)
          {
              return NotFound();
          }
            return await _context.NhanVien.ToListAsync();
        }

        // GET: api/NhanViens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVien>> Get(string id)
        {
          if (_context.NhanVien == null)
          {
              return NotFound();
          }
            var nhanVien = await _context.NhanVien.FindAsync(id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            return nhanVien;
        }

        // PUT: api/NhanViens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NhanVien nhanVien)
        {
            if (id != nhanVien.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(nhanVien).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhanVienExists(id))
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

        // POST: api/NhanViens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhanVien>> Post(NhanVien nhanVien)
        {
          if (_context.NhanVien == null)
          {
              return Problem("Entity set 'dbPMScontext.NhanVien'  is null.");
          }
            _context.NhanVien.Add(nhanVien);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NhanVienExists(nhanVien.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNhanVien", new { id = nhanVien.MaNhanVien }, nhanVien);
        }

        // DELETE: api/NhanViens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NhanVien == null)
            {
                return NotFound();
            }
            var nhanVien = await _context.NhanVien.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            _context.NhanVien.Remove(nhanVien);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhanVienExists(string id)
        {
            return (_context.NhanVien?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }
    }
}
