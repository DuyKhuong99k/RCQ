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
    public class RP_SanLuongNguyenLieuTrenBanNgayController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public RP_SanLuongNguyenLieuTrenBanNgayController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/RP_SanLuongNguyenLieuTrenBanNgay
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RP_SanLuongNguyenLieuTrenBanNgay>>> GetRP_SanLuongNguyenLieuTrenBanNgay()
        {
          if (_context.RP_SanLuongNguyenLieuTrenBanNgay == null)
          {
              return NotFound();
          }
            return await _context.RP_SanLuongNguyenLieuTrenBanNgay.ToListAsync();
        }

        // GET: api/RP_SanLuongNguyenLieuTrenBanNgay/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RP_SanLuongNguyenLieuTrenBanNgay>> GetRP_SanLuongNguyenLieuTrenBanNgay(DateTime id)
        {
          if (_context.RP_SanLuongNguyenLieuTrenBanNgay == null)
          {
              return NotFound();
          }
            var rP_SanLuongNguyenLieuTrenBanNgay = await _context.RP_SanLuongNguyenLieuTrenBanNgay.FindAsync(id);

            if (rP_SanLuongNguyenLieuTrenBanNgay == null)
            {
                return NotFound();
            }

            return rP_SanLuongNguyenLieuTrenBanNgay;
        }

        // PUT: api/RP_SanLuongNguyenLieuTrenBanNgay/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRP_SanLuongNguyenLieuTrenBanNgay(DateTime id, RP_SanLuongNguyenLieuTrenBanNgay rP_SanLuongNguyenLieuTrenBanNgay)
        {
            if (id != rP_SanLuongNguyenLieuTrenBanNgay.Ngay)
            {
                return BadRequest();
            }

            _context.Entry(rP_SanLuongNguyenLieuTrenBanNgay).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RP_SanLuongNguyenLieuTrenBanNgayExists(id))
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

        // POST: api/RP_SanLuongNguyenLieuTrenBanNgay
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RP_SanLuongNguyenLieuTrenBanNgay>> PostRP_SanLuongNguyenLieuTrenBanNgay(RP_SanLuongNguyenLieuTrenBanNgay rP_SanLuongNguyenLieuTrenBanNgay)
        {
          if (_context.RP_SanLuongNguyenLieuTrenBanNgay == null)
          {
              return Problem("Entity set 'dbPMScontext.RP_SanLuongNguyenLieuTrenBanNgay'  is null.");
          }
            _context.RP_SanLuongNguyenLieuTrenBanNgay.Add(rP_SanLuongNguyenLieuTrenBanNgay);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RP_SanLuongNguyenLieuTrenBanNgayExists(rP_SanLuongNguyenLieuTrenBanNgay.Ngay))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRP_SanLuongNguyenLieuTrenBanNgay", new { id = rP_SanLuongNguyenLieuTrenBanNgay.Ngay }, rP_SanLuongNguyenLieuTrenBanNgay);
        }

        // DELETE: api/RP_SanLuongNguyenLieuTrenBanNgay/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRP_SanLuongNguyenLieuTrenBanNgay(DateTime id)
        {
            if (_context.RP_SanLuongNguyenLieuTrenBanNgay == null)
            {
                return NotFound();
            }
            var rP_SanLuongNguyenLieuTrenBanNgay = await _context.RP_SanLuongNguyenLieuTrenBanNgay.FindAsync(id);
            if (rP_SanLuongNguyenLieuTrenBanNgay == null)
            {
                return NotFound();
            }

            _context.RP_SanLuongNguyenLieuTrenBanNgay.Remove(rP_SanLuongNguyenLieuTrenBanNgay);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RP_SanLuongNguyenLieuTrenBanNgayExists(DateTime id)
        {
            return (_context.RP_SanLuongNguyenLieuTrenBanNgay?.Any(e => e.Ngay == id)).GetValueOrDefault();
        }
    }
}
