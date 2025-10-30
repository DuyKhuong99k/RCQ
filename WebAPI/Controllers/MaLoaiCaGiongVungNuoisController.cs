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
    public class MaLoaiCaGiongVungNuoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoaiCaGiongVungNuoisController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLoaiCaGiongVungNuois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaGiongVungNuoi>>> Gets()
        {
          if (_context.MaLoaiCaGiongVungNuoi == null)
          {
              return NotFound();
          }
            return await _context.MaLoaiCaGiongVungNuoi.ToListAsync();
        }

        // GET: api/MaLoaiCaGiongVungNuois/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaGiongVungNuoi>> Get(string id)
        {
          if (_context.MaLoaiCaGiongVungNuoi == null)
          {
              return NotFound();
          }
            var maLoaiCaGiongVungNuoi = await _context.MaLoaiCaGiongVungNuoi.FindAsync(id);

            if (maLoaiCaGiongVungNuoi == null)
            {
                return NotFound();
            }

            return maLoaiCaGiongVungNuoi;
        }

        // PUT: api/MaLoaiCaGiongVungNuois/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaLoaiCaGiongVungNuoi maLoaiCaGiongVungNuoi)
        {
            if (id != maLoaiCaGiongVungNuoi.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maLoaiCaGiongVungNuoi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaLoaiCaGiongVungNuoiExists(id))
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

        // POST: api/MaLoaiCaGiongVungNuois
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaGiongVungNuoi>> Post(MaLoaiCaGiongVungNuoi maLoaiCaGiongVungNuoi)
        {
          if (_context.MaLoaiCaGiongVungNuoi == null)
          {
              return Problem("Entity set 'dbPMScontext.MaLoaiCaGiongVungNuoi'  is null.");
          }
            _context.MaLoaiCaGiongVungNuoi.Add(maLoaiCaGiongVungNuoi);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaLoaiCaGiongVungNuoiExists(maLoaiCaGiongVungNuoi.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maLoaiCaGiongVungNuoi.Ma }, maLoaiCaGiongVungNuoi);
        }

        // DELETE: api/MaLoaiCaGiongVungNuois/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoaiCaGiongVungNuoi == null)
            {
                return NotFound();
            }
            var maLoaiCaGiongVungNuoi = await _context.MaLoaiCaGiongVungNuoi.FindAsync(id);
            if (maLoaiCaGiongVungNuoi == null)
            {
                return NotFound();
            }

            _context.MaLoaiCaGiongVungNuoi.Remove(maLoaiCaGiongVungNuoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaLoaiCaGiongVungNuoiExists(string id)
        {
            return (_context.MaLoaiCaGiongVungNuoi?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
