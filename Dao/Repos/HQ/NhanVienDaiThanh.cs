using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Dao.Repos.HQ
{
    public partial class NhanVienDaiThanh
    {
        private readonly string connectionString;
        private string tableName = @"NhanVienDaiThanh";
        private readonly string qrDelete = @"";

        private readonly string qrInsert = @"
Insert Into NhanVienDaiThanh ([MaNhanVien],[MaChamCong],[MaHoSo],[Xuong],[Name],[BirthDate],[DeptCode0],[DeptName0],[ChucVu],[GenderName],[Tel],[Address],[JobPositionName0],[FirstWorkingDate],[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]) Values (@MaNhanVien,@MaChamCong,@MaHoSo,@Xuong,@Name,@BirthDate,@DeptCode0,@DeptName0,@ChucVu,@GenderName,@Tel,@Address,@JobPositionName0,@FirstWorkingDate,@IsShowDinhMuc,@IsContracting,@IsPhucVu,@IsHuman,@IsGiaCong,@AC,@IsChucNang,@LoaiSanLuong,@IsBanKiem,@IsNhanVienCat,@IsNhom)";

        private readonly string qrUpdate = @"UPDATE [dbo].[NhanVienDaiThanh]
   SET [MaChamCong] = @MaChamCong 
      ,[MaHoSo] = @MaHoSo 
      ,[Xuong] = @Xuong 
      ,[Name] = @Name 
      ,[BirthDate] = @BirthDate 
      ,[DeptCode0] = @DeptCode0 
      ,[DeptName0] = @DeptName0 
      ,[ChucVu] = @ChucVu 
      ,[GenderName] = @GenderName 
      ,[Tel] = @Tel 
      ,[Address] = @Address 
      ,[JobPositionName0] = @JobPositionName0 
      ,[FirstWorkingDate] = @FirstWorkingDate
        ,[IsShowDinhMuc] = @IsShowDinhMuc,[IsContracting] =@IsContracting,[IsPhucVu] =@IsPhucVu, [IsHuman]= @IsHuman, [IsGiaCong]=@IsGiaCong, [AC] = @AC,[IsChucNang] = @IsChucNang,[LoaiSanLuong] =@LoaiSanLuong,[IsBanKiem] = @IsBanKiem , [IsNhanVienCat] = @IsNhanVienCat,[IsNhom] =@IsNhom
 WHERE [MaNhanVien] = @MaNhanVien";

        private readonly string qrGetAll = "Select * from NhanVienDaiThanh";

        public NhanVienDaiThanh(string? _connectionString = null)
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
        public List<Models.Repos.Models.NhanVienDaiThanh> GetNhanVienDaiThanhs()
        {
            try
            {
                var query = @"Select [MaNhanVien]
      ,[MaChamCong]
      ,[MaHoSo]
      ,[Xuong]
      ,[Name]
      ,[BirthDate]
      ,[DeptCode0]
      ,[DeptName0]
      ,[ChucVu]
      ,[GenderName]
      ,[Tel]
      ,[Address]
      ,[JobPositionName0]
      ,[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]
      ,(CASE
    WHEN  [FirstWorkingDate] =N'Đang Làm' THEN 'HD'
    ELSE '-'
END) as  [FirstWorkingDate] from NhanVienDaiThanh order by MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienDaiThanh>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Models.Repos.Models.NhanVienDaiThanh> GetNhanVienDaiThanhs(string xuongId, string maHoSoStartWith)
        {
            try
            {
                var query =
                    @"Select [MaNhanVien]
      ,[MaChamCong]
      ,[MaHoSo]
      ,[Xuong]
      ,[Name]
      ,[BirthDate]
      ,[DeptCode0]
      ,[DeptName0]
      ,[ChucVu]
      ,[GenderName]
      ,[Tel]
      ,[Address]
      ,[JobPositionName0]
        ,[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]
      ,(CASE
    WHEN[FirstWorkingDate] = N'Đang Làm' THEN 'HD'
    ELSE '-'
END) as  [FirstWorkingDate] from NhanVienDaiThanh where Xuong =@xuongId and LEFT(MaHoSo,1) = @maHoSoStartWith and ISNUMERIC( RIGHT(MaHoSo,LEN(MaHoSo) -LEN(@maHoSoStartWith)) ) = 1 order by CAST( RIGHT(MaHoSo,LEN(MaHoSo) -LEN(@maHoSoStartWith)) as int)";
                using (var connection = new SqlConnection(connectionString: connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.NhanVienDaiThanh>(
                            query,
                            new { xuongId = xuongId, maHoSoStartWith = maHoSoStartWith })
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
        public List<Models.Repos.Models.NhanVienDaiThanh> GetNhanVienDaiThanhs(IEnumerable<string> ids)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                var query = $@"Select [MaNhanVien]
      ,[MaChamCong]
      ,[MaHoSo]
      ,[Xuong]
      ,[Name]
      ,[BirthDate]
      ,[DeptCode0]
      ,[DeptName0]
      ,[ChucVu]
      ,[GenderName]
      ,[Tel]
      ,[Address]
      ,[JobPositionName0]
        ,[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]
      ,(CASE
    WHEN  [FirstWorkingDate] =N'Đang Làm' THEN 'HD'
    ELSE '-'
END) as  [FirstWorkingDate] from NhanVienDaiThanh where MaNhanVien in {listOfIdsJoined}";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienDaiThanh>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Models.Repos.Models.NhanVienDaiThanh> GetNhanVienDaiThanhs(string xuongId, int startMaHoSo, int endMaHoSo)
        {
            try
            {
                var query =
                    @"Select [MaNhanVien]
      ,[MaChamCong]
      ,[MaHoSo]
      ,[Xuong]
      ,[Name]
      ,[BirthDate]
      ,[DeptCode0]
      ,[DeptName0]
      ,[ChucVu]
      ,[GenderName]
      ,[Tel]
      ,[Address]
      ,[JobPositionName0]
,[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]
      ,(CASE
    WHEN  [FirstWorkingDate] =N'Đang Làm' THEN 'HD'
    ELSE '-'
END) as  [FirstWorkingDate] from NhanVienDaiThanh where Xuong =@xuongId and Len(MaHoSo)>=3 and (case when ISNUMERIC(MaHoSo) = 1 then cast (MaHoSo as int) else -1
                end) between @batdau and @ketthuc  order by (case when ISNUMERIC(MaHoSo) = 1 then cast (MaHoSo as int) else -1
                end)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienDaiThanh>(
                            query,
                            new { xuongId = xuongId, batdau = startMaHoSo, ketthuc = endMaHoSo })
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
        public List<Models.Repos.Models.NhanVienDaiThanh> GetNhanVienDaiThanhsNotIn(string xuongId, List<Models.Repos.Models.NhanVienDaiThanh> nhanVienDaiThanhs)
        {
            try
            {
                var ids = nhanVienDaiThanhs.Select(x => x.MaNhanVien);
                var query =
                    @"Select [MaNhanVien]
      ,[MaChamCong]
      ,[MaHoSo]
      ,[Xuong]
      ,[Name]
      ,[BirthDate]
      ,[DeptCode0]
      ,[DeptName0]
      ,[ChucVu]
      ,[GenderName]
      ,[Tel]
      ,[Address]
      ,[JobPositionName0]
,[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]
      ,(CASE
    WHEN  [FirstWorkingDate] =N'Đang Làm' THEN 'HD'
    ELSE '-'
END) as  [FirstWorkingDate] from NhanVienDaiThanh where Xuong =@xuongId and Len(MaHoSo)>=3 and MaNhanVien Not In @ids and MaHoSo Is Not Null and MaHoSo <> ''  order by (case when ISNUMERIC(MaHoSo) = 1 then cast (MaHoSo as int) else -1
                end)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienDaiThanh>(query, new { xuongId = xuongId, ids = ids })
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
        public List<Models.Repos.Models.NhanVienDaiThanh> GetNhanVienDaiThanhs(string xuongId)
        {
            try
            {
                var query = @"Select [MaNhanVien]
      ,[MaChamCong]
      ,[MaHoSo]
      ,[Xuong]
      ,[Name]
      ,[BirthDate]
      ,[DeptCode0]
      ,[DeptName0]
      ,[ChucVu]
      ,[GenderName]
      ,[Tel]
      ,[Address]
      ,[JobPositionName0]
      ,[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]
      ,(CASE
    WHEN  [FirstWorkingDate] =N'Đang Làm' THEN 'HD'
    ELSE '-'
END) as  [FirstWorkingDate] from NhanVienDaiThanh where Xuong=@xuongId and Len(MaHoSo)>=3";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienDaiThanh>(query, new { xuongId = xuongId }).Result
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
        public List<Models.Repos.Models.NhanVienDaiThanh> GetNhanVienDaiThanhInIds(string ids)
        {
            try
            {
                var query = $@"Select [MaNhanVien]
      ,[MaChamCong]
      ,[MaHoSo]
      ,[Xuong]
      ,[Name]
      ,[BirthDate]
      ,[DeptCode0]
      ,[DeptName0]
      ,[ChucVu]
      ,[GenderName]
      ,[Tel]
      ,[Address]
      ,[JobPositionName0]
      ,[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]
      ,(CASE
    WHEN  [FirstWorkingDate] =N'Đang Làm' THEN 'HD'
    ELSE '-'
END) as  [FirstWorkingDate] from NhanVienDaiThanh where MaNhanVien in {ids}";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienDaiThanh>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.NhanVienDaiThanh> GetByMaHoSo(string maHoSo)
        {
            try
            {
                var query = @"Select [MaNhanVien]
      ,[MaChamCong]
      ,[MaHoSo]
      ,[Xuong]
      ,[Name]
      ,[BirthDate]
      ,[DeptCode0]
      ,[DeptName0]
      ,[ChucVu]
      ,[GenderName]
      ,[Tel]
      ,[Address]
      ,[JobPositionName0]
      ,[IsShowDinhMuc],[IsContracting],[IsPhucVu],[IsHuman],[IsGiaCong],[AC],[IsChucNang],[LoaiSanLuong],[IsBanKiem],[IsNhanVienCat],[IsNhom]
      ,(CASE
    WHEN  [FirstWorkingDate] =N'Đang Làm' THEN 'HD'
    ELSE '-'
END) as  [FirstWorkingDate] from NhanVienDaiThanh where MaHoSo = @maHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.NhanVienDaiThanh>(query, new { maHoSo = maHoSo }).Result
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
