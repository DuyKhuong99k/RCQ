using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanNguyenLieu
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanNguyenLieu]
      WHERE [MaMayTinhCan] = @MaMayTinhCan 
      and [MaUserCan] = @MaUserCan 
      and [ThoiGianCan] = @ThoiGianCan";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanNguyenLieu]
           ([MaMayTinhCan]
           ,[MaUserCan]
           ,[ThoiGianCan]
           ,[Ngay]
           ,[NhaCC]
           ,[MSL]
           ,[MaAo]
           ,[MaPhuongTien]
           ,[MaLoaiCa]
           ,[MaLoaiThanhPham]
           ,[MaSize]
           ,[MaMau]
           ,[MaBanCatTiet]
           ,[TrongLuong]
           ,[SuDung]
           ,[GhiChu]
           ,[MaXuongSanXuat],[TyLeNuoc],[TrongLuongOrg],[TrongLuongTare],[Pheu],[Chuyen])
     VALUES
           (@MaMayTinhCan 
           ,@MaUserCan 
           ,@ThoiGianCan 
           ,@Ngay 
           ,@NhaCC 
           ,@MSL 
           ,@MaAo 
           ,@MaPhuongTien 
           ,@MaLoaiCa 
           ,@MaLoaiThanhPham 
           ,@MaSize 
           ,@MaMau 
           ,@MaBanCatTiet 
           ,@TrongLuong 
           ,@SuDung 
           ,@GhiChu 
           ,@MaXuongSanXuat,@TyLeNuoc,@TrongLuongOrg,@TrongLuongTare,@Pheu,@Chuyen)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanNguyenLieu]
   SET [Ngay] = @Ngay 
      ,[NhaCC] = @NhaCC 
      ,[MSL] = @MSL 
      ,[MaAo] = @MaAo 
      ,[MaPhuongTien] = @MaPhuongTien 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaLoaiThanhPham] = @MaLoaiThanhPham 
      ,[MaSize] = @MaSize 
      ,[MaMau] = @MaMau 
      ,[MaBanCatTiet] = @MaBanCatTiet 
      ,[TrongLuong] = @TrongLuong 
      ,[SuDung] = @SuDung 
      ,[GhiChu] = @GhiChu 
      ,[MaXuongSanXuat] = @MaXuongSanXuat,
