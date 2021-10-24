using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

public static class SaveLoadData
{
    // 클라우드 x
    private static string optionFile = "*******";
    private static string dataFile = "********";
    private static string timeFile = "********";

    private static string cryptoKey = "**************";
    private static string cryptoIV = "*********";

    public static bool isFirstRun()
    {
        string path = Application.persistentDataPath + dataFile;

        if (File.Exists(path))
        {
            return false;
        }

        return true;
    }

    // 암호화 추가
    public static void SaveUserData(UserData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + dataFile;

        FileStream stream = new FileStream(path, FileMode.Create);
        string json = JsonUtility.ToJson(data);

        byte[] key = Encoding.UTF8.GetBytes(cryptoKey);
        byte[] iv = Encoding.UTF8.GetBytes(cryptoIV);

        byte[] enc = RoomRapping.EncryptStringToBytes(json, key, iv);
        formatter.Serialize(stream, enc);
        stream.Close();
    }

    public static UserData LoadUserData()
    {
        string path = Application.persistentDataPath + dataFile;
        UserData data = new UserData();

        if (File.Exists(path))
        {
            Debug.Log(path);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            byte[] key = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] iv = Encoding.UTF8.GetBytes(cryptoIV);

            byte[] encryptedData = formatter.Deserialize(stream) as byte[];

            string json = RoomRapping.DecryptStringFromBytes(encryptedData, key, iv);

            data = JsonUtility.FromJson<UserData>(json);
            stream.Close();

            return data;
        }
        else
        {
            // 초기 지급본 // 최초 실행
            BinaryFormatter formattet = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            string json = JsonUtility.ToJson(data);

            byte[] key = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] iv = Encoding.UTF8.GetBytes(cryptoIV);

            byte[] enc = RoomRapping.EncryptStringToBytes(json, key, iv);
            formattet.Serialize(stream, enc);
            return data;
        }
    }

    public static void SaveTimeRecord(TimeRecord data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + timeFile;

        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static TimeRecord LoadTimeRecord()
    {
        string path = Application.persistentDataPath + timeFile;
        TimeRecord data = new TimeRecord();

        if (File.Exists(path))
        {
            Debug.Log(path);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as TimeRecord;
            stream.Close();

            return data;
        }
        else
        {
            // 초기 지급본 // 최초 실행
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, data);
            stream.Close();

            return data;
        }
    }

    public static void SaveOptionData(OptionData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + optionFile;

        FileStream stream = new FileStream(path, FileMode.Create);
        string json = JsonUtility.ToJson(data);

        byte[] key = Encoding.UTF8.GetBytes(cryptoKey);
        byte[] iv = Encoding.UTF8.GetBytes(cryptoIV);

        byte[] enc = RoomRapping.EncryptStringToBytes(json, key, iv);
        formatter.Serialize(stream, enc);
        stream.Close();
    }

    public static OptionData LoadOptionData()
    {
        string path = Application.persistentDataPath + optionFile;
        OptionData data = new OptionData();

        if (File.Exists(path))
        {
            Debug.Log(path);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            byte[] key = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] iv = Encoding.UTF8.GetBytes(cryptoIV);

            byte[] encryptedData = formatter.Deserialize(stream) as byte[];

            string json = RoomRapping.DecryptStringFromBytes(encryptedData, key, iv);

            data = JsonUtility.FromJson<OptionData>(json);
            stream.Close();

            return data;
        }
        else
        {
            // 초기 지급본 // 최초 실행
            BinaryFormatter formattet = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            string json = JsonUtility.ToJson(data);

            byte[] key = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] iv = Encoding.UTF8.GetBytes(cryptoIV);

            byte[] enc = RoomRapping.EncryptStringToBytes(json, key, iv);
            formattet.Serialize(stream, enc);
            return data;
        }
    }
}

public static class RoomRapping
{
    #region [ Basic ]
    public static string Decrypt(string textToDecrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();

        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;

        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;

        byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;
        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;

        byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        return Encoding.UTF8.GetString(plainText);
    }

    public static string Encrypt(string textToEncrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();

        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;

        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;

        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;

        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;

        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);

        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }
    #endregion

    public static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;

        using (Rijndael rijAlg = Rijndael.Create())
        {
            rijAlg.Key = Key;
            rijAlg.IV = IV;

            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return encrypted;
    }

    public static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        string plaintext = null;

        using (Rijndael rijAlg = Rijndael.Create())
        {
            rijAlg.Key = Key;
            rijAlg.IV = IV;

            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader
(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

        }

        return plaintext;
    }
}
