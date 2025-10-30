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
    public class NhaCungCapNguyenLieu_tempController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhaCungCapNguyenLieu_tempController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/NhaCungCapNguyenLieu_temp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhaCungCapNguyenLieu_temp>>> GetNhaCungCapNguyenLieu_temp()
        {
          if (_context.NhaCungCapNguyenLieu_temp == null)
          {
              return NotFound();
          }
            return await _context.NhaCungCapNguyenLieu_temp.ToListAsync();
        }

        // GET: api/NhaCungCapNguyenLieu_temp/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhaCungCapNguyenLieu_temp>> GetNhaCungCapNguyenLieu_temp(string id)
        {
          if (_context.NhaCungCapNguyenLieu_temp == null)
          {
              return NotFound();
          }
            var nhaCungCapNguyenLieu_temp = await _context.NhaCungCapNguyenLieu_temp.FindAsync(id);

            if (nhaCungCapNguyenLieu_temp == null)
            {
                return NotFound();
            }

            return nhaCungCapNguyenLieu_temp;
        }

        // PUT: api/NhaCungCapNguyenLieu_temp/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhaCungCapNguyenLieu_temp(string id, NhaCungCapNguyenLieu_temp nhaCungCapNguyenLieu_temp)
        {
            if (id != nhaCungCapNguyenLieu_temp.Ma)
            {
                return BadRequest();
            }

            _context.Entry(nhaCungCapNguyenLieu_temp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhaCungCapNguyenLieu_tempExists(id))
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

        // POST: api/NhaCungCapNguyenLieu_temp
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhaCungCapNguyenLieu_temp>> PostNhaCungCapNguyenLieu_temp(NhaCungCapNguyenLieu_temp nhaCungCapNguyenLieu_temp)
        {
          if (_context.NhaCungCapNguyenLieu_temp == null)
          {
              return Problem("Entity set 'dbPMScontext.NhaCungCapNguyenLieu_temp'  is null.");
          }
            _context.NhaCungCapNguyenLieu_temp.Add(nhaCungCapNguyenLieu_temp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NhaCungCapNguyenLieu_tempExists(nhaCungCapNguyenLieu_temp.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNhaCungCapNguyenLieu_temp", new { id = nhaCungCapNguyenLieu_temp.Ma }, nhaCungCapNguyenLieu_temp);
        }

        // DELETE: api/NhaCungCapNguyenLieu_temp/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhaCungCapNguyenLieu_temp(string id)
        {
            if (_context.NhaCungCapNguyenLieu_temp == null)
            {
                return NotFound();
            }
            var nhaCungCapNguyenLieu_temp = await _context.NhaCungCapNguyenLieu_temp.FindAsync(id);
            if (nhaCungCapNguyenLieu_temp == null)
            {
                return NotFound();
            }

            _context.NhaCungCapNguyenLieu_temp.Remove(nhaCungCapNguyenLieu_temp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhaCungCapNguyenLieu_tempExists(string id)
        {
            return (_context.NhaCungCapNguyenLieu_temp?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
