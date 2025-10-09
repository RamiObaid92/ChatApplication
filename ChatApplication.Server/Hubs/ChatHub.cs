using ChatApplication.Server.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Ganss.Xss;

namespace ChatApplication.Server.Hubs;

// TODO: Implement Hub methods for sending and receiving messages, managing chat rooms, etc. After AuthEndpoints are done.
[Authorize]
public class ChatHub(ApplicationDbContext dbContext) : Hub
{
    private readonly ApplicationDbContext _dbContext = dbContext;

}
