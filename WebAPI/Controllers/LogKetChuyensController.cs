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
    public class LogKetChuyensController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public LogKetChuyensController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/LogKetChuyens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogKetChuyen>>> GetLogKetChuyen()
        {
          if (_context.LogKetChuyen == null)
          {
              return NotFound();
          }
            return await _context.LogKetChuyen.ToListAsync();
        }

        // GET: api/LogKetChuyens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogKetChuyen>> GetLogKetChuyen(int id)
        {
          if (_context.LogKetChuyen == null)
          {
              return NotFound();
          }
            var logKetChuyen = await _context.LogKetChuyen.FindAsync(id);

            if (logKetChuyen == null)
            {
                return NotFound();
            }

            return logKetChuyen;
        }

        // PUT: api/LogKetChuyens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogKetChuyen(int id, LogKetChuyen logKetChuyen)
        {
            if (id != logKetChuyen.STT)
            {
                return BadRequest();
            }

            _context.Entry(logKetChuyen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogKetChuyenExists(id))
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

        // POST: api/LogKetChuyens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LogKetChuyen>> PostLogKetChuyen(LogKetChuyen logKetChuyen)
        {
          if (_context.LogKetChuyen == null)
          {
              return Problem("Entity set 'dbPMScontext.LogKetChuyen'  is null.");
          }
            _context.LogKetChuyen.Add(logKetChuyen);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LogKetChuyenExists(logKetChuyen.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLogKetChuyen", new { id = logKetChuyen.STT }, logKetChuyen);
        }

        // DELETE: api/LogKetChuyens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogKetChuyen(int id)
        {
            if (_context.LogKetChuyen == null)
            {
                return NotFound();
            }
            var logKetChuyen = await _context.LogKetChuyen.FindAsync(id);
            if (logKetChuyen == null)
            {
                return NotFound();
            }

            _context.LogKetChuyen.Remove(logKetChuyen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogKetChuyenExists(int id)
        {
            return (_context.LogKetChuyen?.Any(e => e.STT == id)).GetValueOrDefault();
        }
    }
}
