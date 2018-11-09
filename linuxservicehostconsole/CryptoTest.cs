using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace linuxservicehostconsole
{
    class CryptoTest
    {
        public static byte[] _key = null;
        public static byte[] _iv = null;
        private static int _ivlength = 16;
        SecureRandom random = new SecureRandom();

        public CryptoTest()
        {
            generateEncryptionKeys();
        }

        private void generateEncryptionKeys()
        {
            _key = new byte[32]; // 256 bits
            random.NextBytes(_key);
            _iv = generateIV(16);
        }

        private static byte[] generateIV(int ivLength)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] nonce = new byte[_ivlength];
                rng.GetBytes(nonce);
                return nonce;
            }
        }

        public string Encrypt(string plainText)
        {

            AesEngine engine = new AesEngine();
            CbcBlockCipher blockCipher = new CbcBlockCipher(engine); //CBC
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding()); //Default scheme is PKCS5/PKCS7
            KeyParameter keyParam = new KeyParameter(_key);
            ParametersWithIV keyParamWithIV = new ParametersWithIV(keyParam, _iv, 0, _ivlength);

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            // Encrypt
            cipher.Init(true, keyParamWithIV);
            byte[] outputBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
            int length = cipher.ProcessBytes(inputBytes, outputBytes, 0);
            cipher.DoFinal(outputBytes, length); //Do the final block
            string encryptedInput = Convert.ToBase64String(outputBytes);
            return encryptedInput;
        }


        public string Decrypt(string base64encryptedText)
        {
            AesEngine engine = new AesEngine();
            CbcBlockCipher blockCipher = new CbcBlockCipher(engine); //CBC
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding()); //Default scheme is PKCS5/PKCS7
            KeyParameter keyParam = new KeyParameter(_key);
            ParametersWithIV keyParamWithIV = new ParametersWithIV(keyParam, _iv, 0, _ivlength);

            byte[] encryptedBytes = Convert.FromBase64String(base64encryptedText);
            cipher.Init(false, keyParamWithIV);
            byte[] resultingBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
            int length = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, resultingBytes, 0);
            cipher.DoFinal(resultingBytes, length); //Do the final block
            String result = Encoding.UTF8.GetString(resultingBytes);
            return result;
        }



        //private void generateEncryptionKeys()
        //{
        //    byte[] key = null;
        //    byte[] iv = null;
        //    key = WinRTCrypto.CryptographicBuffer.GenerateRandom(32); // 256 bits
        //    var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
        //    int blocklength = (int)provider.BlockLength;
        //    iv = WinRTCrypto.CryptographicBuffer.GenerateRandom(blocklength);

        //    _key = iv;
        //    _iv = iv;
        //}


        //public string Encrypt(string plainText)
        //{
        //    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        //    var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
        //    var key = provider.CreateSymmetricKey(_key);
        //    byte[] iv = _iv;
        //    byte[] encryptedBytes = WinRTCrypto.CryptographicEngine.Encrypt(key, plainBytes, iv);
        //    return Convert.ToBase64String(encryptedBytes);
        //}

        //public string Decrypt(string base64encryptedText)
        //{
        //    byte[] encryptedArray = Convert.FromBase64String(base64encryptedText);
        //    var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
        //    var key = provider.CreateSymmetricKey(_key);
        //    byte[] iv = _iv;
        //    byte[] decryptedBytes = WinRTCrypto.CryptographicEngine.Decrypt(key, encryptedArray, iv);
        //    return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
        //}
    }
}
