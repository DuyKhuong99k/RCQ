using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamNguyenLieu
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamNguyenLieu]
      WHERE [MaCa] = @MaCa 
      and [Ma] = @Ma";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamNguyenLieu]
           ([MaCa]
           ,[Ma]
           ,[Ten]
           ,[SuDung]
           ,[Min]
           ,[Max]
           ,[IsSNL]
           ,[IsNgopGhe]
           ,[IsNgopGheMuoi]
           ,[IsMuoiGhePhuPham]
           ,[IsNgopAoMuoi]
           ,[IsNgopAoPhuPham]
           ,[IsDatNho]
           ,[IsPhuPhamCaTap]
           ,[IsCaCanTin]
           ,[IsCaNgopXeMuoi]
           ,[IsNgopGhePhuPham]
           ,[IsNgopXePhuPham]
           ,[IsManh]
           ,[TyLeNuoc])
     VALUES
           (@MaCa
           ,@Ma
           ,@Ten
           ,@SuDung
           ,@Min
           ,@Max
           ,@IsSNL
           ,@IsNgopGhe
           ,@IsNgopGheMuoi
           ,@IsMuoiGhePhuPham
           ,@IsNgopAoMuoi
           ,@IsNgopAoPhuPham
           ,@IsDatNho
           ,@IsPhuPhamCaTap
           ,@IsCaCanTin
           ,@IsCaNgopXeMuoi
           ,@IsNgopGhePhuPham
           ,@IsNgopXePhuPham
           ,@IsManh
           ,@TyLeNuoc)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamNguyenLieu]
   SET 
      [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[Min] = @Min
      ,[Max] = @Max
      ,[IsSNL] = @IsSNL
      ,[IsNgopGhe] = @IsNgopGhe
      ,[IsNgopGheMuoi] = @IsNgopGheMuoi
      ,[IsMuoiGhePhuPham] = @IsMuoiGhePhuPham
      ,[IsNgopAoMuoi] = @IsNgopAoMuoi
      ,[IsNgopAoPhuPham] = @IsNgopAoPhuPham
      ,[IsDatNho] = @IsDatNho
      ,[IsPhuPhamCaTap] = @IsPhuPhamCaTap
      ,[IsCaCanTin] = @IsCaCanTin
      ,[IsCaNgopXeMuoi] = @IsCaNgopXeMuoi
      ,[IsNgopGhePhuPham] = @IsNgopGhePhuPham
      ,[IsNgopXePhuPham] = @IsNgopXePhuPham
      ,[IsManh] = @IsManh
      ,[TyLeNuoc] = @TyLeNuoc
 WHERE [MaCa] = @MaCa 
      and [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from MaThanhPhamNguyenLieu";

        public MaThanhPhamNguyenLieu()
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
