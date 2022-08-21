using OTP.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OTP
{
    public class KeyProvider : IKeyProvider
    {

        readonly object stateSync = new object();
        readonly byte[] KeyData;
        readonly int keyLength;

        public KeyProvider(byte[] key)
        {
            if (!(key != null))
                throw new ArgumentNullException("key");
            if (!(key.Length > 0))
                throw new ArgumentException("The key must not be empty");

            this.keyLength = key.Length;
            int paddedKeyLength = (int)Math.Ceiling((decimal)key.Length / (decimal)16) * 16;
            this.KeyData = new byte[paddedKeyLength];
            Array.Copy(key, this.KeyData, key.Length);
        }

        internal byte[] GetCopyOfKey()
        {
            var plainKey = new byte[this.keyLength];
            lock (this.stateSync)
            {
                Array.Copy(this.KeyData, plainKey, this.keyLength);
            }
            return plainKey;
        }

        public byte[] ComputeHmac(OtpHashMode mode, byte[] data)
        {
            byte[] hashedValue = null;
            using (HMAC hmac = CreateHmacHash(mode))
            {
                byte[] key = this.GetCopyOfKey();
                hmac.Key = key;
                hashedValue = hmac.ComputeHash(data);
            }

            return hashedValue;
        }

        private static HMAC CreateHmacHash(OtpHashMode otpHashMode)
        {
            HMAC hmacAlgorithm = null;
            switch (otpHashMode)
            {
                case OtpHashMode.Sha256:
                    hmacAlgorithm = new HMACSHA256();
                    break;
                case OtpHashMode.Sha512:
                    hmacAlgorithm = new HMACSHA512();
                    break;
                default: //case OtpHashMode.Sha1:
                    hmacAlgorithm = new HMACSHA1();
                    break;
            }
            return hmacAlgorithm;
        }
    }
}
