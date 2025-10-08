namespace ChatApplication.Server.Encryption
{
    public interface IEncryptionService
    {
        string Decrypt(string cipherText);
        string Encrypt(string plainText);
    }
}