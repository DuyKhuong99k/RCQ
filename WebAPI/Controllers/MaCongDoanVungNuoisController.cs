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
    public class MaCongDoanVungNuoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaCongDoanVungNuoisController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaCongDoanVungNuois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaCongDoanVungNuoi>>> Gets()
        {
          if (_context.MaCongDoanVungNuoi == null)
          {
              return NotFound();
          }
            return await _context.MaCongDoanVungNuoi.ToListAsync();
        }

        // GET: api/MaCongDoanVungNuois/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaCongDoanVungNuoi>> Get(string id)
        {
          if (_context.MaCongDoanVungNuoi == null)
          {
              return NotFound();
          }
            var maCongDoanVungNuoi = await _context.MaCongDoanVungNuoi.FindAsync(id);

            if (maCongDoanVungNuoi == null)
            {
                return NotFound();
            }

            return maCongDoanVungNuoi;
        }

        // PUT: api/MaCongDoanVungNuois/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaCongDoanVungNuoi maCongDoanVungNuoi)
        {
            if (id != maCongDoanVungNuoi.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maCongDoanVungNuoi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaCongDoanVungNuoiExists(id))
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

        // POST: api/MaCongDoanVungNuois
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaCongDoanVungNuoi>> Post(MaCongDoanVungNuoi maCongDoanVungNuoi)
        {
          if (_context.MaCongDoanVungNuoi == null)
          {
              return Problem("Entity set 'dbPMScontext.MaCongDoanVungNuoi'  is null.");
          }
            _context.MaCongDoanVungNuoi.Add(maCongDoanVungNuoi);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaCongDoanVungNuoiExists(maCongDoanVungNuoi.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaCongDoanVungNuoi", new { id = maCongDoanVungNuoi.Ma }, maCongDoanVungNuoi);
        }

        // DELETE: api/MaCongDoanVungNuois/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaCongDoanVungNuoi == null)
            {
                return NotFound();
            }
            var maCongDoanVungNuoi = await _context.MaCongDoanVungNuoi.FindAsync(id);
            if (maCongDoanVungNuoi == null)
            {
                return NotFound();
            }

            _context.MaCongDoanVungNuoi.Remove(maCongDoanVungNuoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaCongDoanVungNuoiExists(string id)
        {
            return (_context.MaCongDoanVungNuoi?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
