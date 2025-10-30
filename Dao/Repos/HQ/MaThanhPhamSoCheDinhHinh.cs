using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamSoCheDinhHinh
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamSoCheDinhHinh";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamSoCheDinhHinh]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamSoCheDinhHinh]
           ([Ma]
           ,[Ten]
           ,[X]
           ,[Y]
           ,[Min]
           ,[Max]
           ,[TinhKiem]
           ,[TinhPhucVu]
           ,[CaMuoi]
           ,[NguyenLieu]
           ,[Ban09]
           ,[SuDung]
           ,[BravoId]
           ,[LoaiGui]
           ,[TruocLangDa]
           ,[SauLangDa]
           ,[Nhan]
           ,[TinhGio]
           ,[BatCO],[IsNhapTay],[Createdby]
           ,[CreatedDateTime]
           ,[Modifiedby]
           ,[ModifiedDateTime])
     VALUES
           (@Ma
           ,@Ten
           ,@X
           ,@Y
           ,@Min
           ,@Max
           ,@TinhKiem
           ,@TinhPhucVu
           ,@CaMuoi
           ,@NguyenLieu
           ,@Ban09
           ,@SuDung
           ,@BravoId
           ,@LoaiGui
           ,@TruocLangDa
           ,@SauLangDa
           ,@Nhan
           ,@TinhGio
           ,@BatCO,@IsNhapTay,@Createdby
           ,@CreatedDateTime
           ,@Modifiedby
           ,@ModifiedDateTime)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamSoCheDinhHinh]
   SET [Ten] = @Ten
      ,[X] = @X
      ,[Y] = @Y
      ,[Min] = @Min
      ,[Max] = @Max
      ,[TinhKiem] = @TinhKiem
      ,[TinhPhucVu] = @TinhPhucVu
      ,[CaMuoi] = @CaMuoi
      ,[NguyenLieu] = @NguyenLieu
      ,[Ban09] = @Ban09
      ,[SuDung] = @SuDung
      ,[BravoId] = @BravoId
      ,[LoaiGui] = @LoaiGui
      ,[TruocLangDa] = @TruocLangDa
      ,[SauLangDa] = @SauLangDa
      ,[Nhan] = @Nhan
      ,[TinhGio] = @TinhGio
      ,[BatCO] = @BatCO
      ,[IsNhapTay] =@IsNhapTay
,[Createdby] = @Createdby
      ,[CreatedDateTime] = @CreatedDateTime
      ,[Modifiedby] = @Modifiedby
      ,[ModifiedDateTime] = @ModifiedDateTime
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MaThanhPhamSoCheDinhHinh";

        public MaThanhPhamSoCheDinhHinh(string? _connectionString = null)
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
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
            return rows;
        }
        public List<Models.Repos.Models.MaThanhPhamSoCheDinhHinh> Gets(bool loaiGui)
        {
            try
            {
                var query = "Select * from MaThanhPhamSoCheDinhHinh where LoaiGui = @loaiGui";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaThanhPhamSoCheDinhHinh>(query, new { loaiGui = loaiGui })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Models.Repos.Models.MaThanhPhamSoCheDinhHinh> GetThanhPhamSoCheDinhHinhsCaMuoi(bool isCaMuoi)
        {
            try
            {
                var query = $@"Select * from MaThanhPhamSoCheDinhHinh where CaMuoi =@isCaMuoi and LoaiGui = 0";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaThanhPhamSoCheDinhHinh>(query, new { isCaMuoi = isCaMuoi })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
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
        public List<Models.Repos.Models.MaThanhPhamSoCheDinhHinh>GetThanhPhamSoCheDinhHinhsTinhGio(bool isTinhGio)
        {
            try
            {
                var query = $@"Select * from MaThanhPhamSoCheDinhHinh where TinhGio =@isTinhGio and LoaiGui = 0";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaThanhPhamSoCheDinhHinh>(
                            query,
                            new { isTinhGio = isTinhGio })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.MaThanhPhamSoCheDinhHinh> GetThanhPhamSoCheDinhHinhsTinhKiem(bool isTinhKiem)
        {
            try
            {
                var query = $@"Select * from MaThanhPhamSoCheDinhHinh where TinhKiem =@isTinhKiem and LoaiGui = 0";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaThanhPhamSoCheDinhHinh>(
                            query,
                            new { isTinhKiem = isTinhKiem })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.MaThanhPhamSoCheDinhHinh> GetThanhPhamSoCheDinhHinhsPhucVu(bool isPhucVu)
        {
            try
            {
                var query = $@"Select * from MaThanhPhamSoCheDinhHinh where TinhPhucVu =@isPhucVu and LoaiGui = 0";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaThanhPhamSoCheDinhHinh>(query, new { isPhucVu = isPhucVu })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
