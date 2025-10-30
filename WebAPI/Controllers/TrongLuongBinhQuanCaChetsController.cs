using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class TrongLuongBinhQuanCaChetsController : ControllerBase
{
    private readonly dbPMScontext _context;

    public TrongLuongBinhQuanCaChetsController(dbPMScontext context)
    {
        _context = context;
    }

    private MainViewModel Vm => MainViewModel.Instance;

    // DELETE: api/TrongLuongBinhQuanCaChets/5
    [HttpDelete("{dateTime,maAo}")]
    public async Task<IActionResult> Delete(DateTime dateTime,string maAo)
    {
        if (_context.TrongLuongBinhQuanCaChet == null) return NotFound();
        var trongLuongBinhQuanCaChet = Vm.VmTrongLuongBinhQuanCaChet.Find(dateTime,maAo);
        if (trongLuongBinhQuanCaChet == null) return NotFound();

    Vm.VmTrongLuongBinhQuanCaChet.Delete_Command.Execute(trongLuongBinhQuanCaChet);
        return NoContent();
    }

    // GET: api/TrongLuongBinhQuanCaChets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrongLuongBinhQuanCaChet>>> Gets()
    {
        if (_context.TrongLuongBinhQuanCaChet == null) return NotFound();

        return Vm.VmTrongLuongBinhQuanCaChet.Items;
    }

    // GET: api/TrongLuongBinhQuanCaChets/5
    [HttpGet("{dateTime,maAo}")]
    public async Task<ActionResult<TrongLuongBinhQuanCaChet>> Get(DateTime dateTime,string maAo)
    {
        if (_context.TrongLuongBinhQuanCaChet == null) return NotFound();
        var trongLuongBinhQuanCaChet = Vm.VmTrongLuongBinhQuanCaChet.Find(dateTime.Date, maAo);

        if (trongLuongBinhQuanCaChet == null) return NotFound();

        return trongLuongBinhQuanCaChet;
    }

    // POST: api/TrongLuongBinhQuanCaChets
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TrongLuongBinhQuanCaChet>> Post(
        TrongLuongBinhQuanCaChet trongLuongBinhQuanCaChet)
    {
        if (_context.TrongLuongBinhQuanCaChet == null)
            return Problem("Entity set 'dbPMScontext.TrongLuongBinhQuanCaChet'  is null.");
        _context.TrongLuongBinhQuanCaChet.Add(trongLuongBinhQuanCaChet);
        if (Vm.VmTrongLuongBinhQuanCaChet.Exists(trongLuongBinhQuanCaChet))
        {
            return Conflict();
        }
       Vm.VmTrongLuongBinhQuanCaChet.Insert_Command.Execute(trongLuongBinhQuanCaChet);

        return CreatedAtAction("Get", new { dateTime = trongLuongBinhQuanCaChet.Ngay,maAo = trongLuongBinhQuanCaChet.MaAo },
            trongLuongBinhQuanCaChet);
    }

    // PUT: api/TrongLuongBinhQuanCaChets/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{dateTime,maAo}")]
    public async Task<IActionResult> Put(DateTime dateTime,string maAo,
        TrongLuongBinhQuanCaChet trongLuongBinhQuanCaChet)
    {
        if (dateTime.Date != trongLuongBinhQuanCaChet.Ngay.Date || maAo != trongLuongBinhQuanCaChet.MaAo) return BadRequest();

        if (!Vm.VmTrongLuongBinhQuanCaChet.Exists(trongLuongBinhQuanCaChet)) return NotFound();

        Vm.VmTrongLuongBinhQuanCaChet.Update_Command.Execute(trongLuongBinhQuanCaChet);

        return NoContent();
    }
}