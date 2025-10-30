namespace WebAPI.Hub
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using System.Security.Claims;

    [Authorize]
    public class ProgressHub : Hub
    {
        // Hub không cần phương thức cụ thể nếu chỉ dùng để gửi thông báo

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var encryptedToken = Context.GetHttpContext().Request.Query["access_token"].ToString();
            // Kiểm tra quyền truy cập hoặc thực hiện các hành động dựa trên thông tin trong claims
            var userId = user?.FindFirst(ClaimTypes.Name)?.Value;
            if (userId == null)
            {
                // Ngắt kết nối nếu không có userId
                Context.Abort();
            }

            await base.OnConnectedAsync();
        }

        // Bạn có thể thêm các phương thức khác nếu cần thiết để xử lý yêu cầu từ client
        // Ví dụ: Một phương thức đơn giản để gửi tin nhắn tới tất cả các client
        public Task SendMessageToAll(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", message);
        }

        // Xử lý khi ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Thực hiện các hành động nếu cần thiết khi ngắt kết nối
            await base.OnDisconnectedAsync(exception);
        }
    }
}
