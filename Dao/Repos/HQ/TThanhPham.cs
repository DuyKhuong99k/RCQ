using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"T_ThanhPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_ThanhPham]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_ThanhPham]
           ([Ma]
           ,[Ten]
           ,[MaKhuVuc]
           ,[SuDung],[IsPhanCo],[IsDem],[ChoPhepChuyenDoiQuyTrinh],[IsHoaChat],[IsThemLo],[IsBatMau])
     VALUES
           (@Ma
           ,@Ten
           ,@MaKhuVuc
           ,@SuDung,@IsPhanCo,@IsDem, @ChoPhepChuyenDoiQuyTrinh,@IsHoaChat,@IsThemLo,@IsBatMau)";
        private readonly string qrUpdate = @"UPDATE [dbo].[T_ThanhPham]
   SET [Ten] = @Ten
      ,[MaKhuVuc] = @MaKhuVuc
      ,[SuDung] = @SuDung, [IsPhanCo] = @IsPhanCo , [IsDem] = @IsDem, [ChoPhepChuyenDoiQuyTrinh] = @ChoPhepChuyenDoiQuyTrinh, [IsHoaChat] = @IsHoaChat, [IsThemLo] = @IsThemLo, [IsBatMau] = @IsBatMau
 WHERE [Ma] = @Ma
      ";

        private readonly string qrGetAll = "Select * from T_ThanhPham";

        public TThanhPham()
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
