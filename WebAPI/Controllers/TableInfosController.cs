using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TableInfosController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public TableInfosController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{tableName}")]
        [Authorize]
        public IActionResult GetAllsFullField(string tableName)
        {
            var items = Vm.VmTableInfo.GetsTableInfo<object>(tableName, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                string backupFileName = $"backup_{DateTime.Now:yyyyMMddHHmmss}.bak";

                string connectionString = AppViewModels.Base.Ins.ConnectionString2; // nhớ đổi thành connetxtionstring
                var builder = new SqlConnectionStringBuilder(connectionString);
                string databaseName = builder.InitialCatalog;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"BACKUP DATABASE {databaseName} TO DISK = '{backupFileName}'";
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return Ok(new ApiResponse { Success = true, Message = "Tạo file Backup thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Success = false, Message = ex.Message });
            }
        }



        [HttpPost("{tableName}/{dateKeep}")]
        [Authorize]
        public async Task<IActionResult> DeleteOldData(string tableName, int dateKeep)
        {
            try
            {
                string connectionString = AppViewModels.Base.Ins.ConnectionString2; // nhớ đổi thành connection string
                //string deleteQuery = $@"
                //                DELETE FROM {tableName}
                //                WHERE Ngay < DATEADD(day, -{dateKeep}, (SELECT MAX(Ngay) FROM {tableName}))";
                string deleteQuery = $@"
                                SELECT 1";

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(deleteQuery, connection))
                    {
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return Ok(new { Success = true, Message = $"{rowsAffected} dòng dữ liệu đã được xóa từ bảng {tableName}." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

    }


}
