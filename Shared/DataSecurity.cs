using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Shared.Security
{
    /// <summary>
    /// Class for private and public keys and data-encryption
    /// </summary>
    public class DataSecurity
    {
        // public and private keypair
        private string publicXmlKey;
        private string privateXmlKey;

        // encoder
        private UnicodeEncoding _encoder = new UnicodeEncoding();

        // symmetric key
        private Aes aes;

        // internal properties
        public string PublicXmlKey => publicXmlKey;
        public Aes Aes => aes;

        public DataSecurity()
        {
            var csp = new RSACryptoServiceProvider();
            publicXmlKey = csp.ToXmlString(false);
            privateXmlKey = csp.ToXmlString(true);

            aes = Aes.Create();
        }
        public DataSecurity(Aes aes)
        {
            var csp = new RSACryptoServiceProvider();

            this.aes = aes;
        }
        public DataSecurity(string publicXmlKey, Aes aes)
        {
            this.publicXmlKey = publicXmlKey;
            this.aes = aes;
        }
        public DataSecurity(string publicXmlKey, string privateXmlKey)
        {
            this.publicXmlKey = publicXmlKey;
            this.privateXmlKey = privateXmlKey;
        }

        public DataSecurity(string publicXmlKey, string privateXmlKey, Aes aes)
        {
            this.publicXmlKey = publicXmlKey;
            this.privateXmlKey = privateXmlKey;
            this.aes = aes;
        }


        public void ReRoll()
        {
            var csp = new RSACryptoServiceProvider();
            publicXmlKey = csp.ToXmlString(false);
            privateXmlKey = csp.ToXmlString(true);

            aes = Aes.Create();
        }

        public string EncryptRSA(string key, string message)
        {
            var csp = new RSACryptoServiceProvider();
            csp.FromXmlString(key);
            byte[] data = _encoder.GetBytes(message);
            byte[] cypher = csp.Encrypt(data, false);
            return Convert.ToBase64String(cypher);
        }
        public string DecryptRSA(string message)
        {
            var csp = new RSACryptoServiceProvider();
            csp.FromXmlString(privateXmlKey);
            byte[] dataBytes = Convert.FromBase64String(message);
            byte[] plainText = csp.Decrypt(dataBytes, false);
            return _encoder.GetString(plainText);
        }

        public string EncryptAES(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        public string DecryptAES(string cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            byte[] cipherTextB = Convert.FromBase64String(cipherText);

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherTextB))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
