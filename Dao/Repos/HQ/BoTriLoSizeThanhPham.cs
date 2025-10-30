using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BoTriLoSizeThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"BoTriLoSizeThanhPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BoTriLoSizeThanhPham]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BoTriLoSizeThanhPham]
           ([Id]
           ,[MaLo]
           ,[MaViTri]
           ,[MaSize]
           ,[MaThanhPham]
           ,[Ngay]
           ,[Gio]
           ,[CodeId]
           ,[MaSizePhu])
     VALUES
           (@Id
           ,@MaLo
           ,@MaViTri
           ,@MaSize
           ,@MaThanhPham
           ,@Ngay
           ,@Gio
           ,@CodeId
           ,@MaSizePhu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BoTriLoSizeThanhPham]
   SET [MaLo] =@MaLo
           ,[MaViTri] =@MaViTri
           ,[MaSize]=@MaSize
           ,[MaThanhPham]=@MaThanhPham
           ,[Ngay]=@Ngay
           ,[Gio]=@Gio
           ,[CodeId]=@CodeId
           ,[MaSizePhu]=@MaSizePhu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from BoTriLoSizeThanhPham";

        public BoTriLoSizeThanhPham()
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
