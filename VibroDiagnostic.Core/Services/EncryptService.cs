using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Interfaces;

namespace VibroDiagnostic.Core.Services;

public class EncryptService: IEncryptService
{
    private readonly AppSettings _appSettings;

    public EncryptService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public byte[] EncryptFile(byte[] content)
    {
        var key = _appSettings.SecretEncryption;
        return Encrypt(key, content);
    }
    private byte[] Encrypt(string key, byte[] input)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(input, 0, input.Length);
                }
                array = memoryStream.ToArray();
            }
            
        }
        return array;
    }
}