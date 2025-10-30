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
    public class TrongLuongGioiHanDinhHinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;
        private MainViewModel Vm => MainViewModel.Instance;
        public TrongLuongGioiHanDinhHinhsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/TrongLuongGioiHanDinhHinhs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrongLuongGioiHanDinhHinh>>> GetTrongLuongGioiHanDinhHinh()
        {
          if (_context.TrongLuongGioiHanDinhHinh == null)
          {
              return NotFound();
          }
            return await _context.TrongLuongGioiHanDinhHinh.ToListAsync();
        }

        // GET: api/TrongLuongGioiHanDinhHinhs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrongLuongGioiHanDinhHinh>> GetTrongLuongGioiHanDinhHinh(int id)
        {
          if (_context.TrongLuongGioiHanDinhHinh == null)
          {
              return NotFound();
          }
            var trongLuongGioiHanDinhHinh = await _context.TrongLuongGioiHanDinhHinh.FindAsync(id);

            if (trongLuongGioiHanDinhHinh == null)
            {
                return NotFound();
            }

            return trongLuongGioiHanDinhHinh;
        }

        // PUT: api/TrongLuongGioiHanDinhHinhs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrongLuongGioiHanDinhHinh(int id, TrongLuongGioiHanDinhHinh trongLuongGioiHanDinhHinh)
        {
            if (id != trongLuongGioiHanDinhHinh.STT)
            {
                return BadRequest();
            }

            _context.Entry(trongLuongGioiHanDinhHinh).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrongLuongGioiHanDinhHinhExists(id))
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

        // POST: api/TrongLuongGioiHanDinhHinhs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TrongLuongGioiHanDinhHinh>> PostTrongLuongGioiHanDinhHinh(TrongLuongGioiHanDinhHinh trongLuongGioiHanDinhHinh)
        {
          if (_context.TrongLuongGioiHanDinhHinh == null)
          {
              return Problem("Entity set 'dbPMScontext.TrongLuongGioiHanDinhHinh'  is null.");
          }
            _context.TrongLuongGioiHanDinhHinh.Add(trongLuongGioiHanDinhHinh);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TrongLuongGioiHanDinhHinhExists(trongLuongGioiHanDinhHinh.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTrongLuongGioiHanDinhHinh", new { id = trongLuongGioiHanDinhHinh.STT }, trongLuongGioiHanDinhHinh);
        }

        // DELETE: api/TrongLuongGioiHanDinhHinhs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrongLuongGioiHanDinhHinh(int id)
        {
            if (_context.TrongLuongGioiHanDinhHinh == null)
            {
                return NotFound();
            }
            var trongLuongGioiHanDinhHinh = await _context.TrongLuongGioiHanDinhHinh.FindAsync(id);
            if (trongLuongGioiHanDinhHinh == null)
            {
                return NotFound();
            }

            _context.TrongLuongGioiHanDinhHinh.Remove(trongLuongGioiHanDinhHinh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrongLuongGioiHanDinhHinhExists(int id)
        {
            return (_context.TrongLuongGioiHanDinhHinh?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
