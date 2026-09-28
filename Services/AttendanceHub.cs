namespace AttendanceAPI.Services
{
    using Microsoft.AspNetCore.SignalR;

    public class AttendanceHub : Hub
    {
        public async Task SendAttendanceUpdate(object update)
        {
            await Clients.All.SendAsync("ReceiveAttendanceUpdate", update);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}