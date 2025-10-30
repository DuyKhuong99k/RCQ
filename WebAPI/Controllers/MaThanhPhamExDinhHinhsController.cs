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
    public class MaThanhPhamExDinhHinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamExDinhHinhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThanhPhamExDinhHinhs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamExDinhHinh>>> Gets()
        {
          if (_context.MaThanhPhamExDinhHinh == null)
          {
              return NotFound();
          }
            return await _context.MaThanhPhamExDinhHinh.ToListAsync();
        }

        // GET: api/MaThanhPhamExDinhHinhs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamExDinhHinh>> Get(string id)
        {
          if (_context.MaThanhPhamExDinhHinh == null)
          {
              return NotFound();
          }
            var maThanhPhamExDinhHinh = await _context.MaThanhPhamExDinhHinh.FindAsync(id);

            if (maThanhPhamExDinhHinh == null)
            {
                return NotFound();
            }

            return maThanhPhamExDinhHinh;
        }

        // PUT: api/MaThanhPhamExDinhHinhs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThanhPhamExDinhHinh maThanhPhamExDinhHinh)
        {
            if (id != maThanhPhamExDinhHinh.Id)
            {
                return BadRequest();
            }

            _context.Entry(maThanhPhamExDinhHinh).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaThanhPhamExDinhHinhExists(id))
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

        // POST: api/MaThanhPhamExDinhHinhs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamExDinhHinh>> Post(MaThanhPhamExDinhHinh maThanhPhamExDinhHinh)
        {
          if (_context.MaThanhPhamExDinhHinh == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamExDinhHinh'  is null.");
          }
            _context.MaThanhPhamExDinhHinh.Add(maThanhPhamExDinhHinh);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaThanhPhamExDinhHinhExists(maThanhPhamExDinhHinh.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maThanhPhamExDinhHinh.Id }, maThanhPhamExDinhHinh);
        }

        // DELETE: api/MaThanhPhamExDinhHinhs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThanhPhamExDinhHinh == null)
            {
                return NotFound();
            }
            var maThanhPhamExDinhHinh = await _context.MaThanhPhamExDinhHinh.FindAsync(id);
            if (maThanhPhamExDinhHinh == null)
            {
                return NotFound();
            }

            _context.MaThanhPhamExDinhHinh.Remove(maThanhPhamExDinhHinh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaThanhPhamExDinhHinhExists(string id)
        {
            return (_context.MaThanhPhamExDinhHinh?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
