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
    public class DanhMucFormMoTrucTiepsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public DanhMucFormMoTrucTiepsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/DanhMucFormMoTrucTieps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanhMucFormMoTrucTiep>>> GetDanhMucFormMoTrucTiep()
        {
          if (_context.DanhMucFormMoTrucTiep == null)
          {
              return NotFound();
          }
            return await _context.DanhMucFormMoTrucTiep.ToListAsync();
        }

        // GET: api/DanhMucFormMoTrucTieps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DanhMucFormMoTrucTiep>> GetDanhMucFormMoTrucTiep(string id)
        {
          if (_context.DanhMucFormMoTrucTiep == null)
          {
              return NotFound();
          }
            var danhMucFormMoTrucTiep = await _context.DanhMucFormMoTrucTiep.FindAsync(id);

            if (danhMucFormMoTrucTiep == null)
            {
                return NotFound();
            }

            return danhMucFormMoTrucTiep;
        }

        // PUT: api/DanhMucFormMoTrucTieps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDanhMucFormMoTrucTiep(string id, DanhMucFormMoTrucTiep danhMucFormMoTrucTiep)
        {
            if (id != danhMucFormMoTrucTiep.TenForm)
            {
                return BadRequest();
            }

            _context.Entry(danhMucFormMoTrucTiep).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DanhMucFormMoTrucTiepExists(id))
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

        // POST: api/DanhMucFormMoTrucTieps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DanhMucFormMoTrucTiep>> PostDanhMucFormMoTrucTiep(DanhMucFormMoTrucTiep danhMucFormMoTrucTiep)
        {
          if (_context.DanhMucFormMoTrucTiep == null)
          {
              return Problem("Entity set 'dbPMScontext.DanhMucFormMoTrucTiep'  is null.");
          }
            _context.DanhMucFormMoTrucTiep.Add(danhMucFormMoTrucTiep);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DanhMucFormMoTrucTiepExists(danhMucFormMoTrucTiep.TenForm))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDanhMucFormMoTrucTiep", new { id = danhMucFormMoTrucTiep.TenForm }, danhMucFormMoTrucTiep);
        }

        // DELETE: api/DanhMucFormMoTrucTieps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDanhMucFormMoTrucTiep(string id)
        {
            if (_context.DanhMucFormMoTrucTiep == null)
            {
                return NotFound();
            }
            var danhMucFormMoTrucTiep = await _context.DanhMucFormMoTrucTiep.FindAsync(id);
            if (danhMucFormMoTrucTiep == null)
            {
                return NotFound();
            }

            _context.DanhMucFormMoTrucTiep.Remove(danhMucFormMoTrucTiep);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DanhMucFormMoTrucTiepExists(string id)
        {
            return (_context.DanhMucFormMoTrucTiep?.Any(e => e.TenForm == id)).GetValueOrDefault();
        }
    }
}
