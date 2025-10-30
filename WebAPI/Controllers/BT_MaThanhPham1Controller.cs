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
    public class BT_MaThanhPham1Controller : ControllerBase
    {
        private readonly dbPMScontext _context;
        private MainViewModel Vm => MainViewModel.Instance;
        public BT_MaThanhPham1Controller(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/BT_MaThanhPham1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BT_MaThanhPham>>> Gets()
        {
          if (_context.BT_MaThanhPham == null)
          {
              return NotFound();
          }
            return Vm.VmBT_ThanhPham.Items;
        }

        // GET: api/BT_MaThanhPham1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BT_MaThanhPham>> Gets(string id)
        {
          if (_context.BT_MaThanhPham == null)
          {
              return NotFound();
          }
            var bT_MaThanhPham = Vm.VmBT_ThanhPham.Find(id);

            if (bT_MaThanhPham == null)
            {
                return NotFound();
            }

            return bT_MaThanhPham;
        }

        // PUT: api/BT_MaThanhPham1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, BT_MaThanhPham bT_MaThanhPham)
        {
            if (id != bT_MaThanhPham.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmBT_ThanhPham.Exists(id))
            {
                return NotFound();
            }
            Vm.VmBT_ThanhPham.Update_Command.Execute(bT_MaThanhPham);
            return NoContent();
        }

        // POST: api/BT_MaThanhPham1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BT_MaThanhPham>> Post(BT_MaThanhPham bT_MaThanhPham)
        {
          if (_context.BT_MaThanhPham == null)
          {
              return Problem("Entity set 'dbPMScontext.BT_MaThanhPham'  is null.");
          }
            _context.BT_MaThanhPham.Add(bT_MaThanhPham);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BT_MaThanhPhamExists(bT_MaThanhPham.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBT_MaThanhPham", new { id = bT_MaThanhPham.Ma }, bT_MaThanhPham);
        }

        // DELETE: api/BT_MaThanhPham1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.BT_MaThanhPham == null)
            {
                return NotFound();
            }
            var bT_MaThanhPham = await _context.BT_MaThanhPham.FindAsync(id);
            if (bT_MaThanhPham == null)
            {
                return NotFound();
            }

            _context.BT_MaThanhPham.Remove(bT_MaThanhPham);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BT_MaThanhPhamExists(string id)
        {
            return (_context.BT_MaThanhPham?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
