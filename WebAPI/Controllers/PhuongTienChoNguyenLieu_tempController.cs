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
    public class PhuongTienChoNguyenLieu_tempController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhuongTienChoNguyenLieu_tempController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/PhuongTienChoNguyenLieu_temp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhuongTienChoNguyenLieu_temp>>> GetPhuongTienChoNguyenLieu_temp()
        {
          if (_context.PhuongTienChoNguyenLieu_temp == null)
          {
              return NotFound();
          }
            return await _context.PhuongTienChoNguyenLieu_temp.ToListAsync();
        }

        // GET: api/PhuongTienChoNguyenLieu_temp/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhuongTienChoNguyenLieu_temp>> GetPhuongTienChoNguyenLieu_temp(string id)
        {
          if (_context.PhuongTienChoNguyenLieu_temp == null)
          {
              return NotFound();
          }
            var phuongTienChoNguyenLieu_temp = await _context.PhuongTienChoNguyenLieu_temp.FindAsync(id);

            if (phuongTienChoNguyenLieu_temp == null)
            {
                return NotFound();
            }

            return phuongTienChoNguyenLieu_temp;
        }

        // PUT: api/PhuongTienChoNguyenLieu_temp/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhuongTienChoNguyenLieu_temp(string id, PhuongTienChoNguyenLieu_temp phuongTienChoNguyenLieu_temp)
        {
            if (id != phuongTienChoNguyenLieu_temp.Ma)
            {
                return BadRequest();
            }

            _context.Entry(phuongTienChoNguyenLieu_temp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhuongTienChoNguyenLieu_tempExists(id))
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

        // POST: api/PhuongTienChoNguyenLieu_temp
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhuongTienChoNguyenLieu_temp>> PostPhuongTienChoNguyenLieu_temp(PhuongTienChoNguyenLieu_temp phuongTienChoNguyenLieu_temp)
        {
          if (_context.PhuongTienChoNguyenLieu_temp == null)
          {
              return Problem("Entity set 'dbPMScontext.PhuongTienChoNguyenLieu_temp'  is null.");
          }
            _context.PhuongTienChoNguyenLieu_temp.Add(phuongTienChoNguyenLieu_temp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhuongTienChoNguyenLieu_tempExists(phuongTienChoNguyenLieu_temp.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhuongTienChoNguyenLieu_temp", new { id = phuongTienChoNguyenLieu_temp.Ma }, phuongTienChoNguyenLieu_temp);
        }

        // DELETE: api/PhuongTienChoNguyenLieu_temp/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhuongTienChoNguyenLieu_temp(string id)
        {
            if (_context.PhuongTienChoNguyenLieu_temp == null)
            {
                return NotFound();
            }
            var phuongTienChoNguyenLieu_temp = await _context.PhuongTienChoNguyenLieu_temp.FindAsync(id);
            if (phuongTienChoNguyenLieu_temp == null)
            {
                return NotFound();
            }

            _context.PhuongTienChoNguyenLieu_temp.Remove(phuongTienChoNguyenLieu_temp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhuongTienChoNguyenLieu_tempExists(string id)
        {
            return (_context.PhuongTienChoNguyenLieu_temp?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
