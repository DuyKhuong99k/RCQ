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
    public class T_QuyTrinhTheoNgayController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_QuyTrinhTheoNgayController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_QuyTrinhTheoNgay
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_QuyTrinhTheoNgay>>> Gets()
        {
          if (_context.T_QuyTrinhTheoNgay == null)
          {
              return NotFound();
          }
            return Vm.VmT_QuyTrinhTheoNgay.Items;
        }

        // GET: api/T_QuyTrinhTheoNgay/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_QuyTrinhTheoNgay>> Get(string id)
        {
          if (_context.T_QuyTrinhTheoNgay == null)
          {
              return NotFound();
          }
            var t_QuyTrinhTheoNgay = Vm.VmT_QuyTrinhTheoNgay.Find(id);

            if (t_QuyTrinhTheoNgay == null)
            {
                return NotFound();
            }

            return t_QuyTrinhTheoNgay;
        }

        // PUT: api/T_QuyTrinhTheoNgay/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_QuyTrinhTheoNgay t_QuyTrinhTheoNgay)
        {
            if (id != t_QuyTrinhTheoNgay.Id)
            {
                return BadRequest();
            }

            if (!Vm.VmT_QuyTrinhTheoNgay.Exists(t_QuyTrinhTheoNgay))
            {
                return NotFound();
            }
            Vm.VmT_QuyTrinhTheoNgay.Update_Command.Execute(t_QuyTrinhTheoNgay);
            return NoContent();
        }

        // POST: api/T_QuyTrinhTheoNgay
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_QuyTrinhTheoNgay>> Post(T_QuyTrinhTheoNgay t_QuyTrinhTheoNgay)
        {
          if (_context.T_QuyTrinhTheoNgay == null)
          {
              return Problem("Entity set 'dbPMScontext.T_QuyTrinhTheoNgay'  is null.");
          }
            if (Vm.VmT_QuyTrinhTheoNgay.Exists(t_QuyTrinhTheoNgay))
            {
                return Conflict();
            }
            Vm.VmT_QuyTrinhTheoNgay.Insert_Command.Execute(t_QuyTrinhTheoNgay);
            return CreatedAtAction("Get", new { id = t_QuyTrinhTheoNgay.Id }, t_QuyTrinhTheoNgay);
        }

        // DELETE: api/T_QuyTrinhTheoNgay/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_QuyTrinhTheoNgay == null)
            {
                return NotFound();
            }
            var t_QuyTrinhTheoNgay = Vm.VmT_QuyTrinhTheoNgay.Find(id);
            if (t_QuyTrinhTheoNgay == null)
            {
                return NotFound();
            }

            Vm.VmT_QuyTrinhTheoNgay.Delete_Command.Execute(t_QuyTrinhTheoNgay);

            return NoContent();
        }

        private bool T_QuyTrinhTheoNgayExists(string id)
        {
            return (_context.T_QuyTrinhTheoNgay?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