[TyLeNuoc] = @TyLeNuoc, [TrongLuongOrg] = @TrongLuongOrg,[TrongLuongTare] =@TrongLuongTare,[Pheu] = @Pheu, [Chuyen] =@Chuyen
 WHERE [MaMayTinhCan] = @MaMayTinhCan 
      and [MaUserCan] = @MaUserCan 
      and [ThoiGianCan] = @ThoiGianCan";

        private readonly string qrGetAll = "Select * from PhieuCanNguyenLieu";

        public PhieuCanNguyenLieu(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public List<Tuple<string, DateTime>> GetsAo(string xuongId, int topVal)
        {
            var query =
                "SELECT Top (@topVal) MaAo as Item1, Max(ThoiGianCan) as Item2 FROM PhieuCanNguyenLieu Where MaXuongSanXuat = @xuongId GROUP BY MaAo ORDER BY Max(ThoiGianCan) DESC";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<Tuple<string, DateTime>>(query, new { xuongId, topVal })
                .ToList();
            return items;
        }
        public List<string> GetsMSLDinhHinh(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetsMSLTPFillet(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(ThoiGianCan) as datetime) as ThoiGianCan,MSL as MSL,MaSize from PhieuCanTPFillet where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetsMSLDinhHinh_BTP(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetsMSLBTPFilletv2(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPFilletv2 where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetsMSL(string xuongId)
        {
            try
            {
                var query =
                    "Select  Ngay,MAX( ThoiGianCan) as ThoiGianCan ,MSL,MaSize from PhieuCanNguyenLieu where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeDinhHinh(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeCaoThit(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanCaoThit where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeTPFillet(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(ThoiGianCan) as datetime) as ThoiGianCan,MSL as MSL,MaSize from PhieuCanTPFillet where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeBTPFilletv2(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPFilletv2 where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeDinhHinh_BTP(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSize(string xuongId, int top = 4)
        {
            try
            {
                var query =
                    $@"SELECT Top {top} Cast( Max(ThoiGianCan)  as Date) as Item1, MSL as Item2,MaSize as Item3  FROM PhieuCanNguyenLieu Where MaXuongSanXuat = @xuongId GROUP BY MSL,MaSize ORDER BY Max(ThoiGianCan) DESC";

                //var query =
                //    "Select  Ngay,MAX( ThoiGianCan) as ThoiGianCan ,MSL,MaSize from PhieuCanNguyenLieu where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                //var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId = xuongId }).Result.ToList();
                //var items = (from p in itemrls
                //             group p by new { p.MSL, p.MaSize, p.Ngay }into g
                //             select new { MSL = g.Key.MSL, MaSize = g.Key.MaSize, Ngay = g.Key.Ngay }).Distinct()
                //    .ToList();
                //List<Tuple<DateTime?, string, string>> itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                //foreach(var item in items)
                //{
                //    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));
                //}

                //return itemsNeeds;
                var items = connection.Query<Tuple<DateTime?, string, string>>(query, new { xuongId })
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeDinhHinh(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeCaoThit(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanCaoThit where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeDinhHinh_BTP(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeTPFillet(string xuongId)
        {
            try
            {
                var query =
                    @"Select Ngay,Cast(max(ThoiGianCan) as datetime) as ThoiGianCan,MSL as MSL,MaSize from PhieuCanTPFillet where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeBTPFilletv2(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPFilletv2 where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));
                return itemsNeeds;
                //var query =
                //    @"Select  Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPFilletv2 where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                //using var connection = new SqlConnection(ConnectionString);
                //connection.Open();
                //var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId = xuongId }).Result.ToList();
                //var items = (from p in itemrls
                //             group p by new { p.MSL, p.MaSize, p.Ngay } into g
                //             select new { MSL = g.Key.MSL, MaSize = g.Key.MaSize, Ngay = g.Key.Ngay }).Distinct()
                //    .ToList();
                //List<Tuple<DateTime?, string, string>> itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                //foreach (var item in items)
                //{
                //    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));
                //}

                //return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSize(string xuongId)
        {
            try
            {
                var query =
                    @"SELECT Top 10 Cast( Max(ThoiGianCan)  as Date) as Item1, MSL as Item2,MaSize as Item3  FROM PhieuCanNguyenLieu Where MaXuongSanXuat = @xuongId GROUP BY MSL,MaSize ORDER BY Max(ThoiGianCan) DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                //var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId = xuongId }).Result.ToList();
                //var items = (from p in itemrls
                //             group p by new { p.MSL, p.MaSize, p.Ngay }into g
                //             select new { MSL = g.Key.MSL, MaSize = g.Key.MaSize, Ngay = g.Key.Ngay }).Distinct()
                //    .ToList();
                //List<Tuple<DateTime?, string, string>> itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                //foreach(var item in items)
                //{
                //    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));
                //}

                //return itemsNeeds;
                var items = connection.Query<Tuple<DateTime?, string, string>>(query, new { xuongId })
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
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
        public List<T> Gets<T>(DateTime dateTime)
        {
            var query = @"select p.*,Isnull (tp.Ten,'') as ThanhPhamName from (Select * from PhieuCanNguyenLieu Where Ngay = @ngay) p left join MaThanhPhamNguyenLieu tp on p.MaLoaiThanhPham = tp.Ma ";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
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
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var query = @"
WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayTinhCan ORDER BY Ngay DESC, ThoiGianCan DESC) AS RowNum
    FROM PhieuCanNguyenLieu where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, num })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopThanhPhamDashBoard<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var query = @"
select
p.MaLoaiThanhPham as MaThanhPham,
tp.Ten as ThanhPhamName,
Sum(p.TrongLuong) as TrongLuong,
p.MaXuongSanXuat as MaXuong
from
PhieuCanNguyenLieu p,
MaThanhPhamNguyenLieu tp
where
p.Ngay >= @fromDate
and p.Ngay <= @toDate
and p.MaXuongSanXuat = @xuongId
and p.MaLoaiThanhPham = tp.Ma
group by
p.MaLoaiThanhPham,
tp.Ten,
p.MaXuongSanXuat 
";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { fromDate, toDate, xuongId }).Result
                .ToList();
            return items;
        }
        private class ResultMSL
        {
            public string MaSize { get; set; }

            public string MSL { get; set; }

            public DateTime? Ngay { get; set; }

            public DateTime ThoiGianCan { get; set; }
        }

        #region Xử lý phiếu CÂN
        public List<T> GetPhieuCan<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"select
p.MaMayTinhCan,
p.MaUserCan,
p.ThoiGianCan,
p.Ngay,
p.NhaCC,
ncc.Ten as NhaCCName,
p.MSL,
p.MaAo,
ao.Ten as AoName,
p.MaPhuongTien,
pt.Ten as PhuongTienName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaBanCatTiet,
bct.Ten as BanCatTietName,
p.TrongLuong,
p.SuDung,
p.GhiChu
from PhieuCanNguyenLieu p
left join NhaCungCapNguyenLieu ncc on p.NhaCC = ncc.Ma
left join MaAoVungNuoi ao on p.MaAo = ao.Ma
left join PhuongTienChoNguyenLieu pt on p.MaPhuongTien = pt.Ma
left join MaLoaiCaNguyenLieu lc on  p.MaLoaiCa = lc.Ma
left join MaThanhPhamNguyenLieu tp on p.MaLoaiThanhPham = tp.Ma
left join MaSizeNguyenLieu s on p.MaSize = s.Ma
left join MaMauNguyenLieu m on p.MaMau = m.Ma
left join BanCatTiet bct on p.MaBanCatTiet = bct.Ma
where p.Ngay = @dateTime and p.MaXuongSanXuat = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { dateTime = dateTime.Date, xuongId }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public double GetSanLuong(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId)
        {
            var query =
                "Select ISNULL(SUM(TrongLuong),0) from PhieuCanNguyenLieu where Ngay = @ngay and CONVERT(time,ThoiGianCan) >= @fromTime and CONVERT(time,ThoiGianCan) < @toTime and SuDung = 1 and MaXuongSanXuat = @xuongId and TrongLuong>0";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new { ngay = dateTime.Date, fromTime, toTime, xuongId });
            return item;
        }
        public double GetSanLuong(DateTime dateTime, string xuongId, string banId)
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanNguyenLieu p where p.Ngay = @ngay and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and p.TrongLuong>0  and MaBanCatTiet =@banId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new { ngay = dateTime.Date, xuongId, banId });
            return item;
        }
        public double GetSanLuongTruNgop(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string banId,
            string sizeId)
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanNguyenLieu p, MaThanhPhamNguyenLieu tp where p.Ngay = @ngay and CONVERT(time,p.ThoiGianCan) >= @fromTime and CONVERT(time,p.ThoiGianCan) < @toTime and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and  p.MaBanCatTiet = @banId and p.MaSize = @sizeId and p.MaLoaiThanhPham = tp.Ma and tp.Min <> 2";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new
                {
                    ngay = dateTime.Date,
                    fromTime,
                    toTime,
                    xuongId,
                    banId,
                    sizeId
                });
            return item;
        }
        public double GetSanLuongCaNgopTruBan(DateTime dateTime, string xuongId, string sizeId)
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanNguyenLieu p, MaThanhPhamNguyenLieu tp where p.Ngay = @ngay and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and p.TrongLuong>0 and p.MaLoaiThanhPham = tp.Ma and tp.Min=2 and p.MaBanCatTiet in ('CT01','CT02','CT03','CT04','CT05','CT06','CT07','CT08') and MaSize =@sizeId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new { ngay = dateTime.Date, xuongId, sizeId });
            return item;
        }
        public List<string> GetMSLs(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string sizeId)
        {
            var query =
                "Select Distinct MSL from PhieuCanNguyenLieu  where CONVERT(time,ThoiGianCan) between @fromTime and @toTime and Ngay =@ngay and MaXuongSanXuat = @xuongId and MaSize = @sizeId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.Query<string>(
                    query,
                    new
                    {
                        ngay = dateTime.Date,
                        fromTime,
                        toTime,
                        xuongId,
                        sizeId
                    })
                .ToList();
            return item;
        }
        public double GetSanLuongCaNgop(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string banId)
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanNguyenLieu p, MaThanhPhamNguyenLieu tp where CONVERT(time,p.ThoiGianCan) >= @fromTime and CONVERT(time,p.ThoiGianCan) < @toTime and p.Ngay = @ngay and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and p.TrongLuong>0 and p.MaLoaiThanhPham = tp.Ma and tp.Min=2 and  MaBanCatTiet =@banId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new
                {
                    ngay = dateTime.Date,
                    xuongId,
                    fromTime,
                    toTime,
                    banId
                });
            return item;
        }


        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay],
    Cast(
        convert(
            varchar(19),
            p.ThoiGianCan,
            120
        ) as datetime
    ) as ThoiGian,
    p.[MaPhuongTien] ,
    p.[Chuyen],
    pt.Ten as TenPhuongTien ,
    ncc.[Ten] as TenNCC,
    p.[MSL] as MaLo,
    p.[MaAo] ,
    la.[Ten] as TenLoaiCa,
    tp.[Ten] as TenThanhPham,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    p.[TrongLuong] ,
    p.TrongLuongTare ,
    p.TyLeNuoc ,
    p.TrongLuongOrg,
    p.Pheu , 
    p.[MaBanCatTiet],
	bct.Ten as TenBanCatTiet,
    p.MaMayTinhCan, 
    p.[MaXuongSanXuat],
	x.Ten as TenXuong
FROM
    [PhieuCanNguyenLieu] p,
    [NhaCungCapNguyenLieu] ncc,
    [MaLoaiCaNguyenLieu] la,
    [MaThanhPhamNguyenLieu] tp,
    [MaSizeNguyenLieu] s,
    MaMauNguyenLieu mau,
    PhuongTienChoNguyenLieu pt,
	BanCatTiet bct,
	XiNghiep x
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.NhaCC = ncc.Ma
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaPhuongTien = pt.Ma
	and p.MaBanCatTiet = bct.Ma
	and p.MaXuongSanXuat = x.Ma
order by
    ThoiGianCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNCC<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay],
    p.[MaXuongSanXuat],
	x.Ten as TenXuong,
    ncc.[Ten] as TenNCC,
    p.[MSL] as MaLo,
    p.[MaAo] as MaAo,
    p.[MaPhuongTien],
	pt.Ten as TenPhuongTien,
    la.[Ten] as TenLoaiCa,
    tp.[Ten] as TenThanhPham,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong
