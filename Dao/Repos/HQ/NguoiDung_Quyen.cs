using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class NguoiDung_Quyen
    {
        private readonly string connectionString;
        private string tableName = @"NguoiDung_Quyen";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NguoiDung_Quyen]
      WHERE [TenNguoiDung] = @TenNguoiDung
      and [NhomNguoiDung] = @NhomNguoiDung
      and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[NguoiDung_Quyen]
           ([TenNguoiDung]
           ,[NhomNguoiDung]
           ,[Xem]
           ,[Them]
           ,[Sua]
           ,[Xoa]
           ,[KetChuyen]
           ,[CaiDat]
           ,[ChamGio]
           ,[Loai]
           ,[MaXuong])
     VALUES
           (@TenNguoiDung
           ,@NhomNguoiDung
           ,@Xem
           ,@Them
           ,@Sua
           ,@Xoa
           ,@KetChuyen
           ,@CaiDat
           ,@ChamGio
           ,@Loai
           ,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[NguoiDung_Quyen]
   SET [Xem] = @Xem
      ,[Them] = @Them
      ,[Sua] = @Sua
      ,[Xoa] = @Xoa
      ,[KetChuyen] = @KetChuyen
      ,[CaiDat] = @CaiDat
      ,[ChamGio] = @ChamGio
      ,[Loai] = @Loai
     
 WHERE [TenNguoiDung] = @TenNguoiDung
      and [NhomNguoiDung] = @NhomNguoiDung
      and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from NguoiDung_Quyen";

        public NguoiDung_Quyen()
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
