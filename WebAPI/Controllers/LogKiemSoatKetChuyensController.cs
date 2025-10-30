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
    public class LogKiemSoatKetChuyensController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public LogKiemSoatKetChuyensController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/LogKiemSoatKetChuyens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogKiemSoatKetChuyen>>> GetLogKiemSoatKetChuyen()
        {
          if (_context.LogKiemSoatKetChuyen == null)
          {
              return NotFound();
          }
            return await _context.LogKiemSoatKetChuyen.ToListAsync();
        }

        // GET: api/LogKiemSoatKetChuyens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogKiemSoatKetChuyen>> GetLogKiemSoatKetChuyen(long id)
        {
          if (_context.LogKiemSoatKetChuyen == null)
          {
              return NotFound();
          }
            var logKiemSoatKetChuyen = await _context.LogKiemSoatKetChuyen.FindAsync(id);

            if (logKiemSoatKetChuyen == null)
            {
                return NotFound();
            }

            return logKiemSoatKetChuyen;
        }

        // PUT: api/LogKiemSoatKetChuyens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogKiemSoatKetChuyen(long id, LogKiemSoatKetChuyen logKiemSoatKetChuyen)
        {
            if (id != logKiemSoatKetChuyen.Id)
            {
                return BadRequest();
            }

            _context.Entry(logKiemSoatKetChuyen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogKiemSoatKetChuyenExists(id))
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

        // POST: api/LogKiemSoatKetChuyens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LogKiemSoatKetChuyen>> PostLogKiemSoatKetChuyen(LogKiemSoatKetChuyen logKiemSoatKetChuyen)
        {
          if (_context.LogKiemSoatKetChuyen == null)
          {
              return Problem("Entity set 'dbPMScontext.LogKiemSoatKetChuyen'  is null.");
          }
            _context.LogKiemSoatKetChuyen.Add(logKiemSoatKetChuyen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogKiemSoatKetChuyen", new { id = logKiemSoatKetChuyen.Id }, logKiemSoatKetChuyen);
        }

        // DELETE: api/LogKiemSoatKetChuyens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogKiemSoatKetChuyen(long id)
        {
            if (_context.LogKiemSoatKetChuyen == null)
            {
                return NotFound();
            }
            var logKiemSoatKetChuyen = await _context.LogKiemSoatKetChuyen.FindAsync(id);
            if (logKiemSoatKetChuyen == null)
            {
                return NotFound();
            }

            _context.LogKiemSoatKetChuyen.Remove(logKiemSoatKetChuyen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogKiemSoatKetChuyenExists(long id)
        {
            return (_context.LogKiemSoatKetChuyen?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
