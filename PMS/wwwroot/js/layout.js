
// Lấy element navbar
var navbar = document.getElementById("layout-navbar");

// Lấy vị trí ban đầu của navbar và chiều cao của nó
var navbarOffset = navbar.offsetTop;
var navbarHeight = navbar.clientHeight;

// Thêm sự kiện cuộn trang
window.onscroll = function () {
    // Nếu màn hình có chiều rộng lớn hơn 768px
    if (window.innerWidth > 768) {
        // Ngược lại, xóa lớp fixed-navbar và đặt lại chiều cao phần nội dung
        navbar.classList.remove("fixed-navbar");
        document.body.style.paddingTop = "0";
        return; // Không cần tiếp tục xử lý nếu không phải là điện thoại
    }

    // Nếu vị trí cuộn lớn hơn hoặc bằng vị trí ban đầu của navbar
    if (window.pageYOffset >= navbarOffset) {
        // Thêm lớp fixed-navbar để giữ vị trí
        navbar.classList.add("fixed-navbar");
        // Đặt chiều cao của phần nội dung để tránh "nhảy"
        document.body.style.paddingTop = navbarHeight + "px";
    } else {
        // Ngược lại, xóa lớp fixed-navbar và đặt lại chiều cao phần nội dung
        navbar.classList.remove("fixed-navbar");
        document.body.style.paddingTop = "0";
    }
};

function renewToken() {
    $.ajax({
        type: "POST",
        url: "/Authentication/RenewToken", // Đường dẫn đến action trong controller
        success: function (response) {
            if (response.Success === true) {
                // toastOptionNotyfi();
                // toastr["success"](response.Messages, "Thông báo Test");
            }
            else {
                // toastOptionNotyfi();
                // toastr["error"](response.Messages, "Thông báo Test");
            }
        },
        error: function () {
            toastOptionNotyfi();
            toastr["error"]("Có lỗi xảy ra khi gia hạn đăng nhập!", "Thông báo");
        }
    });
}
document.addEventListener("click", async function () {
    // var expiresTokenString = "@Context.Session.GetString("ExpiresJWTToken")";
    // var expiresTokenDate = new Date(expiresTokenString);
    var expiresTokenInSeconds = @Context.Session.GetString("ExpiresSec"); // Math.floor(expiresTokenDate.getTime() / 1000);
    var currentTime = new Date();
    var currentTimeUTC = new Date(currentTime.getUTCFullYear(), currentTime.getUTCMonth(), currentTime.getUTCDate(), currentTime.getUTCHours(), currentTime.getUTCMinutes(), currentTime.getUTCSeconds());
    var currentTimeInSeconds = Math.floor(currentTimeUTC.getTime() / 1000);
    var timeRemaining = expiresTokenInSeconds - currentTimeInSeconds;
    if (timeRemaining <= 0) {
        bootbox.alert({
            title: "Phiên đăng nhập đã hết hạn!",
            message: "Bạn cần đăng nhập lại.",
            callback: function () {
                performLogout();
            }
        });
    } else if (timeRemaining <= 300) {
        await renewToken();
        // Sau khi renewToken hoàn thành, timeRemaining sẽ được cập nhật. Kiểm tra lại thời gian còn lại trước khi gọi renewToken lần nữa
        if (timeRemaining > 0 && timeRemaining <= 300) {
            await renewToken();
        }
    }


});


// Logout
document.getElementById('logoutLink').addEventListener('click', function () {
    bootbox.confirm({
        title: "Thông báo!",
        message: "Bạn có chắc muốn thoát.",
        buttons: {
            confirm: {
                label: 'OK',
                className: 'btn btn-primary'
            }
        },
        callback: function (result) {
            if (result) {
                //Người dùng đã bấm OK, Gọi API logout ở đây
                performLogout();
            }
        }
    });

});
//hàm Logout
function performLogout() {
    $.ajax({
        type: "POST",
        url: "/Authentication/Logout",
        success: function (response) {
            if (response.Success === true) { // Sử dụng response.Success thay vì response.isSuccess
                // Thông báo đăng xuất thành công và sau đó chuyển trang
                toastOptionNotyfi();
                toastr["success"](response.Messages, "Thông báo");
                window.location = location.origin + response.url;//.host = response.url;
            } else {
                debugger
                // Thông báo lỗi đăng xuất
                toastOptionNotyfi();
                toastr["error"](response.Messages, "Thông báo");
                setTimeout(function () {
                    window.location = location.origin + response.url;
                }, 5000);
            }
        },
        error: function () {
            // Xử lý lỗi AJAX
            toastOptionNotyfi();
            toastr["error"]("Lỗi khi Logout", "Thông báo");

        }
    });
}

