//LẤY THỜI GIAN HIỆN TẠI
function getCurrentTime() {
    let now = new Date();

    // Lấy các thành phần giờ, phút, giây và mili giây
    let hours = String(now.getHours()).padStart(2, '0');
    let minutes = String(now.getMinutes()).padStart(2, '0');
    let seconds = String(now.getSeconds()).padStart(2, '0');
    let milliseconds = String(now.getMilliseconds()).padStart(7, '0');

    // Định dạng thời gian theo yêu cầu
    let currentTime = `${hours}:${minutes}:${seconds}.${milliseconds}`;

    return currentTime;
}

//ADD THỜI GIAN VÀO INPUT
function setFormattedTime(timeString) {
    // Chuyển đổi chuỗi thời gian sang đối tượng Date
    var time = new Date("1970-01-01T" + timeString);

    // Lấy giờ và phút
    var hours = time.getHours().toString().padStart(2, '0');
    var minutes = time.getMinutes().toString().padStart(2, '0');
    var seconds = time.getSeconds().toString().padStart(2, '0');

    // Định dạng lại chuỗi thời gian
    var formattedTime = hours + ":" + minutes;

    return formattedTime;
}
//LẤY NGAY HIỆN TẠI
function getCurrentDate() {
    let now = new Date();

    // Lấy các thành phần ngày, tháng, năm
    let year = now.getFullYear();
    let month = String(now.getMonth() + 1).padStart(2, '0'); // Tháng bắt đầu từ 0
    let day = String(now.getDate()).padStart(2, '0');

    // Định dạng ngày theo yêu cầu
    let currentDate = `${year}-${month}-${day}`;

    return currentDate;
}
function setDateInput(datestring) {
    // Chuyển đổi datestring thành định dạng yyyy-mm-dd
    let date = new Date(datestring);
    let year = date.getFullYear();
    let month = ("0" + (date.getMonth() + 1)).slice(-2); // Tháng bắt đầu từ 0 nên cần +1
    let day = ("0" + date.getDate()).slice(-2);

    // Tạo chuỗi ngày theo định dạng yyyy-mm-dd
    let formattedDate = `${year}-${month}-${day}`;

    // Gán giá trị này cho input
    return formattedDate;
}
// lấy ngày giờ hiện tại
function getCurrentDateTime() {
    let now = new Date();

    // Lấy các thành phần giờ, phút, giây và mili giây
    let year = now.getFullYear();
    let month = String(now.getMonth() + 1).padStart(2, '0'); // Tháng bắt đầu từ 0
    let day = String(now.getDate()).padStart(2, '0');
    let hours = String(now.getHours()).padStart(2, '0');
    let minutes = String(now.getMinutes()).padStart(2, '0');
    let seconds = String(now.getSeconds()).padStart(2, '0');

    // Định dạng thời gian theo yêu cầu của input datetime-local
    let currentTime = `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;

    return currentTime;
}
function setDateTimeInput() {
    let dateTime = getCurrentDateTime();
   
    return dateTime;
}