FROM
    [PhieuCanNguyenLieu] p,
    [NhaCungCapNguyenLieu] ncc,
    [MaLoaiCaNguyenLieu] la,
    [MaThanhPhamNguyenLieu] tp,
    [MaSizeNguyenLieu] s,
    MaMauNguyenLieu mau,
	PhuongTienChoNguyenLieu pt,
	XiNghiep x
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.NhaCC = ncc.Ma
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaPhuongTien  = pt.Ma
	and p.MaXuongSanXuat = x.Ma
    
GROUP BY
    p.[Ngay],
    ncc.[Ten],
    p.[MSL],
    p.[MaAo],
    p.[MaPhuongTien],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat],
	pt.Ten,
	x.Ten
order by
    p.Ngay,
    ncc.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopBCT<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay],
p.[MaXuongSanXuat],
x.Ten as TenXuong,
    p.MaBanCatTiet,
	bct.Ten as TenBanCatTiet,
    p.[MSL] as MaLo,
    p.[MaAo],
    la.[Ten] as TenLoaiCa,
    tp.[Ten] as TenThanhPham,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
Cast(
        convert(
            varchar(19),
            Min(p.ThoiGianCan),
            120
        ) as datetime
    )  as BatDau,
Cast(
        convert(
            varchar(19),
            Max(p.ThoiGianCan),
            120
        ) as datetime
    ) as KetThuc
