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
    public class MaThanhPhamNguyenLieu_tempController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamNguyenLieu_tempController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/MaThanhPhamNguyenLieu_temp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamNguyenLieu_temp>>> GetMaThanhPhamNguyenLieu_temp()
        {
          if (_context.MaThanhPhamNguyenLieu_temp == null)
          {
              return NotFound();
          }
            return await _context.MaThanhPhamNguyenLieu_temp.ToListAsync();
        }

        // GET: api/MaThanhPhamNguyenLieu_temp/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamNguyenLieu_temp>> GetMaThanhPhamNguyenLieu_temp(string id)
        {
          if (_context.MaThanhPhamNguyenLieu_temp == null)
          {
              return NotFound();
          }
            var maThanhPhamNguyenLieu_temp = await _context.MaThanhPhamNguyenLieu_temp.FindAsync(id);

            if (maThanhPhamNguyenLieu_temp == null)
            {
                return NotFound();
            }

            return maThanhPhamNguyenLieu_temp;
        }

        // PUT: api/MaThanhPhamNguyenLieu_temp/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaThanhPhamNguyenLieu_temp(string id, MaThanhPhamNguyenLieu_temp maThanhPhamNguyenLieu_temp)
        {
            if (id != maThanhPhamNguyenLieu_temp.MaCa)
            {
                return BadRequest();
            }

            _context.Entry(maThanhPhamNguyenLieu_temp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaThanhPhamNguyenLieu_tempExists(id))
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

        // POST: api/MaThanhPhamNguyenLieu_temp
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamNguyenLieu_temp>> PostMaThanhPhamNguyenLieu_temp(MaThanhPhamNguyenLieu_temp maThanhPhamNguyenLieu_temp)
        {
          if (_context.MaThanhPhamNguyenLieu_temp == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamNguyenLieu_temp'  is null.");
          }
            _context.MaThanhPhamNguyenLieu_temp.Add(maThanhPhamNguyenLieu_temp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaThanhPhamNguyenLieu_tempExists(maThanhPhamNguyenLieu_temp.MaCa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaThanhPhamNguyenLieu_temp", new { id = maThanhPhamNguyenLieu_temp.MaCa }, maThanhPhamNguyenLieu_temp);
        }

        // DELETE: api/MaThanhPhamNguyenLieu_temp/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaThanhPhamNguyenLieu_temp(string id)
        {
            if (_context.MaThanhPhamNguyenLieu_temp == null)
            {
                return NotFound();
            }
            var maThanhPhamNguyenLieu_temp = await _context.MaThanhPhamNguyenLieu_temp.FindAsync(id);
            if (maThanhPhamNguyenLieu_temp == null)
            {
                return NotFound();
            }

            _context.MaThanhPhamNguyenLieu_temp.Remove(maThanhPhamNguyenLieu_temp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaThanhPhamNguyenLieu_tempExists(string id)
        {
            return (_context.MaThanhPhamNguyenLieu_temp?.Any(e => e.MaCa == id)).GetValueOrDefault();
        }
    }
}
