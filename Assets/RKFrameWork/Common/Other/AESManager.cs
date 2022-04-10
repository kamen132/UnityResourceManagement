
using System.Security.Cryptography;
using System.Text;

public class AESManager  {

    public static byte[] Encrypt(string key, byte[] toEncryptArray)
    {
        byte[] keyByte = Encoding.UTF8.GetBytes(FileUtils.GetStringMD5(key));
        return Encrypt(keyByte, toEncryptArray);
    }

    public static byte[] Decrypt(string key, byte[] toEncryptArray, int offset, int length)
    {
        byte[] keyByte = Encoding.UTF8.GetBytes(FileUtils.GetStringMD5(key));
        return Decrypt(keyByte, toEncryptArray, offset, length);
    }

    public static byte[] Decrypt(string key, byte[] toEncryptArray)
    {
        byte[] keyByte = Encoding.UTF8.GetBytes(FileUtils.GetStringMD5(key));
        return Decrypt(keyByte, toEncryptArray);
    }

    public static byte[] Encrypt(byte[] keyArray, byte[] toEncryptArray)
    {
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        
        rDel.Padding = PaddingMode.PKCS7;
        rDel.BlockSize = 256;
        ICryptoTransform cTransform = rDel.CreateEncryptor();
        return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
    }

    public static byte[] Decrypt(byte[] keyArray, byte[] toEncryptArray)
    {
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.BlockSize = 256;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateDecryptor();
        return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
    }
    public static byte[] Decrypt(byte[] keyArray, byte[] toEncryptArray, int offset, int length)
    {
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.BlockSize = 256;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateDecryptor();
        return cTransform.TransformFinalBlock(toEncryptArray, offset, length);
    }
}
