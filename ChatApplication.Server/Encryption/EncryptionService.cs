using System.Security.Cryptography;
using System.Text;

namespace ChatApplication.Server.Encryption;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly ILogger<EncryptionService> _logger;

    public EncryptionService(IConfiguration configuration, ILogger<EncryptionService> logger)
    {
        _logger = logger;
        var aesKey = configuration["AesKey"];

        if (string.IsNullOrEmpty(aesKey) || aesKey.Length != 32)
        {
            _logger.LogError("Invalid AES key configuration. Key must be a 32-character string. Received length: {KeyLength}", aesKey?.Length ?? 0);
            throw new ArgumentException("AesKey must be a 32-character string.");
        }

        _key = Encoding.UTF8.GetBytes(aesKey);
        _logger.LogInformation("EncryptionService initialized successfully");
    }

    public string Encrypt(string plainText)
    {
        _logger.LogDebug("Starting encryption for text of length {Length}", plainText?.Length ?? 0);

        try
        {
            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream msEncrypt = new();
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);

            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
            }

            var result = Convert.ToBase64String(msEncrypt.ToArray());
            _logger.LogDebug("Encryption completed successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during encryption");
            throw;
        }
    }

    public string Decrypt(string cipherText)
    {
        _logger.LogDebug("Starting decryption for cipher text of length {Length}", cipherText?.Length ?? 0);

        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = _key;

            byte[] iv = new byte[aes.BlockSize / 8];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream msDecrypt = new(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            var result = srDecrypt.ReadToEnd();
            _logger.LogDebug("Decryption completed successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during decryption. Returning placeholder message");
            return "[Unable to decrypt message]";
        }
    }
}