FROM
    [PhieuCanNguyenLieu] p,
    [MaLoaiCaNguyenLieu] la,
    [MaThanhPhamNguyenLieu] tp,
    [MaSizeNguyenLieu] s,
    MaMauNguyenLieu mau,
	XiNghiep x,
	BanCatTiet bct
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaXuongSanXuat = x.Ma
	and p.MaBanCatTiet = bct.Ma
GROUP BY
    p.[Ngay],
    p.[MSL],
    p.[MaAo],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.MaBanCatTiet,
	x.Ten,
	bct.Ten,
p.[MaXuongSanXuat]
order by
    p.Ngay,
    p.MaBanCatTiet";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopPhuongTien<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay],
	p.[MaXuongSanXuat],
	x.Ten as TenXuong,
    p.[MaPhuongTien],
	pt.Ten as TenPhuongTien,
    p.[MSL] as MaLo,
    p.[MaAo] as MaAo,
    la.[Ten] as TenLoaiCa,
    tp.[Ten] as TenThanhPham,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
Cast(
        convert(
            varchar(19),
            Min(p.ThoiGianCan),
            120
        ) as datetime
    )  as BatDau,
Cast(
        convert(
            varchar(19),
            Max(p.ThoiGianCan),
            120
        ) as datetime
    ) as KetThuc
