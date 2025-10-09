using ChatApplication.Server.Data.Entities;

namespace ChatApplication.Server.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(ApplicationUserEntity user);
    }
}