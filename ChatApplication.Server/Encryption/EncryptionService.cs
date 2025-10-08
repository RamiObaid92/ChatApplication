namespace ChatApplication.Server.Encryption;

public class EncryptionService
{
    public EncryptionService(IConfiguration configuration)
    {
        var aesKey = configuration["AesKey"];

    }
}
