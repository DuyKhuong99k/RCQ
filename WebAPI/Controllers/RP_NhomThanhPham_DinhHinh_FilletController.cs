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
    public class RP_NhomThanhPham_DinhHinh_FilletController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public RP_NhomThanhPham_DinhHinh_FilletController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/RP_NhomThanhPham_DinhHinh_Fillet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RP_NhomThanhPham_DinhHinh_Fillet>>> GetRP_NhomThanhPham_DinhHinh_Fillet()
        {
          if (_context.RP_NhomThanhPham_DinhHinh_Fillet == null)
          {
              return NotFound();
          }
            return await _context.RP_NhomThanhPham_DinhHinh_Fillet.ToListAsync();
        }

        // GET: api/RP_NhomThanhPham_DinhHinh_Fillet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RP_NhomThanhPham_DinhHinh_Fillet>> GetRP_NhomThanhPham_DinhHinh_Fillet(string id)
        {
          if (_context.RP_NhomThanhPham_DinhHinh_Fillet == null)
          {
              return NotFound();
          }
            var rP_NhomThanhPham_DinhHinh_Fillet = await _context.RP_NhomThanhPham_DinhHinh_Fillet.FindAsync(id);

            if (rP_NhomThanhPham_DinhHinh_Fillet == null)
            {
                return NotFound();
            }

            return rP_NhomThanhPham_DinhHinh_Fillet;
        }

        // PUT: api/RP_NhomThanhPham_DinhHinh_Fillet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRP_NhomThanhPham_DinhHinh_Fillet(string id, RP_NhomThanhPham_DinhHinh_Fillet rP_NhomThanhPham_DinhHinh_Fillet)
        {
            if (id != rP_NhomThanhPham_DinhHinh_Fillet.Ma)
            {
                return BadRequest();
            }

            _context.Entry(rP_NhomThanhPham_DinhHinh_Fillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RP_NhomThanhPham_DinhHinh_FilletExists(id))
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

        // POST: api/RP_NhomThanhPham_DinhHinh_Fillet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RP_NhomThanhPham_DinhHinh_Fillet>> PostRP_NhomThanhPham_DinhHinh_Fillet(RP_NhomThanhPham_DinhHinh_Fillet rP_NhomThanhPham_DinhHinh_Fillet)
        {
          if (_context.RP_NhomThanhPham_DinhHinh_Fillet == null)
          {
              return Problem("Entity set 'dbPMScontext.RP_NhomThanhPham_DinhHinh_Fillet'  is null.");
          }
            _context.RP_NhomThanhPham_DinhHinh_Fillet.Add(rP_NhomThanhPham_DinhHinh_Fillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RP_NhomThanhPham_DinhHinh_FilletExists(rP_NhomThanhPham_DinhHinh_Fillet.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRP_NhomThanhPham_DinhHinh_Fillet", new { id = rP_NhomThanhPham_DinhHinh_Fillet.Ma }, rP_NhomThanhPham_DinhHinh_Fillet);
        }

        // DELETE: api/RP_NhomThanhPham_DinhHinh_Fillet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRP_NhomThanhPham_DinhHinh_Fillet(string id)
        {
            if (_context.RP_NhomThanhPham_DinhHinh_Fillet == null)
            {
                return NotFound();
            }
            var rP_NhomThanhPham_DinhHinh_Fillet = await _context.RP_NhomThanhPham_DinhHinh_Fillet.FindAsync(id);
            if (rP_NhomThanhPham_DinhHinh_Fillet == null)
            {
                return NotFound();
            }

            _context.RP_NhomThanhPham_DinhHinh_Fillet.Remove(rP_NhomThanhPham_DinhHinh_Fillet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RP_NhomThanhPham_DinhHinh_FilletExists(string id)
        {
            return (_context.RP_NhomThanhPham_DinhHinh_Fillet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
