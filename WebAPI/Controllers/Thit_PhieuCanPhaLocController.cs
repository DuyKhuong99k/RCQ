using Microsoft.AspNetCore.Mvc;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class Thit_PhieuCanPhaLocController : ControllerBase
{
    private readonly dbPMScontext _context;

    public Thit_PhieuCanPhaLocController(dbPMScontext context)
    {
        _context = context;
    }

    private MainViewModel Vm => MainViewModel.Instance;

    // DELETE: api/Thit_PhieuCanPhaLoc/5
    [HttpDelete("{dateTime,xuongId,mayCan,stt}")]
    public async Task<IActionResult> DeleteThit_PhieuCanPhaLoc(DateTime dateTime, string xuongId,
        string mayCan, int stt)
    {
        if (_context.Thit_PhieuCanPhaLoc == null) return NotFound();
        var thit_PhieuCanPhaLoc = Vm.VmThit_PhieuCanPhaLoc.Find(dateTime, xuongId, mayCan, stt);
        if (thit_PhieuCanPhaLoc == null) return NotFound();
        Vm.VmThit_PhieuCanPhaLoc.Delete_Command.Execute(thit_PhieuCanPhaLoc);
        return NoContent();
    }

    // GET: api/Thit_PhieuCanPhaLoc/5
    [HttpGet("{dateTime,xuongId,mayCan,stt}")]
    public async Task<ActionResult<Thit_PhieuCanPhaLoc>> Get(DateTime dateTime, string xuongId,
        string mayCan, int stt)
    {
        if (_context.Thit_PhieuCanPhaLoc == null) return NotFound();

        var thit_PhieuCanPhaLoc = Vm.VmThit_PhieuCanPhaLoc.Find(dateTime, xuongId, mayCan, stt);

        if (thit_PhieuCanPhaLoc == null) return NotFound();

        return thit_PhieuCanPhaLoc;
    }

    // GET: api/Thit_PhieuCanPhaLoc
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Thit_PhieuCanPhaLoc>>> Gets()
    {
        if (_context.Thit_PhieuCanPhaLoc == null) return NotFound();

        return Vm.VmThit_PhieuCanPhaLoc.Items;
    }

    // POST: api/Thit_PhieuCanPhaLoc
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("{dateTime,xuongId,mayCan,stt}")]
    public async Task<ActionResult<Thit_PhieuCanPhaLoc>> Post(DateTime dateTime, string xuongId,
        string mayCan, int stt,
        Thit_PhieuCanPhaLoc thit_PhieuCanPhaLoc)
    {
        if (_context.Thit_PhieuCanPhaLoc == null)
            return Problem("Entity set 'dbPMScontext.Thit_PhieuCanPhaLoc'  is null.");
        if (Vm.VmThit_PhieuCanPhaLoc.Exists(thit_PhieuCanPhaLoc)) return Conflict();
        Vm.VmThit_SizePhaLoc.Insert_Command.Execute(thit_PhieuCanPhaLoc);
        return CreatedAtAction("Get",
            new
            {
                dateTime = thit_PhieuCanPhaLoc.Ngay.Date, xuongId = thit_PhieuCanPhaLoc.MaXuong,
                mayCan = thit_PhieuCanPhaLoc.MaMayCan, stt = thit_PhieuCanPhaLoc.STT
            }, thit_PhieuCanPhaLoc);
    }

    // PUT: api/Thit_PhieuCanPhaLoc/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{dateTime,xuongId,mayCan,stt}")]
    public async Task<IActionResult> Put(DateTime dateTime, string xuongId,
        string mayCan, int stt, Thit_PhieuCanPhaLoc thit_PhieuCanPhaLoc)
    {
        if (stt != thit_PhieuCanPhaLoc.STT || dateTime.Date != thit_PhieuCanPhaLoc.Ngay.Date ||
            xuongId != thit_PhieuCanPhaLoc.MaXuong || mayCan != thit_PhieuCanPhaLoc.MaMayCan) return BadRequest();

        if (!Vm.VmThit_PhieuCanPhaLoc.Exists(thit_PhieuCanPhaLoc)) return NotFound();
        Vm.VmThit_PhieuCanPhaLoc.Update_Command.Execute(thit_PhieuCanPhaLoc);
        return NoContent();
    }
}