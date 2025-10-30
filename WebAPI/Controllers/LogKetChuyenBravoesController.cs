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
    public class LogKetChuyenBravoesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public LogKetChuyenBravoesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/LogKetChuyenBravoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogKetChuyenBravo>>> GetLogKetChuyenBravo()
        {
          if (_context.LogKetChuyenBravo == null)
          {
              return NotFound();
          }
            return await _context.LogKetChuyenBravo.ToListAsync();
        }

        // GET: api/LogKetChuyenBravoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogKetChuyenBravo>> GetLogKetChuyenBravo(string id)
        {
          if (_context.LogKetChuyenBravo == null)
          {
              return NotFound();
          }
            var logKetChuyenBravo = await _context.LogKetChuyenBravo.FindAsync(id);

            if (logKetChuyenBravo == null)
            {
                return NotFound();
            }

            return logKetChuyenBravo;
        }

        // PUT: api/LogKetChuyenBravoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogKetChuyenBravo(string id, LogKetChuyenBravo logKetChuyenBravo)
        {
            if (id != logKetChuyenBravo.MaSanPham)
            {
                return BadRequest();
            }

            _context.Entry(logKetChuyenBravo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogKetChuyenBravoExists(id))
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

        // POST: api/LogKetChuyenBravoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LogKetChuyenBravo>> PostLogKetChuyenBravo(LogKetChuyenBravo logKetChuyenBravo)
        {
          if (_context.LogKetChuyenBravo == null)
          {
              return Problem("Entity set 'dbPMScontext.LogKetChuyenBravo'  is null.");
          }
            _context.LogKetChuyenBravo.Add(logKetChuyenBravo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LogKetChuyenBravoExists(logKetChuyenBravo.MaSanPham))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLogKetChuyenBravo", new { id = logKetChuyenBravo.MaSanPham }, logKetChuyenBravo);
        }

        // DELETE: api/LogKetChuyenBravoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogKetChuyenBravo(string id)
        {
            if (_context.LogKetChuyenBravo == null)
            {
                return NotFound();
            }
            var logKetChuyenBravo = await _context.LogKetChuyenBravo.FindAsync(id);
            if (logKetChuyenBravo == null)
            {
                return NotFound();
            }

            _context.LogKetChuyenBravo.Remove(logKetChuyenBravo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogKetChuyenBravoExists(string id)
        {
            return (_context.LogKetChuyenBravo?.Any(e => e.MaSanPham == id)).GetValueOrDefault();
        }
    }
}