function logoutThoatTest() {
    $.ajax({
        type: "POST",
        url: "/Authentication/LogoutTest",
        success: function (response) {
            if (response.Success === true) { // Sử dụng response.Success thay vì response.isSuccess
                // Thông báo đăng xuất thành công và sau đó chuyển trang
                toastOptionNotyfi();
                toastr["success"](response.Messages, "Thông báo");
                window.location = location.origin + response.url;//.host = response.url;
            } else {
                debugger
                // Thông báo lỗi đăng xuất
                toastOptionNotyfi();
                toastr["error"](response.Messages, "Thông báo");
                setTimeout(function () {
                    window.location = location.origin + response.url;
                }, 5000);
            }
        },
        error: function () {
            // Xử lý lỗi AJAX
            toastOptionNotyfi();
            toastr["error"]("Lỗi khi Logout", "Thông báo");

        }
    });
}

// show info
document.getElementById('infoLink').addEventListener('click', function () {
    var id = $('#abc').val();
    $.ajax({
        url: '/Authentication/GetInfoUserById',
        data: { id: id },
        type: 'GET',
        dataType: 'json',
        success: function (rs) {
            if (rs.isSuccess === true) {

                $('#usernameInfoLayout').val(rs.UserName);
                $('#avatarImgInfoLayout').val(rs.AvatarImg);
                $('#hoTenInfoLayout').val(rs.HoTen);
                $('#emailInfoLayout').val(rs.Email);
                $('#maNhanVienInfoLayout').val(rs.MaNhanVien);
                $('#phoneNumberInfoLayout').val(rs.PhoneNumber);
                $('#addressInfoLayout').val(rs.Address);
                $('#cardIDInfoLayout').val(rs.CardID);
                $('#isActiveInfoLayout').prop('checked', rs.IsActive);
                //$('#statusInfo').val(rs.Status);
                $('#userNameInfoLayout').val(rs.UserName);
                $('#passwordInfoLayout').val(rs.Password);
                $('#modalInfoLayout').modal('show');
            } else {
                toastOptionNotyfi();
                toastr["warning"]('Không tìm thấy thông tin!', "Thông báo");
            }
        },
        error: function (error) {
            toastOptionNotyfi();
            toastr["error"]('Đã xảy ra lỗi: ' + error, "Thông báo");
        }
    });
});
//update info user
function updateInfoUserLayout() {
    var id = $('#abc').val();
    var avatarImg = $('#avatarImgInfoLayout').val();
    var hoten = $('#hoTenInfoLayout').val();
    var email = $('#emailInfoLayout').val();
    var maNhanVien = $('#maNhanVienInfoLayout').val();
    var phoneNumber = $('#phoneNumberInfoLayout').val();
    var address = $('#addressInfoLayout').val();
    var cardID = $('#cardIDInfoLayout').val();
    var isActive = $('#isActiveInfoLayout').prop('checked');
    //var status = $('#statusInfo').val();
    var userName = $('#userNameInfoLayout').val();
    var password = $('#passwordInfoLayout').val();
    $.ajax({
        url: '/Authentication/DoUpdate',
        data: { id: id, avatarImg: avatarImg, hoten: hoten, email: email, maNhanVien: maNhanVien, phoneNumber: phoneNumber, address: address, cardID: cardID, isActive: isActive, userName: userName, password: password },
        type: 'GET',
        dataType: 'json',
        success: function (rs) {
            if (rs.isSuccess === true) {
                //$('#modalInfoLayout').modal('hide');
                reloadData();
                toastOptionNotyfi();
                toastr["success"](rs.Messages, "Thông báo");
            } else {
                toastOptionNotyfi();
                toastr["warning"](rs.Messages, "Thông báo");
            }
        },
        error: function (error) {
            toastOptionNotyfi();
            toastr["error"]('Đã xảy ra lỗi: ' + error, "Thông báo");
        }
    });
}