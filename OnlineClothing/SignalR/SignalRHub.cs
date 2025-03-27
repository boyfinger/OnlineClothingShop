using Microsoft.AspNetCore.SignalR;

namespace OnlineClothing.SignalR
{
    public class SignalRHub : Hub
    {
        public async Task RefreshData()
        {
            await Clients.All.SendAsync("ReloadData");
        }
    }
}
