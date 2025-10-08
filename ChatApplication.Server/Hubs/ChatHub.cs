using ChatApplication.Server.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Ganss.Xss;

namespace ChatApplication.Server.Hubs;

[Authorize]
public class ChatHub(ApplicationDbContext dbContext) : Hub
{
    private readonly ApplicationDbContext _dbContext = dbContext;

}
