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
    public class MaMauNguyenLieu_tempController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaMauNguyenLieu_tempController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/MaMauNguyenLieu_temp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaMauNguyenLieu_temp>>> GetMaMauNguyenLieu_temp()
        {
          if (_context.MaMauNguyenLieu_temp == null)
          {
              return NotFound();
          }
            return await _context.MaMauNguyenLieu_temp.ToListAsync();
        }

        // GET: api/MaMauNguyenLieu_temp/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaMauNguyenLieu_temp>> GetMaMauNguyenLieu_temp(string id)
        {
          if (_context.MaMauNguyenLieu_temp == null)
          {
              return NotFound();
          }
            var maMauNguyenLieu_temp = await _context.MaMauNguyenLieu_temp.FindAsync(id);

            if (maMauNguyenLieu_temp == null)
            {
                return NotFound();
            }

            return maMauNguyenLieu_temp;
        }

        // PUT: api/MaMauNguyenLieu_temp/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaMauNguyenLieu_temp(string id, MaMauNguyenLieu_temp maMauNguyenLieu_temp)
        {
            if (id != maMauNguyenLieu_temp.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maMauNguyenLieu_temp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaMauNguyenLieu_tempExists(id))
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

        // POST: api/MaMauNguyenLieu_temp
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaMauNguyenLieu_temp>> PostMaMauNguyenLieu_temp(MaMauNguyenLieu_temp maMauNguyenLieu_temp)
        {
          if (_context.MaMauNguyenLieu_temp == null)
          {
              return Problem("Entity set 'dbPMScontext.MaMauNguyenLieu_temp'  is null.");
          }
            _context.MaMauNguyenLieu_temp.Add(maMauNguyenLieu_temp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaMauNguyenLieu_tempExists(maMauNguyenLieu_temp.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaMauNguyenLieu_temp", new { id = maMauNguyenLieu_temp.Ma }, maMauNguyenLieu_temp);
        }

        // DELETE: api/MaMauNguyenLieu_temp/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaMauNguyenLieu_temp(string id)
        {
            if (_context.MaMauNguyenLieu_temp == null)
            {
                return NotFound();
            }
            var maMauNguyenLieu_temp = await _context.MaMauNguyenLieu_temp.FindAsync(id);
            if (maMauNguyenLieu_temp == null)
            {
                return NotFound();
            }

            _context.MaMauNguyenLieu_temp.Remove(maMauNguyenLieu_temp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaMauNguyenLieu_tempExists(string id)
        {
            return (_context.MaMauNguyenLieu_temp?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
