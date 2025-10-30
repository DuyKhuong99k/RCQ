function layTenMayVaThoiGian() {
    // Lấy tên máy tính từ user agent hoặc thay thế bằng một giá trị mặc định
    const tenMay = "PMS" || "UnknownMachine";

    // Lấy thời gian hiện tại với độ phân giải mili giây
    const ngayHienTai = new Date();
    const nam = ngayHienTai.getFullYear();
    const thang = ("0" + (ngayHienTai.getMonth() + 1)).slice(-2); // Tháng từ 0-11 nên phải cộng thêm 1
    const ngay = ("0" + ngayHienTai.getDate()).slice(-2);
    const gio = ("0" + ngayHienTai.getHours()).slice(-2);
    const phut = ("0" + ngayHienTai.getMinutes()).slice(-2);
    const giay = ("0" + ngayHienTai.getSeconds()).slice(-2);
    const miliGiay = ("00" + ngayHienTai.getMilliseconds()).slice(-3); // Mili giây có thể từ 0-999

    // Tạo ID duy nhất bằng cách kết hợp tên máy tính và thời gian đến mili giây
    return tenMay.replace(/\s+/g, '') + nam + thang + ngay + gio + phut + giay + miliGiay;
}
function AddUserWaitView(xuongId) {
    const id = layTenMayVaThoiGian(); //LIN20240918
    //const fromDate = "@DateTime.Now.Date.AddDays(int.Parse(NumberDate ?? "0") * -1).ToString("yyyy-MM-dd")";
    const thoiGian =  new Date().toISOString();
    $.ajax({
        type: 'POST',
        url: '/Dashboard/AddUserWaitView',
        data: {
            id: id,
            thoiGian: thoiGian,
            xuongId: xuongId
        },
        success: function (rs) {
            //if (rs.isSuccess === true) {
            //    toastOptionNotyfi();
            //    toastr["success"](rs.Messages, "Thông báo");
            //} else {
            //    toastOptionNotyfi();
            //    toastr["warning"](rs.Messages, "Thông báo");
            //}
        },
        error: function (error) {
            toastOptionNotyfi();
            toastr["error"]('Đã xảy ra lỗi: ' + error, "Thông báo");
        }
    });
}