using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using ExcelDataReader;
using Dapper;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Text;

class Program
{
    static void Main()
    {
        string excelFilePath = "FileExcel/DanhSachCongNhanRauQuaRV.xlsx";
        string connectionString = "data source=.;initial catalog=PMS_HQ;user id=pmsvn;password=Sql@123456789;MultipleActiveResultSets=True;App=EntityFramework;TrustServerCertificate=True";

        if (!File.Exists(excelFilePath))
        {
            Console.WriteLine("Khong tim thay file Excel.");
            return;
        }

        Console.WriteLine("Đa tim thay File can import, Ban co muon bat đau import du lieu? (Y/N): ");
        string userInput = Console.ReadLine().Trim().ToUpper();
        if (userInput != "Y")
        {
            Console.WriteLine("Huy thao tac import.");
            return;
        }

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        int successCount = 0;
        int errorCount = 0;
        List<string> errorList = new List<string>();

        using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });

            DataTable table = dataSet.Tables[0];

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                try
                                {
                                    string maNhanVien = row["MaNhanVien"].ToString();
                                    if (string.IsNullOrEmpty(maNhanVien))
                                    {
                                        errorList.Add("Bo qua dong khong co MaNhanVien.");
                                        errorCount++;
                                        continue;
                                    }

                                    var existing = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM NhanVien WHERE cast( Id as NVARCHAR(MAX)) = @MaNhanVien", new { MaNhanVien = maNhanVien }, transaction);
                                    if (existing > 0)
                                    {
                                        errorList.Add($"Bo qua MaNhanVien {maNhanVien} vi đa ton tai.");
                                        errorCount++;
                                        continue;
                                    }

                                    var nhanVien = new
                                    {
                                        Id = maNhanVien,
                                        MaSo = row["MaHoSo"].ToString(),
                                        Ten = row["Name"].ToString(),
                                        IsPhucVu = 0,
                                        IsBanKiem = 0,
                                        NgayGioTao = DateTime.Now.ToString("yyyyMMddHHmmss")

                                    };
                                    //var tenUnicode = );

                                    //string sql = @"INSERT INTO NhanVienDaiThanh (MaNhanVien,MaHoSo,Xuong,Name,DeptName0, IsShowDinhMuc, IsContracting, IsPhucVu, IsHuman, IsGiaCong, AC, IsChucNang, LoaiSanLuong, IsBanKiem, IsNhanVienCat, IsNhom, MNgay) 
                                    //                   VALUES (@MaNhanVien,@MaHoSo,@Xuong,@Name,@DeptName0, @IsShowDinhMuc, @IsContracting, @IsPhucVu, @IsHuman, @IsGiaCong, @AC, @IsChucNang, @LoaiSanLuong, @IsBanKiem, @IsNhanVienCat, @IsNhom, @MNgay)";

                                    string sql = @"INSERT INTO NhanVien (Id,MaSo,Ten,IsPhucVu,IsBanKiem, NgayGioTao) 
                                                       VALUES (@Id,@MaSo,@Ten,@IsPhucVu,@IsBanKiem, @NgayGioTao)";

                                    connection.Execute(sql, nhanVien, transaction);
                                    //                                    using (var cmd = new SqlCommand(@"INSERT INTO NhanVien (Id, MaSo, Ten, IsPhucVu, IsBanKiem, NgayGioTao) 
                                    //                                  VALUES (@Id, @MaSo, @Ten, @IsPhucVu, @IsBanKiem, @NgayGioTao)", connection, transaction))
                                    //{
                                    //    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Text) { Value = nhanVien.Id });
                                    //    cmd.Parameters.Add(new SqlParameter("@MaSo", SqlDbType.Text) { Value = nhanVien.MaSo });
                                    //    cmd.Parameters.Add(new SqlParameter("@Ten", SqlDbType.Text) { Value = nhanVien.Ten ?? (object)DBNull.Value });
                                    //    cmd.Parameters.Add(new SqlParameter("@IsPhucVu", SqlDbType.Int) { Value = nhanVien.IsPhucVu });
                                    //    cmd.Parameters.Add(new SqlParameter("@IsBanKiem", SqlDbType.Int) { Value = nhanVien.IsBanKiem });
                                    //    cmd.Parameters.Add(new SqlParameter("@NgayGioTao", SqlDbType.Text) { Value = nhanVien.NgayGioTao });

                                    //    cmd.ExecuteNonQuery();
                                    //}
                                    // Chuyển đổi chuỗi Ten sang UTF-8
                                    //                     var tenUnicode = Encoding.UTF8.GetString(Encoding.Default.GetBytes(nhanVien.Ten));

                                    //                     string sql = @"INSERT INTO NhanVien (Id, MaSo, Ten, IsPhucVu, IsBanKiem, NgayGioTao) 
                                    //VALUES (@Id, @MaSo, @Ten, @IsPhucVu, @IsBanKiem, @NgayGioTao)";

                                    //                     // Đảm bảo tham số được truyền chính xác
                                    //                     connection.Execute(sql, new
                                    //                     {
                                    //                         Id = nhanVien.Id,
                                    //                         MaSo = nhanVien.MaSo,
                                    //                         Ten = tenUnicode,  // Truyền chuỗi đã chuyển đổi sang Unicode
                                    //                         IsPhucVu = nhanVien.IsPhucVu,
                                    //                         IsBanKiem = nhanVien.IsBanKiem,
                                    //                         NgayGioTao = nhanVien.NgayGioTao
                                    //                     }, transaction);

                                    successCount++;
                                }
                                catch (Exception ex)
                                {
                                    errorList.Add($"Loi khi xu ly dong {row["MaNhanVien"]}: {ex.Message}");
                                    errorCount++;
                                }
                            }

                            transaction.Commit();
                            Console.WriteLine("Import hoan tat!");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("Loi xay ra trong qua trinh nhap du lieu: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Khong the ket noi đen co so du lieu: " + ex.Message);
            }
        }

        // Hiển thị kết quả tổng kết
        Console.WriteLine("\n==================== KET QUA IMPORT ====================");
        Console.WriteLine($"* So dong them thanh cong: {successCount}");
        Console.WriteLine($"* So dong khong them duoc: {errorCount}");
        if (errorList.Count > 0)
        {
            Console.WriteLine("\n* Chi tiet loi:");
            foreach (var error in errorList)
            {
                Console.WriteLine($"   - {error}");
            }
        }
        Console.WriteLine("\n=========================================================");
        // Giữ cửa sổ console mở
        Console.WriteLine("Nhấn Enter để thoát...");
        Console.ReadLine();
    }
}
