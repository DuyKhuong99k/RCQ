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
    public class MaSanPhamDinhHinhBravoesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaSanPhamDinhHinhBravoesController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/MaSanPhamDinhHinhBravoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaSanPhamDinhHinhBravo>>> GetMaSanPhamDinhHinhBravo()
        {
          if (_context.MaSanPhamDinhHinhBravo == null)
          {
              return NotFound();
          }
            return await _context.MaSanPhamDinhHinhBravo.ToListAsync();
        }

        // GET: api/MaSanPhamDinhHinhBravoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaSanPhamDinhHinhBravo>> GetMaSanPhamDinhHinhBravo(string id)
        {
          if (_context.MaSanPhamDinhHinhBravo == null)
          {
              return NotFound();
          }
            var maSanPhamDinhHinhBravo = await _context.MaSanPhamDinhHinhBravo.FindAsync(id);

            if (maSanPhamDinhHinhBravo == null)
            {
                return NotFound();
            }

            return maSanPhamDinhHinhBravo;
        }

        // PUT: api/MaSanPhamDinhHinhBravoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaSanPhamDinhHinhBravo(string id, MaSanPhamDinhHinhBravo maSanPhamDinhHinhBravo)
        {
            if (id != maSanPhamDinhHinhBravo.DaiThanhId)
            {
                return BadRequest();
            }

            _context.Entry(maSanPhamDinhHinhBravo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaSanPhamDinhHinhBravoExists(id))
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

        // POST: api/MaSanPhamDinhHinhBravoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaSanPhamDinhHinhBravo>> PostMaSanPhamDinhHinhBravo(MaSanPhamDinhHinhBravo maSanPhamDinhHinhBravo)
        {
          if (_context.MaSanPhamDinhHinhBravo == null)
          {
              return Problem("Entity set 'dbPMScontext.MaSanPhamDinhHinhBravo'  is null.");
          }
            _context.MaSanPhamDinhHinhBravo.Add(maSanPhamDinhHinhBravo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaSanPhamDinhHinhBravoExists(maSanPhamDinhHinhBravo.DaiThanhId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaSanPhamDinhHinhBravo", new { id = maSanPhamDinhHinhBravo.DaiThanhId }, maSanPhamDinhHinhBravo);
        }

        // DELETE: api/MaSanPhamDinhHinhBravoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaSanPhamDinhHinhBravo(string id)
        {
            if (_context.MaSanPhamDinhHinhBravo == null)
            {
                return NotFound();
            }
            var maSanPhamDinhHinhBravo = await _context.MaSanPhamDinhHinhBravo.FindAsync(id);
            if (maSanPhamDinhHinhBravo == null)
            {
                return NotFound();
            }

            _context.MaSanPhamDinhHinhBravo.Remove(maSanPhamDinhHinhBravo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaSanPhamDinhHinhBravoExists(string id)
        {
            return (_context.MaSanPhamDinhHinhBravo?.Any(e => e.DaiThanhId == id)).GetValueOrDefault();
        }
    }
}
