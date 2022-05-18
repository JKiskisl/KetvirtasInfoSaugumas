using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ketvpraktNoforms
{
    class AESCrypto
    {
        ICryptoTransform encryptor;
        ICryptoTransform decryptor;

        /// <summary>
        /// Initialize AES class object and Cipher Object
        /// </summary>
        /// <param name="sKey">Secret key must be 8/16/32 long</param>
        public AESCrypto(string sKey)
        {
            RijndaelManaged aes = new RijndaelManaged();

            byte[] key = Encoding.UTF8.GetBytes(sKey);
            byte[] IV = Encoding.UTF8.GetBytes(sKey);

            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            decryptor = aes.CreateDecryptor(key, IV);
            encryptor = aes.CreateEncryptor(key, IV);
        }

        /// <summary>
        /// Encrypt File
        /// </summary>
        /// <param name="inputFile">Input file path</param>
        /// <param name="outputFile">Output file path</param>
        public void EncryptFile(string inputFile, string outputFile)
        {
            using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
            {

                using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        int data;
                        while ((data = fsIn.ReadByte()) != -1)
                        {
                            cs.WriteByte((byte)data);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt File
        /// </summary>
        /// <param name="inputFile">Input file path</param>
        /// <param name="outputFile">Output File path</param>
        public void DecryptFile(string inputFile, string outputFile)
        {
            using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
            {
                using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                    {
                        int data; while ((data = cs.ReadByte()) != -1)
                        { fsOut.WriteByte((byte)data); }
                    }
                }
            }
        }

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="input">Input text</param>
        /// <returns></returns>
        public string EncryptText(string input)
        {
            string output = "";
            using (Stream fsCrypt = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                {
                    var bytes = Encoding.UTF8.GetBytes(input);
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    fsCrypt.Position = 0;
                    byte[] bytes1 = new byte[fsCrypt.Length];
                    fsCrypt.Read(bytes1, 0, (int)fsCrypt.Length);
                    output = Convert.ToBase64String(bytes1);
                }
            }
            return output;
        }

        /// <summary>
        /// Decrypt Text
        /// </summary>
        /// <param name="input">input text</param>
        /// <returns></returns>
        public string DecryptText(string input)
        {
            string output = "";
            using (Stream fsCrypt = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Write))
                {
                    var bytes = Convert.FromBase64String(input);
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();

                    fsCrypt.Position = 0;
                    StreamReader reader = new StreamReader(fsCrypt);
                    output = reader.ReadToEnd();
                }
            }
            return output;
        }
        //-----------------------
        private static byte[] encrypt_aes256(byte[] unencrypted, byte[] key, byte[] salt)
        {
            byte[] encrypted = null;
            using (MemoryStream memory_stream = new MemoryStream())
            {
                using (RijndaelManaged aes256 = new RijndaelManaged())
                {
                    aes256.KeySize = 256; //64 chars equals aes256 key size
                    aes256.BlockSize = 128;//block = algorithm(256)/2
                    Rfc2898DeriveBytes rfc_key = new Rfc2898DeriveBytes(key, salt, 2000);
                    aes256.Key = rfc_key.GetBytes(aes256.KeySize / 8);
                    aes256.IV = rfc_key.GetBytes(aes256.BlockSize / 8);
                    aes256.Mode = CipherMode.CBC;//encrypts into blocks
                    using (CryptoStream crypto_stream = new CryptoStream(memory_stream, aes256.CreateEncryptor(), CryptoStreamMode.Write)) //creates the encryptor
                    {
                        crypto_stream.Write(unencrypted, 0, unencrypted.Length); //writes encrypted bytes to a memory stream
                        crypto_stream.Close();
                    }
                    encrypted = memory_stream.ToArray();
                }
            }
            return encrypted;
        }
        private static byte[] decrypt_aes256(byte[] encrypted, byte[] key, byte[] salt)
        {
            byte[] unencrypted = null;
            using (MemoryStream memory_stream = new MemoryStream())
            {
                using (RijndaelManaged aes256 = new RijndaelManaged())
                {
                    aes256.KeySize = 256; //64 chars equals aes256 key size
                    aes256.BlockSize = 128;//block = algorithm(256)/2
                    Rfc2898DeriveBytes rfc_key = new Rfc2898DeriveBytes(key, salt, 2000);
                    aes256.Key = rfc_key.GetBytes(aes256.KeySize / 8);
                    aes256.IV = rfc_key.GetBytes(aes256.BlockSize / 8);
                    aes256.Mode = CipherMode.CBC;//encrypts into blocks
                    using (CryptoStream crypto_stream = new CryptoStream(memory_stream, aes256.CreateDecryptor(), CryptoStreamMode.Write)) //creates the decryptor
                    {
                        crypto_stream.Write(encrypted, 0, encrypted.Length); //writes decrypted bytes to a memory stream
                        crypto_stream.Close();
                    }
                    unencrypted = memory_stream.ToArray();
                }
            }
            return unencrypted;
        }

        public static void Encrypt(string file, string key, string salt)
        {
            byte[] unencrypted = File.ReadAllBytes(file);
            File.Delete(file);
            byte[] key_bytes = Encoding.UTF8.GetBytes(key);
            key_bytes = SHA256Managed.Create().ComputeHash(key_bytes);
            byte[] salt_bytes = Encoding.UTF8.GetBytes(salt);
            salt_bytes = SHA256Managed.Create().ComputeHash(salt_bytes);
            byte[] encrypted = encrypt_aes256(unencrypted, key_bytes, salt_bytes);
            File.WriteAllBytes(file, encrypted);
        }
        public static void Decrypt(string file, string key, string salt)
        {
            byte[] encrypted = File.ReadAllBytes(file);
            File.Delete(file);
            byte[] key_bytes = Encoding.UTF8.GetBytes(key);
            key_bytes = SHA256Managed.Create().ComputeHash(key_bytes);
            byte[] salt_bytes = Encoding.UTF8.GetBytes(salt);
            salt_bytes = SHA256Managed.Create().ComputeHash(salt_bytes);
            byte[] decrypted = decrypt_aes256(encrypted, key_bytes, salt_bytes);
            File.WriteAllBytes(file, decrypted);
        }




    }
}
