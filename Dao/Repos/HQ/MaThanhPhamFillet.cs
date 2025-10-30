using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamFillet
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamFillet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamFillet]
      WHERE [MaCa] = @MaCa 
      and [Ma] = @Ma";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamFillet]
           ([MaCa]
           ,[Ma]
           ,[Ten]
           ,[SuDung]
           ,[Min]
           ,[Max],[IsSoChe],[IsCaMuoi],[DinhMuc],[BravoId],[ThoiGianTren1kgSeconds],[DinhMucHaoHut],[CodeId],[IsNotSetByTime],[ColorRGB],[KhongPhanBietSize],[IsXeBuom],[IsChuyenFillet],[IsDat],[IsNguyenLieuXeBuom],[IsNguyenCon],[ThoiGianHT])
     VALUES
           (@MaCa 
           ,@Ma 
           ,@Ten 
           ,@SuDung  
           ,@Min 
           ,@Max,@IsSoChe,@IsCaMuoi,@DinhMuc,@BravoId,@ThoiGianTren1kgSeconds,@DinhMucHaoHut, @CodeId,@IsNotSetByTime,@ColorRGB,@KhongPhanBietSize,@IsXeBuom,@IsChuyenFillet,@IsDat,@IsNguyenLieuXeBuom,@IsNguyenCon, @ThoiGianHT)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamFillet]
   SET [Ten] = @Ten 
      ,[SuDung] = @SuDung 
      ,[Min] = @Min 
      ,[Max] = @Max
        ,[IsSoChe] = @IsSoChe, [IsCaMuoi] = @IsCaMuoi, [DinhMuc] = @DinhMuc,[BravoId] = @BravoId,[ThoiGianTren1kgSeconds] = @ThoiGianTren1kgSeconds , [DinhMucHaoHut] =@DinhMucHaoHut,[CodeId] = @CodeId,[IsNotSetByTime]= @IsNotSetByTime,[ColorRGB] = @ColorRGB, [KhongPhanBietSize]=@KhongPhanBietSize, [IsXeBuom]=@IsXeBuom, IsChuyenFillet=@IsChuyenFillet,[IsDat] = @IsDat, [IsNguyenLieuXeBuom] = @IsNguyenLieuXeBuom, [IsNguyenCon] = @IsNguyenCon, [ThoiGianHT] = @ThoiGianHT
 WHERE [MaCa] = @MaCa 
      and [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from MaThanhPhamFillet";

        public MaThanhPhamFillet()
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
