namespace Services;

/// <summary>Short HMI text for the new HQ online flow only. Legacy messages stay unchanged.</summary>
public static class ScaleOnlineMessages
{
    public const string Unstable = "Cân chưa ổn định. Không lưu phiếu";
    public const string ZeroWeight = "Số kg bằng 0. Không lưu phiếu";
    public const string NegativeWeight = "Số kg âm. Không lưu phiếu";
    public const string InvalidWeightRange = "Số kg hoặc bì vượt giới hạn";
    public const string UnknownEmployee = "Thẻ chưa đăng ký nhân viên";
    public const string IdentitySendFailed = "Chưa gửi được tên. Quẹt lại";
    public const string SavedTotalsError = "Đã lưu phiếu. Chưa có tổng";
    public const string InvalidPayload = "JSON phiếu cân sai. Gửi lại";
    public const string ProcessingError = "Chưa rõ đã lưu. Báo quản lý";
    // These three texts already existed in CommitHqLite before this feature.
    public const string InvalidTime = "Thời gian phiếu cân không hợp lệ.";
    public const string InvalidId = "Thiếu hoặc sai Id phiếu cân.";
    public const string InvalidTare = "Trọng lượng tare không được âm.";
    public const string InvalidPrecision = "Ký/tare quá 3 số lẻ";
    public const string InvalidCard = "Thiếu/sai mã thẻ";
    public const string WeightConflict = "Trùng Id, khác dữ liệu";
    public const string OldWaiting = "Lượt chờ cũ. Quẹt lại thẻ";
    public const string Busy = "Cân bận. Quẹt lại";
    public const string DatabaseError = "Lỗi SQL. Báo quản lý";
    public const string MissingSnapshot = "Ca cũ thiếu mã. Quẹt Ra rồi Vào";
    public const string ScanConflict = "Trùng Id lượt quẹt";
    public const string OutOfOrder = "Sai thứ tự giờ. Kiểm tra cân";
    public const string MultipleShifts = "Trùng ca. Báo quản lý";
    public const string MissingCheckIn = "Chưa Vào ca. Quẹt Vào";
    public const string InvalidShift = "Ca lỗi. Báo quản lý";
    public const string CheckoutTime = "Giờ Ra sai. Kiểm tra giờ";
    public const string SwitchTime = "Chuyển ca sớm. Chờ 1 giây";
    public const string SwitchConflict = "Trùng lượt đổi ca. Quẹt lại";
    public const string InvalidConfiguration = "Sai cấu hình ca. Báo quản lý";
    public const string BeforeCheckIn = "Giờ cân trước Vào. Kiểm tra giờ";
    public const string MissingProduct = "Chưa chọn thành phẩm";
    public const string MissingMaterial = "Chưa chọn nguyên liệu";
    public const string MissingWorkshop = "Chưa rõ xưởng. Báo quản lý";
    public const string InvalidProduct = "Mã thành phẩm sai/tắt";
    public const string InvalidMaterial = "Mã nguyên liệu sai/tắt";
    public const string DifferentProduct = "Khác thành phẩm. Quẹt Vào";
    public const string DifferentMaterial = "Khác nguyên liệu. Quẹt Vào";
    public const string DifferentWorkshop = "Khác xưởng. Quẹt Vào";
    public const string DifferentContext = "Khác ca. Quẹt Vào";
    public const string ManualTransfer = "Cùng mã: cân tiếp. Đổi mã: quẹt Vào";

    public static string IncompleteContext(ScaleWorkContext selected) =>
        selected.Product.Length == 0 ? MissingProduct : selected.Product.Length > 50 ? InvalidProduct :
        selected.Material.Length == 0 ? MissingMaterial : selected.Material.Length > 50 ? InvalidMaterial : MissingWorkshop;

    public static string ContextMismatch(ScaleWorkContext current, ScaleWorkContext selected) =>
        !ScaleSharedAttendancePolicy.SameProduct(current.Product, selected.Product) ? DifferentProduct :
        !ScaleSharedAttendancePolicy.SameProduct(current.Material, selected.Material) ? DifferentMaterial :
        !ScaleSharedAttendancePolicy.SameProduct(current.Workshop, selected.Workshop) ? DifferentWorkshop : DifferentContext;
}
