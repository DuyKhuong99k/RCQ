using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class MaLoaiCaVungNuoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoaiCaVungNuoisController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLoaiCaVungNuois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaVungNuoi>>> Gets()
        {
            if (_context.MaLoaiCaVungNuoi == null)
            {
                return NotFound();
            }
            return await _context.MaLoaiCaVungNuoi.ToListAsync();
        }

        // GET: api/MaLoaiCaVungNuois/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaVungNuoi>> Get(string id)
        {
            if (_context.MaLoaiCaVungNuoi == null)
            {
                return NotFound();
            }
            var maLoaiCaVungNuoi = await _context.MaLoaiCaVungNuoi.FindAsync(id);

            if (maLoaiCaVungNuoi == null)
            {
                return NotFound();
            }

            return maLoaiCaVungNuoi;
        }

        // PUT: api/MaLoaiCaVungNuois/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaLoaiCaVungNuoi maLoaiCaVungNuoi)
        {
            if (id != maLoaiCaVungNuoi.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maLoaiCaVungNuoi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaLoaiCaVungNuoiExists(id))
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

        // POST: api/MaLoaiCaVungNuois
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaVungNuoi>> Post(MaLoaiCaVungNuoi maLoaiCaVungNuoi)
        {
            if (_context.MaLoaiCaVungNuoi == null)
            {
                return Problem("Entity set 'dbPMScontext.MaLoaiCaVungNuoi'  is null.");
            }
            _context.MaLoaiCaVungNuoi.Add(maLoaiCaVungNuoi);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaLoaiCaVungNuoiExists(maLoaiCaVungNuoi.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maLoaiCaVungNuoi.Ma }, maLoaiCaVungNuoi);
        }

        // DELETE: api/MaLoaiCaVungNuois/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoaiCaVungNuoi == null)
            {
                return NotFound();
            }
            var maLoaiCaVungNuoi = await _context.MaLoaiCaVungNuoi.FindAsync(id);
            if (maLoaiCaVungNuoi == null)
            {
                return NotFound();
            }

            _context.MaLoaiCaVungNuoi.Remove(maLoaiCaVungNuoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaLoaiCaVungNuoiExists(string id)
        {
            return (_context.MaLoaiCaVungNuoi?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaLoaiCaVungNuoi.OrderByDescending(x => x.Ma).ToList();
            var connectionstring = _context.Database.GetConnectionString();
            return Ok(items);
        }

    }
}
