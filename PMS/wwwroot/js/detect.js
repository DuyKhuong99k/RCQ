function onDevToolsOpen() {
    // Chuyển hướng trang
  // window.location.href= "/Error/StopAttack"

    // Xóa console và hiển thị cảnh báo
    setTimeout(() => {
        clearConsole();
        showWarningMessage();
    }, 1);
}

function redirectPage(url) {
    history.pushState({}, null, url);
}

function clearConsole() {
    console.clear();
}

function showWarningMessage() {
    const warningMessage = "Cảnh Báo: Sử dụng chức năng này mà không có sự cho phép là vi phạm bản quyền hệ thống và bảo hành thiết bị, mọi thao tác ở đây đều được PMS ghi nhận. Ngưng truy cập ngay!";
    const styles = `
        background-color: #f44336;
        color: #ffffff;
        font-family: 'Arial', sans-serif;
        font-size: 16px;
        font-weight: bold;
        padding: 15px 20px;
        border-radius: 10px;
        text-align: center;
        box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.5);
        animation: fadeIn 0.5s ease-out;
    `;
    console.log(`%c${warningMessage}`, styles);
}

class DevToolsChecker extends Error {
    toString() {
        onDevToolsOpen();
    }

    get message() {
        // Bạn có thể thêm hành động khác ở đây nếu cần
    }
}

console.log(new DevToolsChecker());