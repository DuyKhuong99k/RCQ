using ClosedXML.Excel;
using Models.Repos.Models;
using ViewModels.Repos.Staff;
using PMSHub.EmployeeImport;

internal static class HubEmployeeExcelChecks
{
    public static void Run(Action<bool, string> check, string? samplePath = null)
    {
        NhanVienImportRow Row(string code, string? file = null, string name = "Nhân viên") => new()
        {
            RowNumber = 2, MaNhanVien = code, MaHoSo = file ?? code, Name = name
        };
        List<NhanVienImportRow> ReadFixture(Action<IXLWorksheet> configure)
        {
            using var book = new XLWorkbook();
            var sheet = book.AddWorksheet("NhanVien");
            sheet.Cell(1, 1).Value = "Mã nhân viên";
            sheet.Cell(1, 2).Value = "Mã hồ sơ";
            sheet.Cell(1, 3).Value = "Tên nhân viên";
            configure(sheet);
            using var stream = new MemoryStream();
            book.SaveAs(stream);
            stream.Position = 0;
            return HubEmployeeExcelReader.Read(stream);
        }
        void RejectFixture(Action<IXLWorksheet> configure, string name)
        {
            try { ReadFixture(configure); }
            catch (InvalidDataException) { check(true, name); return; }
            throw new Exception("FAILED: " + name);
        }
        
        var rows = ReadFixture(s =>
        {
            s.Cell(2, 1).Value = 123;
            s.Cell(2, 1).Style.NumberFormat.Format = "00000";
            s.Cell(2, 2).Value = "00123";
            s.Cell(2, 3).Value = " Nguyễn Văn A ";
            s.Cell(4, 1).Value = "T2";
            s.Cell(4, 2).Value = "T2";
            s.Cell(4, 3).Value = "B";
            s.Cell(8000, 1).Style.Fill.BackgroundColor = XLColor.Red;
        });
        check(rows.Count == 2 && rows[1].RowNumber == 4, "Blank/formatting rows ignored, Excel line retained");
        check(rows[0].MaNhanVien == "00123" && rows[0].MaHoSo == "00123" && rows[0].Name == "Nguyễn Văn A", "Leading zeros, Vietnamese and trimming preserved");
        rows = ReadFixture(s =>
        {
            s.Cell(1, 1).Value = " TEN NHAN VIEN ";
            s.Cell(1, 2).Value = "MA NHAN VIEN";
            s.Cell(1, 3).Value = "MA HO SO";
            s.Cell(2, 1).Value = "An"; s.Cell(2, 2).Value = "T1"; s.Cell(2, 3).Value = "HS1";
        });
        check(rows[0].MaNhanVien == "T1" && rows[0].Name == "An", "Headers match by name, case and accents; column order independent");
        RejectFixture(s => s.Cell(1, 2).Value = "Sai cột", "Missing header rejected");
        RejectFixture(s => s.Cell(1, 4).Value = "Mã nhân viên", "Duplicate header rejected");
        RejectFixture(s => { }, "Empty list rejected");
        RejectFixture(s => { s.Cell(2, 1).FormulaA1 = "1+1"; s.Cell(2, 2).Value = "2"; s.Cell(2, 3).Value = "A"; }, "Formula rejected");
        RejectFixture(s => { for (var i = 2; i <= 1002; i++) s.Cell(i, 1).Value = "T" + i; }, "More than 1000 rows rejected");
        
        var validation = NhanVienImport.Validate(new[] { Row(" t1 "), Row("T1") }, Array.Empty<NhanVienDaiThanh>());
        check(!validation.IsSuccess && validation.ErrorCount == 2, "Case insensitive duplicate employee codes rejected");
        validation = NhanVienImport.Validate(new[] { Row("T1", "HS"), Row("T2", "hs") }, Array.Empty<NhanVienDaiThanh>());
        check(validation.ErrorCount == 2, "Duplicate file codes rejected");
        validation = NhanVienImport.Validate(new[] { Row("T1", "", ""), Row(new string('X', 51)), Row("T3", name: new string('X', 101)) }, Array.Empty<NhanVienDaiThanh>());
        check(validation.ErrorCount == 3, "Missing fields and model length limits checked");
        validation = NhanVienImport.Validate(new[] { Row("T1") }, new[] { new NhanVienDaiThanh { MaNhanVien = "t1", MaHoSo = "old" } });
        check(validation.IsSuccess && validation.SkippedCount == 1, "Existing employee skipped without overwriting");
        validation = NhanVienImport.Validate(new[] { Row("T1", "HS1") }, new[] { new NhanVienDaiThanh { MaNhanVien = "OTHER", MaHoSo = "HS1" } });
        check(!validation.IsSuccess && validation.ErrorCount == 1, "File code assigned to another employee rejected");
        var employee = NhanVienImport.CreateEmployee(Row("T1"), "2");
        check(employee.Xuong == "2" && employee.IsContracting && employee.IsHuman && employee.IsShowDinhMuc
            && employee.LoaiSanLuong == 1 && employee.MaChamCong == null && employee.DeptName0 == null
            && !employee.IsPhucVu && !employee.IsGiaCong && !employee.IsBanKiem && !employee.IsChucNang && !employee.IsNhom,
            "Import defaults match add-employee defaults");
        
        
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        using var template = File.OpenRead(Path.Combine(root, "PMSHub/wwwroot/templates/NhanVienImportTemplate.xlsx"));
        using var templateBook = new XLWorkbook(template);
        var templateSheet = templateBook.Worksheet(1);
        check(templateSheet.Cell(1, 1).GetString() == "Mã nhân viên" &&
              templateSheet.Cell(1, 2).GetString() == "Mã hồ sơ" &&
              templateSheet.Cell(1, 3).GetString() == "Tên nhân viên" &&
              templateSheet.RowsUsed(XLCellsUsedOptions.Contents).Count() == 1,
            "Hub: mẫu trống đúng 3 tiêu đề, không có nhân viên giả để nhập nhầm");
        check(templateSheet.Cell(1, 1).Style.Font.FontName == "Times New Roman" &&
              templateSheet.Cell(1, 1).Style.Font.FontSize == 14 && templateSheet.Cell(1, 1).Style.Font.Bold &&
              templateSheet.Cell(1, 1).Style.Alignment.Horizontal == XLAlignmentHorizontalValues.Center,
            "Hub: tiêu đề mẫu giống file tham chiếu, Times New Roman 14 in đậm căn giữa");
        check(templateSheet.Cell(2, 1).Style.Border.TopBorder == XLBorderStyleValues.Thin &&
              templateSheet.Cell(30, 3).Style.Border.BottomBorder == XLBorderStyleValues.Thin,
            "Hub: mẫu có khung bảng tới dòng 30 để người dùng dễ hình dung");
        check(templateSheet.Cell(2, 1).Style.NumberFormat.Format == "@" &&
              templateSheet.Cell(1001, 2).Style.NumberFormat.Format == "@",
            "Hub: mã dạng Text cho đủ 1000 dòng để giữ số 0 ở đầu");
        templateSheet.Cell(2, 1).Value = "0000123";
        templateSheet.Cell(2, 2).Value = "0000123";
        templateSheet.Cell(2, 3).Value = "Nhân viên thử";
        using var filledTemplate = new MemoryStream();
        templateBook.SaveAs(filledTemplate);
        filledTemplate.Position = 0;
        var templateRows = HubEmployeeExcelReader.Read(filledTemplate);
        check(templateRows.Single().MaNhanVien == "0000123", "Hub: mẫu điền dữ liệu đọc được và giữ số 0 đầu");
        using var oversizedArchive = new MemoryStream();
        using (var zip = new System.IO.Compression.ZipArchive(oversizedArchive, System.IO.Compression.ZipArchiveMode.Create, true))
            for (var i = 0; i <= 1000; i++) zip.CreateEntry("entry" + i);
        oversizedArchive.Position = 0;
        try { HubEmployeeExcelReader.Read(oversizedArchive); throw new Exception("Archive limit was not enforced"); }
        catch (InvalidDataException error)
        { check(error.Message.Contains("quá lớn sau giải nén"), "Hub: chặn archive quá nhiều thành phần trước khi phân tích workbook"); }
        if (samplePath != null)
        {
            using var sample = File.OpenRead(samplePath);
            var sampleRows = HubEmployeeExcelReader.Read(sample);
            var sampleResult = NhanVienImport.Validate(sampleRows, Array.Empty<NhanVienDaiThanh>());
            check(sampleRows.Count == 32 && sampleResult.IsSuccess && sampleResult.ReadyCount == 32,
                "Hub: file mẫu 32 nhân viên trong chat Dev_Khuong đọc và kiểm tra hợp lệ; không lưu SQL");
        }
    }
}

