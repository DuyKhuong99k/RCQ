using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Dao
{
    public partial class PhieuCanKeToan
    {
        private readonly string connectionString;
        public PhieuCanKeToan(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
        public DataTable Gets(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = $@"declare @DynamicPivotQuery as nvarchar(Max);
declare @ColumnName as nvarchar(Max);
declare @PivotSelectColumnNames AS NVARCHAR(MAX);
IF OBJECT_ID('tempdb.dbo.#tempPhieuCan', 'U') IS NOT NULL
    DROP TABLE #tempPhieuCan;
IF OBJECT_ID('tempdb.dbo.#tempPhieuCan2', 'U') IS NOT NULL
    DROP TABLE #tempPhieuCan2;
IF OBJECT_ID('tempdb.dbo.#nhanvien', 'U') IS NOT NULL
    DROP TABLE #nhanvien;
Select * into #nhanvien from (SELECT EmployeeCode, DeptCode as DeptCode2,
	ROW_NUMBER() OVER (PARTITION BY EmployeeCode ORDER BY EffectiveDate DESC) Id
	 FROM B30HRMAppointed
	 WHERE DeptCode <> '' AND IsActive = 1 AND EffectiveDate <= @fromDate) nv where Id=1
Select * into #tempPhieuCan
from (
Select MaNhanVien,Ngay,CaLamViec,MaSanPham,TenSanPham,TrongLuongTra as TrongLuong,Case when DanhGia = 1 then N'Đậu' when DanhGia=0 then N'Rớt' else '' end as DanhGia,'DH' as Bang from PhieuCanDinhHinh  Where Ngay>=@fromDate and Ngay<= @toDate
UNION ALL
Select MaNhanVien,Ngay,CaLamViec,MaSanPham,pro.Name,TrongLuong,'' as DanhGia,'FL' as Bang from PhieuCanFillet p,B20SalaryProduct pro Where Ngay>=@fromDate and Ngay<= @toDate and p.MaSanPham = pro.Code
UNION ALL
Select MaNhanVien,Ngay,CaLamViec,MaSanPham,TenSanPham,TrongLuong,'' as DanhGia,'KDH' as Bang from PhieuCanKiemDinhHinh where Ngay>=@fromDate and Ngay<= @toDate
) p

Select * into #tempPhieuCan2
from (
Select Case when p.BranchCode = 'A02' then N'Đại Thành' when p.BranchCode = 'B02' then N'Đại Đại Thành' else '' end as Xuong ,p.CaLamViec,isnull( grp.Name,'') as Nhom,isnull(dept.Name,'') as BoPhan,p.MaSanPham,p.TenSanPham,p.DanhGia,p.Bang,p.Ngay,Sum(TrongLuong) as TrongLuong  from (
select p.BranchCode, p.MaNhanVien,isnull(nv.DeptCode2, p.DeptCode) as DeptCode,p.GroupCode,p.CaLamViec,p.MaSanPham,p.TenSanPham,p.TrongLuong,p.DanhGia,p.Bang,p.Ngay from ( select n.BranchCode, MaNhanVien, DeptCode,n.GroupCode,ca.Name as CaLamViec,MaSanPham,TenSanPham,TrongLuong,DanhGia,Bang,Ngay from #tempPhieuCan p,B20Employee n,B20HrmShift ca where p.MaNhanVien = n.Code and p.CaLamViec = ca.Code) p 
left join #nhanvien nv
on p.MaNhanVien = nv.EmployeeCode
) p
left join B20Dept dept
on	p.DeptCode = dept.Code
left join B20Group grp
on p.GroupCode = grp.Code
group by p.BranchCode, grp.Name ,dept.Name,p.MaSanPham,p.TenSanPham,p.DanhGia,p.Bang,p.Ngay,p.CaLamViec) p
Select @ColumnName = ISNULL(@ColumnName + ',','') +QUOTENAME(Ngay)
from (select Ngay from #tempPhieuCan2 Group by Ngay) as MS
SELECT @PivotSelectColumnNames 
    = ISNULL(@PivotSelectColumnNames + ',','')
    + 'ISNULL(' + QUOTENAME(Ngay) + ', 0) AS '
    + QUOTENAME(Ngay)
FROM (select Ngay from #tempPhieuCan2 Group by Ngay) AS Courses order by Ngay
set @DynamicPivotQuery = N'Select  p.Xuong As [Xưởng] ,p.BoPhan as [Bộ Phận],p.Nhom as [Nhóm],p.CaLamViec as [Ca Làm Việc] ,p.MaSanPham as [Mã Sản Phẩm],p.TenSanPham as [Tên Sản Phẩm],p.DanhGia as [Đánh Giá],t.TrongLuong as [Trọng Lượng], '+ @PivotSelectColumnNames  +',p.Bang as [TB] from #tempPhieuCan2 pivot (sum([TrongLuong]) for Ngay in ('+ @ColumnName + ')) as p
left join (Select Xuong ,Nhom,BoPhan,CaLamViec,MaSanPham,TenSanPham,DanhGia,Bang,Sum(TrongLuong) as TrongLuong from #tempPhieuCan2  group by Xuong ,Nhom,BoPhan,CaLamViec,MaSanPham,TenSanPham,DanhGia,Bang) t
on p.Xuong = t. Xuong and p.Nhom = t.Nhom and p.BoPhan = t.BoPhan and p.CaLamViec = t.CaLamViec and p.MaSanPham = t.MaSanPham and p.TenSanPham = t.TenSanPham and p.DanhGia = t.DanhGia and p.Bang =t.Bang 
order by p.Xuong,p.BoPhan,p.MaSanPham,p.Nhom,p.CaLamViec';
EXEC sp_executesql @DynamicPivotQuery
";
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);

                        connection.Open();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            var dataTable = new DataTable();
                            da.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetsNhanVien(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = $@"declare @DynamicPivotQuery as nvarchar(Max);

declare @ColumnName as nvarchar(Max);

declare @PivotSelectColumnNames AS NVARCHAR(MAX);

IF OBJECT_ID('tempdb.dbo.#tempPhieuCan', 'U') IS NOT NULL DROP TABLE #tempPhieuCan;
IF OBJECT_ID('tempdb.dbo.#tempPhieuCan2', 'U') IS NOT NULL DROP TABLE #tempPhieuCan2;
IF OBJECT_ID('tempdb.dbo.#nhanvien', 'U') IS NOT NULL DROP TABLE #nhanvien;
Select
    * into #nhanvien from (SELECT EmployeeCode, DeptCode as DeptCode2,
    ROW_NUMBER() OVER (
        PARTITION BY EmployeeCode
        ORDER BY
            EffectiveDate DESC
    ) Id
FROM
    B30HRMAppointed
WHERE
    DeptCode <> ''
    AND IsActive = 1
    AND EffectiveDate <= @fromDate
) nv
where
    Id = 1
Select
    * into #tempPhieuCan
from
    (
        Select
            MaNhanVien,
            Ngay,
            CaLamViec,
            MaSanPham,
            TenSanPham,
            TrongLuongTra as TrongLuong,
Case
                when DanhGia = 1 then N'Đậu'
                when DanhGia = 0 then N'Rớt'
                else ''
            end as DanhGia,
            'DH' as Bang
        from
            PhieuCanDinhHinh
        Where
            Ngay >= @fromDate
            and Ngay <= @toDate
        UNION
        ALL
        Select
            MaNhanVien,
            Ngay,
            CaLamViec,
            MaSanPham,
            pro.Name,
            TrongLuong,
            '' as DanhGia,
            'FL' as Bang
        from
            PhieuCanFillet p,
            B20SalaryProduct pro
        Where
            Ngay >= @fromDate
            and Ngay <= @toDate
            and p.MaSanPham = pro.Code
        UNION
        ALL
        Select
            MaNhanVien,
            Ngay,
            CaLamViec,
            MaSanPham,
            TenSanPham,
            TrongLuong,
            '' as DanhGia,
            'KDH' as Bang
        from
            PhieuCanKiemDinhHinh
        where
            Ngay >= @fromDate
            and Ngay <= @toDate
    ) p
Select
    * into #tempPhieuCan2
from
    (
        Select
            Case
                when p.BranchCode = 'A02' then N'Đại Thành'
                when p.BranchCode = 'B02' then N'Đại Đại Thành'
                else ''
            end as Xuong,
            p.MaHoSo,
            p.MaNhanVien,
            p.TenNhanVien,
            p.CaLamViec,
            isnull(grp.Name, '') as Nhom,
            isnull(dept.Name, '') as BoPhan,
            p.MaSanPham,
            p.TenSanPham,
            p.DanhGia,
            p.Bang,
            p.Ngay,
            Sum(TrongLuong) as TrongLuong
        from
            (
                select
                    p.BranchCode,
                    p.MaNhanVien,
                    p.MaHoSo,
                    p.TenNhanVien,
                    isnull(nv.DeptCode2, p.DeptCode) as DeptCode,
                    p.GroupCode,
                    p.CaLamViec,
                    p.MaSanPham,
                    p.TenSanPham,
                    p.TrongLuong,
                    p.DanhGia,
                    p.Bang,
                    p.Ngay
                from
                    (
                        select
                            n.BranchCode,
                            MaNhanVien,
                            n.FileNo as MaHoSo,
                            n.Name as TenNhanVien,
                            DeptCode,
                            n.GroupCode,
                            ca.Name as CaLamViec,
                            MaSanPham,
                            TenSanPham,
                            TrongLuong,
                            DanhGia,
                            Bang,
                            Ngay
                        from
                            #tempPhieuCan p,B20Employee n,B20HrmShift ca where p.MaNhanVien = n.Code and p.CaLamViec = ca.Code) p 
                            left join #nhanvien nv
                            on p.MaNhanVien = nv.EmployeeCode
                    ) p
                    left join B20Dept dept on p.DeptCode = dept.Code
                    left join B20Group grp on p.GroupCode = grp.Code
                group by
                    p.BranchCode,
                    grp.Name,
                    dept.Name,
                    p.MaSanPham,
                    p.TenSanPham,
                    p.DanhGia,
                    p.Bang,
                    p.Ngay,
                    p.CaLamViec,
                    p.MaHoSo,
                    p.MaNhanVien,
                    p.TenNhanVien
            ) p
        Select
            @ColumnName = ISNULL(@ColumnName + ',', '') + QUOTENAME(Ngay)
        from
            (
                select
                    Ngay
                from
                    #tempPhieuCan2 Group by Ngay) as MS
                SELECT
                    @PivotSelectColumnNames = ISNULL(@PivotSelectColumnNames + ',', '') + 'ISNULL(' + QUOTENAME(Ngay) + ', 0) AS ' + QUOTENAME(Ngay)
                FROM
                    (
                        select
                            Ngay
                        from
                            #tempPhieuCan2 Group by Ngay) AS Courses order by Ngay
                        set
                            @DynamicPivotQuery = N'Select  p.Xuong As [Xưởng] ,p.MaNhanVien as [Mã Nhân Viên],t.MaHoSo as [Mã Hồ Sơ],p.TenNhanVien as [Tên Nhân Viên],p.BoPhan as [Bộ Phận],p.Nhom as [Nhóm],p.CaLamViec as [Ca Làm Việc] ,p.MaSanPham as [Mã Sản Phẩm],p.TenSanPham as [Tên Sản Phẩm],p.DanhGia as [Đánh Giá],t.TrongLuong as [Trọng Lượng], ' + @PivotSelectColumnNames + ',p.Bang as [TB] from #tempPhieuCan2 pivot (sum([TrongLuong]) for Ngay in (' + @ColumnName + ')) as p
left join (Select Xuong ,MaNhanVien,MaHoSo,Nhom,BoPhan,CaLamViec,MaSanPham,TenSanPham,DanhGia,Bang,Sum(TrongLuong) as TrongLuong from #tempPhieuCan2  group by Xuong ,Nhom,BoPhan,CaLamViec,MaSanPham,TenSanPham,DanhGia,Bang,MaNhanVien,MaHoSo) t
on p.Xuong = t. Xuong and p.Nhom = t.Nhom and p.BoPhan = t.BoPhan and p.CaLamViec = t.CaLamViec and p.MaSanPham = t.MaSanPham and p.TenSanPham = t.TenSanPham and p.DanhGia = t.DanhGia and p.Bang =t.Bang and p.MaNhanVien =t.MaNhanVien
order by p.Xuong,p.BoPhan,p.MaSanPham,p.Nhom,p.CaLamViec,p.MaNhanVien';

EXEC sp_executesql @DynamicPivotQuery
";
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);

                        connection.Open();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            var dataTable = new DataTable();
                            da.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //        public DataTable Gets(DateTime fromDate, DateTime toDate)
        //        {
        //            try
        //            {
        //                var query = $@"declare @DynamicPivotQuery as nvarchar(Max);
        //declare @ColumnName as nvarchar(Max);
        //declare @PivotSelectColumnNames AS NVARCHAR(MAX);

        //DROP TABLE IF EXISTS tempdb.dbo.#tempPhieuCan
        //DROP TABLE IF EXISTS tempdb.dbo.#tempPhieuCan2

        //Select * into #tempPhieuCan
        //from (
        //Select MaNhanVien,Ngay,CaLamViec,MaSanPham,TenSanPham,TrongLuongTra as TrongLuong,Case when DanhGia = 1 then N'Đậu' when DanhGia=0 then N'Rớt' else '' end as DanhGia,'DH' as Bang from PhieuCanDinhHinh  Where Ngay>=@fromDate and Ngay<= @toDate
        //UNION ALL
        //Select MaNhanVien,Ngay,CaLamViec,MaSanPham,pro.Name,TrongLuong,'' as DanhGia,'FL' as Bang from PhieuCanFillet p,B20SalaryProduct pro Where Ngay>=@fromDate and Ngay<= @toDate and p.MaSanPham = pro.Code
        //UNION ALL
        //Select MaNhanVien,Ngay,CaLamViec,MaSanPham,TenSanPham,TrongLuong,'' as DanhGia,'KDH' as Bang from PhieuCanKiemDinhHinh where Ngay>=@fromDate and Ngay<= @toDate
        //) p
        //Select * into #tempPhieuCan2
        //from (
        //Select Case when p.BranchCode = 'A02' then N'Đại Thành' when p.BranchCode = 'B02' then N'Đại Đại Thành' else '' end as Xuong ,p.CaLamViec,isnull( grp.Name,'') as Nhom,isnull(dept.Name,'') as BoPhan,p.MaSanPham,p.TenSanPham,p.DanhGia,p.Bang,p.Ngay,Sum(TrongLuong) as TrongLuong  from (
        //select n.BranchCode, MaNhanVien,n.DeptCode,n.GroupCode,ca.Name as CaLamViec,MaSanPham,TenSanPham,TrongLuong,DanhGia,Bang,Ngay from #tempPhieuCan p,B20Employee n,B20HrmShift ca where p.MaNhanVien = n.Code and p.CaLamViec = ca.Code) p
        //left join B20Dept dept
        //on	p.DeptCode = dept.Code
        //left join B20Group grp
        //on p.GroupCode = grp.Code
        //group by p.BranchCode, grp.Name ,dept.Name,p.MaSanPham,p.TenSanPham,p.DanhGia,p.Bang,p.Ngay,p.CaLamViec) p
        //Select @ColumnName = ISNULL(@ColumnName + ',','') +QUOTENAME(Ngay)
        //from (select Ngay from #tempPhieuCan2 Group by Ngay) as MS
        //SELECT @PivotSelectColumnNames 
        //    = ISNULL(@PivotSelectColumnNames + ',','')
        //    + 'ISNULL(' + QUOTENAME(Ngay) + ', 0) AS '
        //    + QUOTENAME(Ngay)
        //FROM (select Ngay from #tempPhieuCan2 Group by Ngay) AS Courses order by Ngay
        //set @DynamicPivotQuery = N'Select  p.Xuong As [Xưởng] ,p.BoPhan as [Bộ Phận],p.Nhom as [Nhóm],p.CaLamViec as [Ca Làm Việc] ,p.MaSanPham as [Mã Sản Phẩm],p.TenSanPham as [Tên Sản Phẩm],p.DanhGia as [Đánh Giá],t.TrongLuong as [Trọng Lượng], '+ @PivotSelectColumnNames  +',p.Bang as [TB] from #tempPhieuCan2 pivot (sum([TrongLuong]) for Ngay in ('+ @ColumnName + ')) as p
        //left join (Select Xuong ,Nhom,BoPhan,CaLamViec,MaSanPham,TenSanPham,DanhGia,Bang,Sum(TrongLuong) as TrongLuong from #tempPhieuCan2  group by Xuong ,Nhom,BoPhan,CaLamViec,MaSanPham,TenSanPham,DanhGia,Bang) t
        //on p.Xuong = t. Xuong and p.Nhom = t.Nhom and p.BoPhan = t.BoPhan and p.CaLamViec = t.CaLamViec and p.MaSanPham = t.MaSanPham and p.TenSanPham = t.TenSanPham and p.DanhGia = t.DanhGia and p.Bang =t.Bang 
        //order by p.Xuong,p.BoPhan,p.MaSanPham,p.Nhom,p.CaLamViec';
        //EXEC sp_executesql @DynamicPivotQuery
        //";
        //                using(var connection = new SqlConnection(ConnectionString))
        //                {
        //                    using(var cmd = new SqlCommand(query, connection))
        //                    {
        //                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
        //                        cmd.Parameters.AddWithValue("@toDate", toDate);

        //                        connection.Open();
        //                        using(var da = new SqlDataAdapter(cmd))
        //                        {
        //                            var dataTable = new DataTable();
        //                            da.Fill(dataTable);
        //                            return dataTable;
        //                        }
        //                    }
        //                }
        //            } catch(Exception)
        //            {
        //                throw;
        //            }
        //        }
    }
}
