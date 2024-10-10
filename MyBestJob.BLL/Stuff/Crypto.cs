using System.Security.Cryptography;
using System.Text;

namespace MyBestJob.BLL.Stuff;

public static class Crypto
{
    public static string EncryptSensitiveData(string text, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.GenerateIV();

        using var memoryStream = new MemoryStream();
        memoryStream.Write(aes.IV, 0, aes.IV.Length);

        var textBytes = Encoding.UTF8.GetBytes(text);

        using var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        cryptoStream.Write(textBytes, 0, textBytes.Length);
        cryptoStream.FlushFinalBlock();

        var encrypted = Convert.ToBase64String(memoryStream.ToArray());
        return encrypted;
    }

    public static string DecryptSensitiveData(string text, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);

        var textBytes = Convert.FromBase64String(text);

        using var memoryStream = new MemoryStream(textBytes);
        var aesIV = new byte[aes.BlockSize / 8];
        memoryStream.Read(aesIV, 0, aesIV.Length);
        aes.IV = aesIV;

        using var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        var decrypted = streamReader.ReadToEnd();
        return decrypted;
    }
}
