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
    public class NhanVienTheoLinesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhanVienTheoLinesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NhanVienTheoLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienTheoLine>>> Gets()
        {
          if (_context.NhanVienTheoLine == null)
          {
              return NotFound();
          }
            return Vm.VmNhanVienTheoLine.Items;
        }

        // GET: api/NhanVienTheoLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienTheoLine>> Get(int id)
        {
          if (_context.NhanVienTheoLine == null)
          {
              return NotFound();
          }
            var nhanVienTheoLine = Vm.VmNhanVienTheoLine.Find(id);

            if (nhanVienTheoLine == null)
            {
                return NotFound();
            }

            return nhanVienTheoLine;
        }

        // PUT: api/NhanVienTheoLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, NhanVienTheoLine nhanVienTheoLine)
        {
            if (id != nhanVienTheoLine.Id)
            {
                return BadRequest();
            }

            if (!Vm.VmNhanVienTheoLine.Exists(nhanVienTheoLine))
            {
                return NotFound();
            }
            Vm.VmNhanVienTheoLine.Update_Command.Execute(nhanVienTheoLine);
            return NoContent();
        }

        // POST: api/NhanVienTheoLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhanVienTheoLine>> Post(NhanVienTheoLine nhanVienTheoLine)
        {
          if (_context.NhanVienTheoLine == null)
          {
              return Problem("Entity set 'dbPMScontext.NhanVienTheoLine'  is null.");
          }
            if (Vm.VmNhanVienTheoLine.Exists(nhanVienTheoLine))
            {
                return Conflict();
            }
            Vm.VmNhanVienTheoLine.Insert_Command.Execute(nhanVienTheoLine);
            return CreatedAtAction("Get", new { id = nhanVienTheoLine.Id }, nhanVienTheoLine);
        }

        // DELETE: api/NhanVienTheoLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.NhanVienTheoLine == null)
            {
                return NotFound();
            }
            var nhanVienTheoLine = Vm.VmNhanVienTheoLine.Find(id);
            if (nhanVienTheoLine == null)
            {
                return NotFound();
            }

            Vm.VmNhanVienTheoLine.Delete_Command.Execute(nhanVienTheoLine);

            return NoContent();
        }

        private bool NhanVienTheoLineExists(int id)
        {
            return (_context.NhanVienTheoLine?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
