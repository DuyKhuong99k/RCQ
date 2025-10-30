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
    public class RP_MaThanhPhamDinhHinhController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public RP_MaThanhPhamDinhHinhController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/RP_MaThanhPhamDinhHinh
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RP_MaThanhPhamDinhHinh>>> Gets()
        {
          if (_context.RP_MaThanhPhamDinhHinh == null)
          {
              return NotFound();
          }
            return await _context.RP_MaThanhPhamDinhHinh.ToListAsync();
        }

        // GET: api/RP_MaThanhPhamDinhHinh/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RP_MaThanhPhamDinhHinh>> Get(string id)
        {
          if (_context.RP_MaThanhPhamDinhHinh == null)
          {
              return NotFound();
          }
            var rP_MaThanhPhamDinhHinh = await _context.RP_MaThanhPhamDinhHinh.FindAsync(id);

            if (rP_MaThanhPhamDinhHinh == null)
            {
                return NotFound();
            }

            return rP_MaThanhPhamDinhHinh;
        }

        // PUT: api/RP_MaThanhPhamDinhHinh/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, RP_MaThanhPhamDinhHinh rP_MaThanhPhamDinhHinh)
        {
            if (id != rP_MaThanhPhamDinhHinh.Ma)
            {
                return BadRequest();
            }

            _context.Entry(rP_MaThanhPhamDinhHinh).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RP_MaThanhPhamDinhHinhExists(id))
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

        // POST: api/RP_MaThanhPhamDinhHinh
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RP_MaThanhPhamDinhHinh>> Post(RP_MaThanhPhamDinhHinh rP_MaThanhPhamDinhHinh)
        {
          if (_context.RP_MaThanhPhamDinhHinh == null)
          {
              return Problem("Entity set 'dbPMScontext.RP_MaThanhPhamDinhHinh'  is null.");
          }
            _context.RP_MaThanhPhamDinhHinh.Add(rP_MaThanhPhamDinhHinh);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RP_MaThanhPhamDinhHinhExists(rP_MaThanhPhamDinhHinh.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = rP_MaThanhPhamDinhHinh.Ma }, rP_MaThanhPhamDinhHinh);
        }

        // DELETE: api/RP_MaThanhPhamDinhHinh/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.RP_MaThanhPhamDinhHinh == null)
            {
                return NotFound();
            }
            var rP_MaThanhPhamDinhHinh = await _context.RP_MaThanhPhamDinhHinh.FindAsync(id);
            if (rP_MaThanhPhamDinhHinh == null)
            {
                return NotFound();
            }

            _context.RP_MaThanhPhamDinhHinh.Remove(rP_MaThanhPhamDinhHinh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RP_MaThanhPhamDinhHinhExists(string id)
        {
            return (_context.RP_MaThanhPhamDinhHinh?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
