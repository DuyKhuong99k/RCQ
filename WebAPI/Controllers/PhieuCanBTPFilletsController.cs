using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanBTPFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanBTPFilletsController(dbPMScontext context)
        {
            _context = context;
        }
         private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PhieuCanBTPFillets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanBTPFillet>>> GetPhieuCanBTPFillet()
        {
            if (_context.PhieuCanBTPFillet == null)
            {
                return NotFound();
            }
            return await _context.PhieuCanBTPFillet.ToListAsync();
        }

        // GET: api/PhieuCanBTPFillets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanBTPFillet>> GetPhieuCanBTPFillet(string id)
        {
            if (_context.PhieuCanBTPFillet == null)
            {
                return NotFound();
            }
            var phieuCanBTPFillet = await _context.PhieuCanBTPFillet.FindAsync(id);

            if (phieuCanBTPFillet == null)
            {
                return NotFound();
            }

            return phieuCanBTPFillet;
        }

        // PUT: api/PhieuCanBTPFillets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhieuCanBTPFillet(string id, PhieuCanBTPFillet phieuCanBTPFillet)
        {
            if (id != phieuCanBTPFillet.MaMayTinhCan)
            {
                return BadRequest();
            }

            _context.Entry(phieuCanBTPFillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhieuCanBTPFilletExists(id))
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

        // POST: api/PhieuCanBTPFillets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhieuCanBTPFillet>> PostPhieuCanBTPFillet(PhieuCanBTPFillet phieuCanBTPFillet)
        {
            if (_context.PhieuCanBTPFillet == null)
            {
                return Problem("Entity set 'dbPMScontext.PhieuCanBTPFillet'  is null.");
            }
            _context.PhieuCanBTPFillet.Add(phieuCanBTPFillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhieuCanBTPFilletExists(phieuCanBTPFillet.MaMayTinhCan))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhieuCanBTPFillet", new { id = phieuCanBTPFillet.MaMayTinhCan }, phieuCanBTPFillet);
        }

        // DELETE: api/PhieuCanBTPFillets/5
        [HttpDelete("{id}")]
        
        public async Task<IActionResult> DeletePhieuCanBTPFillet(string id)
        {
            if (_context.PhieuCanBTPFillet == null)
            {
                return NotFound();
            }
            var phieuCanBTPFillet = await _context.PhieuCanBTPFillet.FindAsync(id);
            if (phieuCanBTPFillet == null)
            {
                return NotFound();
            }

            _context.PhieuCanBTPFillet.Remove(phieuCanBTPFillet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhieuCanBTPFilletExists(string id)
        {
            return (_context.PhieuCanBTPFillet?.Any(e => e.MaMayTinhCan == id)).GetValueOrDefault();
        }
        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanBTPFillet_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanBTPFilletv2 == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanBTPFilletv2.GetPhieuCanBTPFillet_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(DateTime dateTime, string xuongId)
        {
            var items = _context.PhieuCanBTPFilletv2.Where(x => x.Ngay == dateTime && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        #endregion
    }
}
