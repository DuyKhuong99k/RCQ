using Dapper;
using Dapper.Database;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dao.Repos.HQ
{
    public partial class BanCatTiet
    {
        private readonly string connectionString;
        private string tableName = @"BanCatTiet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BanCatTiet]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BanCatTiet]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[X1]
           ,[Y1]
           ,[X2]
           ,[Y2]
           ,[ColSpanX1]
           ,[ColSpanX2]
           ,[IsShowX1]
           ,[IsShowX2]
           ,[IsDetect]
           ,[IsFalse]
           ,[DoUuTienX1]
           ,[DoUuTienX2]
           ,[SoLanChiaCaX1]
           ,[SoLanChiaCaX2]
           ,[IsKhoaX1]
           ,[IsKhoaX2]
           ,[ThoiGianQuangDuongPheu1X1]
           ,[ThoiGianQuangDuongPheu1X2]
           ,[ThoiGianQuangDuongPheu2X1]
           ,[ThoiGianQuangDuongPheu2X2]
           ,[PlcAdr]
           ,[TimeOpen]
           ,[PlcValue]
           ,[VongChiaCa]
           ,[PlcYadr]
           ,[PlcYValue]
           ,[ViTri_HX1]
           ,[ViTri_HX2]
           ,[ThoiGianNhanCaX1]
           ,[ThoiGianNhanCaX2]
           ,[PlcOffAdr]
           ,[PlcOffValue]
           ,[IsCaMuoiX1]
           ,[IsCaMuoiX2])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
           ,@X1
           ,@Y1
           ,@X2
           ,@Y2
           ,@ColSpanX1
           ,@ColSpanX2
           ,@IsShowX1
           ,@IsShowX2
           ,@IsDetect
           ,@IsFalse
           ,@DoUuTienX1
           ,@DoUuTienX2
           ,@SoLanChiaCaX1
           ,@SoLanChiaCaX2
           ,@IsKhoaX1
           ,@IsKhoaX2
           ,@ThoiGianQuangDuongPheu1X1
           ,@ThoiGianQuangDuongPheu1X2
           ,@ThoiGianQuangDuongPheu2X1
           ,@ThoiGianQuangDuongPheu2X2
           ,@PlcAdr
           ,@TimeOpen
           ,@PlcValue
           ,@VongChiaCa
           ,@PlcYadr
           ,@PlcYValue
           ,@ViTri_HX1
           ,@ViTri_HX2
           ,@ThoiGianNhanCaX1
           ,@ThoiGianNhanCaX2
           ,@PlcOffAdr
           ,@PlcOffValue
           ,@IsCaMuoiX1
           ,IsCaMuoiX2)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BanCatTiet]
   SET [Ten] = @Ten
           ,[SuDung] = @SuDung
           ,[X1] = @X1
           ,[Y1] = @Y1
           ,[X2] =@X2
           ,[Y2] =@Y2
           ,[ColSpanX1] =@ColSpanX1
           ,[ColSpanX2] =@ColSpanX2
           ,[IsShowX1] =@IsShowX1
           ,[IsShowX2] = @IsShowX2
           ,[IsDetect]=@IsDetect
           ,[IsFalse]=@IsFalse
           ,[DoUuTienX1]=@DoUuTienX1
           ,[DoUuTienX2]=@DoUuTienX2
           ,[SoLanChiaCaX1]=@SoLanChiaCaX1
           ,[SoLanChiaCaX2]=@SoLanChiaCaX2
           ,[IsKhoaX1]=@IsKhoaX1
           ,[IsKhoaX2]=@IsKhoaX2
           ,[ThoiGianQuangDuongPheu1X1]=@ThoiGianQuangDuongPheu1X1
           ,[ThoiGianQuangDuongPheu1X2]=@ThoiGianQuangDuongPheu1X2
           ,[ThoiGianQuangDuongPheu2X1]=@ThoiGianQuangDuongPheu2X1
           ,[ThoiGianQuangDuongPheu2X2]=@ThoiGianQuangDuongPheu2X2
           ,[PlcAdr]=@PlcAdr
           ,[TimeOpen]=@TimeOpen
           ,[PlcValue]=@PlcValue
           ,[VongChiaCa]=@VongChiaCa
           ,[PlcYadr]=@PlcYadr
           ,[PlcYValue]=@PlcYValue
           ,[ViTri_HX1]=@ViTri_HX1
           ,[ViTri_HX2]=@ViTri_HX2
           ,[ThoiGianNhanCaX1]=@ThoiGianNhanCaX1
           ,[ThoiGianNhanCaX2]=@ThoiGianNhanCaX2
           ,[PlcOffAdr]=@PlcOffAdr
           ,[PlcOffValue]=@PlcOffValue
           ,[IsCaMuoiX1]=@IsCaMuoiX1
           ,[IsCaMuoiX2]=@IsCaMuoiX2
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from BanCatTiet";


        public BanCatTiet(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

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
            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
            return rows;
        }
        public List<T> Gets<T>(bool suDung)
        {
            try
            {
                var query = "Select * from BanCatTiet Where SuDung = @suDung";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { suDung }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
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