FROM
    [PhieuCanNguyenLieu] p,
    [MaLoaiCaNguyenLieu] la,
    [MaThanhPhamNguyenLieu] tp,
    [MaSizeNguyenLieu] s,
    MaMauNguyenLieu mau,
	XiNghiep x,
	PhuongTienChoNguyenLieu pt
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
GROUP BY
    p.[Ngay],
    p.[MSL],
    p.[MaAo],
    p.[MaPhuongTien],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
	x.Ten,
	pt.Ten,
	p.[MaXuongSanXuat]
order by
    p.Ngay,
    p.MaPhuongTien";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsCaTra<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    p.Ngay ,
    CAST(p.ThoiGianCan as datetime) as ThoiGian,
    ncc.Ten as NhaCungCapName,
    p.MSL as MaLo,
    p.MaAo,
    p.MaPhuongTien,
    pt.Ten as PhuongTienName,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as MauName,
	p.MaBanCatTiet,
	b.Ten as BanCatTietName,
    p.TrongLuong
    
from
    PhieuCanNguyenLieu p,
    NhaCungCapNguyenLieu ncc,
    PhuongTienChoNguyenLieu pt,
    MaLoaiCaNguyenLieu la,
    MaThanhPhamNguyenLieu tp,
    MaSizeNguyenLieu s,
    MaMauNguyenLieu mau,
	BanCatTiet b
where
    p.Ngay = @ngay
    and p.MaXuongSanXuat = @xuongId
    and p.TrongLuong < 0
    and p.SuDung = 1
    and p.NhaCC = ncc.Ma
    and p.MaPhuongTien = pt.Ma
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaBanCatTiet = b.Ma";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result.ToList();
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
