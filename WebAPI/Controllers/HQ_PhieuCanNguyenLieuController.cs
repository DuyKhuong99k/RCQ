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
    [Route("api/[controller]")]
    [ApiController]
    public class HQ_PhieuCanNguyenLieuController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_PhieuCanNguyenLieuController(dbPMScontext context)
        {
            _context = context;
        }

        
    }
}
