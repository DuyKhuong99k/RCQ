using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.A_Model;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashBoardsController : ControllerBase
    {
        private readonly dbPMScontext _context;
        public DashBoardsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;


        [HttpPost]
        public async Task<IActionResult> AddUserWaitView(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<ListUserWaitView>(dataT.Item1);
            string id = model.Id;
            DateTime thoiGian = model.ThoiGian;
            string xuongId = model.XuongId;
            Vm.VmDashBoard.AddUserWaitView(id,thoiGian,xuongId);
            return Ok();
        }
    }
}
