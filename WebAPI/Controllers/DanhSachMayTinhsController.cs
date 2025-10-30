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
    public class DanhSachMayTinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public DanhSachMayTinhsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/DanhSachMayTinhs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanhSachMayTinh>>> GetDanhSachMayTinh()
        {
          if (_context.DanhSachMayTinh == null)
          {
              return NotFound();
          }
            return await _context.DanhSachMayTinh.ToListAsync();
        }

        // GET: api/DanhSachMayTinhs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DanhSachMayTinh>> GetDanhSachMayTinh(string id)
        {
          if (_context.DanhSachMayTinh == null)
          {
              return NotFound();
          }
            var danhSachMayTinh = await _context.DanhSachMayTinh.FindAsync(id);

            if (danhSachMayTinh == null)
            {
                return NotFound();
            }

            return danhSachMayTinh;
        }

        // PUT: api/DanhSachMayTinhs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDanhSachMayTinh(string id, DanhSachMayTinh danhSachMayTinh)
        {
            if (id != danhSachMayTinh.IDMayTinh)
            {
                return BadRequest();
            }

            _context.Entry(danhSachMayTinh).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DanhSachMayTinhExists(id))
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

        // POST: api/DanhSachMayTinhs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DanhSachMayTinh>> PostDanhSachMayTinh(DanhSachMayTinh danhSachMayTinh)
        {
          if (_context.DanhSachMayTinh == null)
          {
              return Problem("Entity set 'dbPMScontext.DanhSachMayTinh'  is null.");
          }
            _context.DanhSachMayTinh.Add(danhSachMayTinh);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DanhSachMayTinhExists(danhSachMayTinh.IDMayTinh))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDanhSachMayTinh", new { id = danhSachMayTinh.IDMayTinh }, danhSachMayTinh);
        }

        // DELETE: api/DanhSachMayTinhs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDanhSachMayTinh(string id)
        {
            if (_context.DanhSachMayTinh == null)
            {
                return NotFound();
            }
            var danhSachMayTinh = await _context.DanhSachMayTinh.FindAsync(id);
            if (danhSachMayTinh == null)
            {
                return NotFound();
            }

            _context.DanhSachMayTinh.Remove(danhSachMayTinh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DanhSachMayTinhExists(string id)
        {
            return (_context.DanhSachMayTinh?.Any(e => e.IDMayTinh == id)).GetValueOrDefault();
        }
    }
}
