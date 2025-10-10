using ChatApplication.Server.Data.Context;
using ChatApplication.Server.Data.Entities;
using ChatApplication.Server.Encryption;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApplication.Server.Hubs;

public class ChatHub : Hub
{
    private readonly HtmlSanitizer _sanitizer = new();

    public async Task SendMessage(string user, string message)
    {

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var sanitizedUser = _sanitizer.Sanitize(user);
        var sanitizedMessage = _sanitizer.Sanitize(message);

        await Clients.All.SendAsync("ReceiveMessage", sanitizedUser, sanitizedMessage);
    }
}
