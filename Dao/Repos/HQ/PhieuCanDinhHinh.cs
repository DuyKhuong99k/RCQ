using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanDinhHinh
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanDinhHinh";
        private readonly string qrDelete = @"Delete [dbo].[PhieuCanDinhHinh] where  [Ngay] =@Ngay";

        private readonly string qrInsert = @"Insert Into PhieuCanDinhHinh ([Ngay],[MaNhanVien],[SuDung],[MaSanPham],[TenSanPham],[TrongLuongNhan],[TrongLuongTra],[DinhMucThucTe],[DinhMucYeuCau],[DanhGia],[SoRo],[_Status],[CaLamViec]) Values (@Ngay,@MaNhanVien,@SuDung,@MaSanPham,@TenSanPham,@TrongLuongNhan,@TrongLuongTra,@DinhMucThucTe,@DinhMucYeuCau,@DanhGia,@SoRo,@_Status,@CaLamViec)";

        private readonly string qrUpdate = @"

";

        private readonly string qrGetAll = "Select * from PhieuCanDinhHinh";

        public PhieuCanDinhHinh(string? _connectionString = null)
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
          public int Insert<TEntity>(List<TEntity> phieuCans)
        {
            try
            {
                var query =
                    "Insert Into PhieuCanDinhHinh ([Ngay],[MaNhanVien],[SuDung],[MaSanPham],[TenSanPham],[TrongLuongNhan],[TrongLuongTra],[DinhMucThucTe],[DinhMucYeuCau],[DanhGia],[SoRo],[_Status],[CaLamViec]) Values (@Ngay,@MaNhanVien,@SuDung,@MaSanPham,@TenSanPham,@TrongLuongNhan,@TrongLuongTra,@DinhMucThucTe,@DinhMucYeuCau,@DanhGia,@SoRo,@_Status,@CaLamViec)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var affectedRows = connection.Execute(query, phieuCans, transaction);
                        transaction.Commit();
                        return affectedRows;
                    }
                }
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
