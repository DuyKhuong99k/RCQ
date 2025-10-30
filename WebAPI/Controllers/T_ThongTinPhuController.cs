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
    public class T_ThongTinPhuController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_ThongTinPhuController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_ThongTinPhu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_ThongTinPhu>>> Gets()
        {
          if (_context.T_ThongTinPhu == null)
          {
              return NotFound();
          }
            return Vm.VmT_ThongTinPhu.Items;
        }

        // GET: api/T_ThongTinPhu/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_ThongTinPhu>> Get(string id)
        {
          if (_context.T_ThongTinPhu == null)
          {
              return NotFound();
          }
            var t_ThongTinPhu = Vm.VmT_ThongTinPhu.Find(id);

            if (t_ThongTinPhu == null)
            {
                return NotFound();
            }

            return t_ThongTinPhu;
        }

        // PUT: api/T_ThongTinPhu/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_ThongTinPhu t_ThongTinPhu)
        {
            if (id != t_ThongTinPhu.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_ThongTinPhu.Exists(t_ThongTinPhu))
            {
                return NotFound();
            }
            Vm.VmT_ThongTinPhu.Update_Command.Execute(t_ThongTinPhu);
            return NoContent();
        }

        // POST: api/T_ThongTinPhu
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_ThongTinPhu>> Post(T_ThongTinPhu t_ThongTinPhu)
        {
          if (_context.T_ThongTinPhu == null)
          {
              return Problem("Entity set 'dbPMScontext.T_ThongTinPhu'  is null.");
          }
            if (Vm.VmT_ThongTinPhu.Exists(t_ThongTinPhu))
            {
                return Conflict();
            }
            Vm.VmT_ThongTinPhu.Insert_Command.Execute(t_ThongTinPhu);
            return CreatedAtAction("Get", new { id = t_ThongTinPhu.Ma }, t_ThongTinPhu);
        }

        // DELETE: api/T_ThongTinPhu/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_ThongTinPhu == null)
            {
                return NotFound();
            }
            var t_ThongTinPhu = Vm.VmT_ThongTinPhu.Find(id);
            if (t_ThongTinPhu == null)
            {
                return NotFound();
            }

            Vm.VmT_ThongTinPhu.Delete_Command.Execute(t_ThongTinPhu);

            return NoContent();
        }

        private bool T_ThongTinPhuExists(string id)
        {
            return (_context.T_ThongTinPhu?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
