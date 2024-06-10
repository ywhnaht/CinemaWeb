using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace CinemaWeb.SupportFile
{
    public class NotificationHub : Hub
    {
        public void SendNotification(string title, string message)
        {
            Clients.All.broadcastNotification(title, message);
        }
    }
}