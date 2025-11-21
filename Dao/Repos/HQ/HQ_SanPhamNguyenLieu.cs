using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Repos.HQ
{
    public partial class HQ_SanPhamNguyenLieu
    {
        private readonly string connectionString;
        private string tableName = @"HQ_SanPhamNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_SanPhamNguyenLieu]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[HQ_SanPhamNguyenLieu]
           ([Id]
           ,[Ten]
           ,[SuDung]
           ,[Min]
           ,[Max]
           ,[MaQuyCach])
     VALUES
           (@Id 
           ,@Ten 
           ,@SuDung 
           ,@Min
           ,@Max
           ,@MaQuyCach)";

        private readonly string qrUpdate = @"UPDATE [dbo].[HQ_SanPhamNguyenLieu]
   SET [Ten] = @Ten  
      ,[SuDung] = @SuDung 
      ,[Min] = @Min
      ,[Max] = @Max
      ,[MaQuyCach]
 WHERE  [Id] = @Id";

        private readonly string qrGetAll = "Select * from HQ_SanPhamNguyenLieu";

        public HQ_SanPhamNguyenLieu()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }

        public int Delete<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, item);
            return rows;
        }

        public List<T> Gets<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
            return rows;
        }
        public List<T> GetAllsFullField<T>()
        {
            var query = @"select
sp.Id,
sp.Ten,
sp.SuDung,
sp.MNgay,
sp.Min,
sp.Max,
sp.MaQuyCach,
qc.Ten as TenQuyCach
from HQ_SanPhamNguyenLieu sp
left join HQ_QuyCachNguyenLieu qc on qc.Id = sp.MaQuyCach order by sp.MNgay desc";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query)
                .ToList();
            return items;
        }

        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }

        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
    }
}
