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
    public class MaSizeNguyenLieu_tempController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaSizeNguyenLieu_tempController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/MaSizeNguyenLieu_temp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaSizeNguyenLieu_temp>>> GetMaSizeNguyenLieu_temp()
        {
          if (_context.MaSizeNguyenLieu_temp == null)
          {
              return NotFound();
          }
            return await _context.MaSizeNguyenLieu_temp.ToListAsync();
        }

        // GET: api/MaSizeNguyenLieu_temp/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaSizeNguyenLieu_temp>> GetMaSizeNguyenLieu_temp(string id)
        {
          if (_context.MaSizeNguyenLieu_temp == null)
          {
              return NotFound();
          }
            var maSizeNguyenLieu_temp = await _context.MaSizeNguyenLieu_temp.FindAsync(id);

            if (maSizeNguyenLieu_temp == null)
            {
                return NotFound();
            }

            return maSizeNguyenLieu_temp;
        }

        // PUT: api/MaSizeNguyenLieu_temp/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaSizeNguyenLieu_temp(string id, MaSizeNguyenLieu_temp maSizeNguyenLieu_temp)
        {
            if (id != maSizeNguyenLieu_temp.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maSizeNguyenLieu_temp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaSizeNguyenLieu_tempExists(id))
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

        // POST: api/MaSizeNguyenLieu_temp
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaSizeNguyenLieu_temp>> PostMaSizeNguyenLieu_temp(MaSizeNguyenLieu_temp maSizeNguyenLieu_temp)
        {
          if (_context.MaSizeNguyenLieu_temp == null)
          {
              return Problem("Entity set 'dbPMScontext.MaSizeNguyenLieu_temp'  is null.");
          }
            _context.MaSizeNguyenLieu_temp.Add(maSizeNguyenLieu_temp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaSizeNguyenLieu_tempExists(maSizeNguyenLieu_temp.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaSizeNguyenLieu_temp", new { id = maSizeNguyenLieu_temp.Ma }, maSizeNguyenLieu_temp);
        }

        // DELETE: api/MaSizeNguyenLieu_temp/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaSizeNguyenLieu_temp(string id)
        {
            if (_context.MaSizeNguyenLieu_temp == null)
            {
                return NotFound();
            }
            var maSizeNguyenLieu_temp = await _context.MaSizeNguyenLieu_temp.FindAsync(id);
            if (maSizeNguyenLieu_temp == null)
            {
                return NotFound();
            }

            _context.MaSizeNguyenLieu_temp.Remove(maSizeNguyenLieu_temp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaSizeNguyenLieu_tempExists(string id)
        {
            return (_context.MaSizeNguyenLieu_temp?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
