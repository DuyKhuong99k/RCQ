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
    public class NhanVienPhuTheoLinesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhanVienPhuTheoLinesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NhanVienPhuTheoLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienPhuTheoLine>>> Gets()
        {
          if (_context.NhanVienPhuTheoLine == null)
          {
              return NotFound();
          }
            return await _context.NhanVienPhuTheoLine.ToListAsync();
        }

        // GET: api/NhanVienPhuTheoLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienPhuTheoLine>> Get(string id)
        {
          if (_context.NhanVienPhuTheoLine == null)
          {
              return NotFound();
          }
            var nhanVienPhuTheoLine = await _context.NhanVienPhuTheoLine.FindAsync(id);

            if (nhanVienPhuTheoLine == null)
            {
                return NotFound();
            }

            return nhanVienPhuTheoLine;
        }

        // PUT: api/NhanVienPhuTheoLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NhanVienPhuTheoLine nhanVienPhuTheoLine)
        {
            if (id != nhanVienPhuTheoLine.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(nhanVienPhuTheoLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhanVienPhuTheoLineExists(id))
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

        // POST: api/NhanVienPhuTheoLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhanVienPhuTheoLine>> Post(NhanVienPhuTheoLine nhanVienPhuTheoLine)
        {
          if (_context.NhanVienPhuTheoLine == null)
          {
              return Problem("Entity set 'dbPMScontext.NhanVienPhuTheoLine'  is null.");
          }
            _context.NhanVienPhuTheoLine.Add(nhanVienPhuTheoLine);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NhanVienPhuTheoLineExists(nhanVienPhuTheoLine.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNhanVienPhuTheoLine", new { id = nhanVienPhuTheoLine.MaNhanVien }, nhanVienPhuTheoLine);
        }

        // DELETE: api/NhanVienPhuTheoLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NhanVienPhuTheoLine == null)
            {
                return NotFound();
            }
            var nhanVienPhuTheoLine = await _context.NhanVienPhuTheoLine.FindAsync(id);
            if (nhanVienPhuTheoLine == null)
            {
                return NotFound();
            }

            _context.NhanVienPhuTheoLine.Remove(nhanVienPhuTheoLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhanVienPhuTheoLineExists(string id)
        {
            return (_context.NhanVienPhuTheoLine?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }
    }
}
