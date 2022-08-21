using OTP.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTP
{
    public abstract class Otp
    {

        protected readonly IKeyProvider secretKey;

        protected readonly OtpHashMode hashMode;

        public Otp(byte[] secretKey, OtpHashMode mode)
        {
            if (!(secretKey != null))
                throw new ArgumentNullException("secretKey");
            if (!(secretKey.Length > 0))
                throw new ArgumentException("secretKey empty");

            this.secretKey = new KeyProvider(secretKey);

            this.hashMode = mode;
        }

        protected abstract string Compute(long counter, OtpHashMode mode);

        // Boilerplate
        protected internal long CalculateOtp(byte[] data, OtpHashMode mode)
        {
            byte[] hmacComputedHash = this.secretKey.ComputeHmac(mode, data);

            // The RFC has a hard coded index 19 in this value.
            // This is the same thing but also accomodates SHA256 and SHA512
            // hmacComputedHash[19] => hmacComputedHash[hmacComputedHash.Length - 1]

            int offset = hmacComputedHash[hmacComputedHash.Length - 1] & 0x0F;
            return (hmacComputedHash[offset] & 0x7f) << 24
                | (hmacComputedHash[offset + 1] & 0xff) << 16
                | (hmacComputedHash[offset + 2] & 0xff) << 8
                | (hmacComputedHash[offset + 3] & 0xff) % 1000000;
        }

        protected internal static string Digits(long input, int digitCount)
        {
            var truncatedValue = ((int)input % (int)Math.Pow(10, digitCount));
            return truncatedValue.ToString().PadLeft(digitCount, '0');
        }

        protected bool Verify(long initialStep, string valueToVerify, out long matchedStep, VerificationWindow window)
        {
            if (window == null)
                window = new VerificationWindow();
            foreach (var frame in window.ValidationCandidates(initialStep))
            {
                var comparisonValue = this.Compute(frame, this.hashMode);
                if (ValuesEqual(comparisonValue, valueToVerify))
                {
                    matchedStep = frame;
                    return true;
                }
            }

            matchedStep = 0;
            return false;
        }

        // Constant time comparison of two values
        private bool ValuesEqual(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }

        
    }
}
