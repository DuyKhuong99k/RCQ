using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Security.Crypt
{
    public class ED
    {
        private static string DefaultPasswordEncrypt = "nhphuc@gmail.com";

        public static void Encrypt<T>(ref T obj)
        {
            foreach(var item in obj.GetType().GetProperties())
            {
                if(Attribute.IsDefined(item, typeof(EAD.EncryptAttribute)))
                {
                    var val = item.GetValue(obj, null).ToString();
                    item.SetValue(obj, EncryptString(val, DefaultPasswordEncrypt), null);
                }
            }
        }

        public static void SetDefaultString(string def) { DefaultPasswordEncrypt = def; }
        public static string EncryptString(string text) { return EncryptString(text, DefaultPasswordEncrypt); }

        public static void Decrypt<T>(ref T obj)
        {
            foreach(var item in obj.GetType().GetProperties())
            {
                if(Attribute.IsDefined(item, typeof(EAD.EncryptAttribute)))
                {
                    var val = item.GetValue(obj, null).ToString();
                    item.SetValue(obj, DecryptString(val, DefaultPasswordEncrypt), null);
                }
            }
        }

        public static string DecryptString(string text) { return DecryptString(text, DefaultPasswordEncrypt); }

        public static string EncryptString(string text, string password)
        {
            string result = string.Empty;
            try
            {
                byte[] baPwd = Encoding.UTF8.GetBytes(password);

                // Hash the password with SHA256
                byte[] baPwdHash = SHA256Managed.Create().ComputeHash(baPwd);

                byte[] baText = Encoding.UTF8.GetBytes(text);

                byte[] baSalt = GetRandomBytes();
                byte[] baEncrypted = new byte[baSalt.Length + baText.Length];

                for(int i = 0; i < baText.Length; i++)
                    baEncrypted[i] = baText[i];
                for(int i = 0; i < baSalt.Length; i++)
                    baEncrypted[i + baText.Length] = baSalt[i];

                baEncrypted = AES_Encrypt(baEncrypted, baPwdHash);

                result = Convert.ToBase64String(baEncrypted);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                //throw ex;
            }

            return result;
        }

        public static string DecryptString(string text, string password)
        {
            string result = string.Empty;
            try
            {
                byte[] baPwd = Encoding.UTF8.GetBytes(password);

                // Hash the password with SHA256
                byte[] baPwdHash = SHA256Managed.Create().ComputeHash(baPwd);

                byte[] baText = Convert.FromBase64String(text);

                byte[] baDecrypted = AES_Decrypt(baText, baPwdHash);

                // Remove salt
                int saltLength = GetSaltLength();
                byte[] baResult = new byte[baDecrypted.Length - saltLength];
                for(int i = 0; i < baResult.Length; i++)
                    baResult[i] = baDecrypted[i];

                result = Encoding.UTF8.GetString(baResult);
            } catch(Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //throw ex;
            }

            return result;
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            try
            {
                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using(MemoryStream ms = new MemoryStream())
                {
                    using(RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using(var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            //cs.Close();
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }
            } catch(Exception ex)
            {
                throw ex;
            }

            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            try
            {
                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using(MemoryStream ms = new MemoryStream())
                {
                    using(RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using(var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            //cs.Close();
                        }
                        decryptedBytes = ms.ToArray();
                    }
                }
            } catch(Exception ex)
            {
                throw ex;
            }
            return decryptedBytes;
        }

        private static byte[] GetRandomBytes()
        {
            int saltLength = GetSaltLength();
            byte[] ba = new byte[saltLength];
            RNGCryptoServiceProvider.Create().GetBytes(ba);
            return ba;
        }

        private static int GetSaltLength() { return 8; }
    }
}