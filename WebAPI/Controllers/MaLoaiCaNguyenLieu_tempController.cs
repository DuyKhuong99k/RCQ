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
    public class MaLoaiCaNguyenLieu_tempController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoaiCaNguyenLieu_tempController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/MaLoaiCaNguyenLieu_temp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaNguyenLieu_temp>>> GetMaLoaiCaNguyenLieu_temp()
        {
          if (_context.MaLoaiCaNguyenLieu_temp == null)
          {
              return NotFound();
          }
            return await _context.MaLoaiCaNguyenLieu_temp.ToListAsync();
        }

        // GET: api/MaLoaiCaNguyenLieu_temp/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaNguyenLieu_temp>> GetMaLoaiCaNguyenLieu_temp(string id)
        {
          if (_context.MaLoaiCaNguyenLieu_temp == null)
          {
              return NotFound();
          }
            var maLoaiCaNguyenLieu_temp = await _context.MaLoaiCaNguyenLieu_temp.FindAsync(id);

            if (maLoaiCaNguyenLieu_temp == null)
            {
                return NotFound();
            }

            return maLoaiCaNguyenLieu_temp;
        }

        // PUT: api/MaLoaiCaNguyenLieu_temp/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaLoaiCaNguyenLieu_temp(string id, MaLoaiCaNguyenLieu_temp maLoaiCaNguyenLieu_temp)
        {
            if (id != maLoaiCaNguyenLieu_temp.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maLoaiCaNguyenLieu_temp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaLoaiCaNguyenLieu_tempExists(id))
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

        // POST: api/MaLoaiCaNguyenLieu_temp
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaNguyenLieu_temp>> PostMaLoaiCaNguyenLieu_temp(MaLoaiCaNguyenLieu_temp maLoaiCaNguyenLieu_temp)
        {
          if (_context.MaLoaiCaNguyenLieu_temp == null)
          {
              return Problem("Entity set 'dbPMScontext.MaLoaiCaNguyenLieu_temp'  is null.");
          }
            _context.MaLoaiCaNguyenLieu_temp.Add(maLoaiCaNguyenLieu_temp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaLoaiCaNguyenLieu_tempExists(maLoaiCaNguyenLieu_temp.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaLoaiCaNguyenLieu_temp", new { id = maLoaiCaNguyenLieu_temp.Ma }, maLoaiCaNguyenLieu_temp);
        }

        // DELETE: api/MaLoaiCaNguyenLieu_temp/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaLoaiCaNguyenLieu_temp(string id)
        {
            if (_context.MaLoaiCaNguyenLieu_temp == null)
            {
                return NotFound();
            }
            var maLoaiCaNguyenLieu_temp = await _context.MaLoaiCaNguyenLieu_temp.FindAsync(id);
            if (maLoaiCaNguyenLieu_temp == null)
            {
                return NotFound();
            }

            _context.MaLoaiCaNguyenLieu_temp.Remove(maLoaiCaNguyenLieu_temp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaLoaiCaNguyenLieu_tempExists(string id)
        {
            return (_context.MaLoaiCaNguyenLieu_temp?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
