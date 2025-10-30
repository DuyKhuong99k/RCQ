using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;

namespace WebAPI.Controllers
{
     [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaChuAoVungNuoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaChuAoVungNuoisController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaChuAoVungNuois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaChuAoVungNuoi>>> Gets()
        {
          if (_context.MaChuAoVungNuoi == null)
          {
              return NotFound();
          }
            return await _context.MaChuAoVungNuoi.ToListAsync();
        }

        // GET: api/MaChuAoVungNuois/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaChuAoVungNuoi>> GetMaChuAoVungNuoi(string id)
        {
          if (_context.MaChuAoVungNuoi == null)
          {
              return NotFound();
          }
            var maChuAoVungNuoi = await _context.MaChuAoVungNuoi.FindAsync(id);

            if (maChuAoVungNuoi == null)
            {
                return NotFound();
            }

            return maChuAoVungNuoi;
        }

        // PUT: api/MaChuAoVungNuois/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaChuAoVungNuoi(string id, MaChuAoVungNuoi maChuAoVungNuoi)
        {
            if (id != maChuAoVungNuoi.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maChuAoVungNuoi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaChuAoVungNuoiExists(id))
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

        // POST: api/MaChuAoVungNuois
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaChuAoVungNuoi>> PostMaChuAoVungNuoi(MaChuAoVungNuoi maChuAoVungNuoi)
        {
          if (_context.MaChuAoVungNuoi == null)
          {
              return Problem("Entity set 'dbPMScontext.MaChuAoVungNuoi'  is null.");
          }
            _context.MaChuAoVungNuoi.Add(maChuAoVungNuoi);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaChuAoVungNuoiExists(maChuAoVungNuoi.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaChuAoVungNuoi", new { id = maChuAoVungNuoi.Ma }, maChuAoVungNuoi);
        }

        // DELETE: api/MaChuAoVungNuois/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaChuAoVungNuoi(string id)
        {
            if (_context.MaChuAoVungNuoi == null)
            {
                return NotFound();
            }
            var maChuAoVungNuoi = await _context.MaChuAoVungNuoi.FindAsync(id);
            if (maChuAoVungNuoi == null)
            {
                return NotFound();
            }

            _context.MaChuAoVungNuoi.Remove(maChuAoVungNuoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaChuAoVungNuoiExists(string id)
        {
            return (_context.MaChuAoVungNuoi?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
