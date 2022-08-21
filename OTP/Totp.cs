using OTP.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTP
{
    public class Totp : Otp
    {
        /// <summary>
        /// The number of ticks as Measured at Midnight Jan 1st 1970;
        /// </summary>
        const long unixEpochTicks = 621355968000000000L;
        /// <summary>
        /// A divisor for converting ticks to seconds
        /// </summary>
        const long ticksToSeconds = 10000000L;

        private readonly int step;
        private readonly int totpSize;
        private readonly TimeCorrection correctedTime;


        public Totp(byte[] secretKey, int step = 30, OtpHashMode mode = OtpHashMode.Sha1, int totpSize = 6, TimeCorrection timeCorrection = null)
            : base(secretKey, mode)
        {
            VerifyParameters(step, totpSize);

            this.step = step;
            this.totpSize = totpSize;

            this.correctedTime = timeCorrection ?? TimeCorrection.UncorrectedInstance;
        }

        private static void VerifyParameters(int step, int totpSize)
        {
            if (!(step > 0))
                throw new ArgumentOutOfRangeException("step");
            if (!(totpSize > 0))
                throw new ArgumentOutOfRangeException("totpSize");
            if (!(totpSize <= 10))
                throw new ArgumentOutOfRangeException("totpSize");
        }

        public string ComputeTotp(DateTime timestamp)
        {
            return ComputeTotpFromSpecificTime(this.correctedTime.GetCorrectedTime(timestamp));
        }

        public string ComputeTotp()
        {
            return this.ComputeTotpFromSpecificTime(this.correctedTime.CorrectedUtcNow);
        }

        private string ComputeTotpFromSpecificTime(DateTime timestamp)
        {
            var window = CalculateTimeStepFromTimestamp(timestamp);
            return this.Compute(window, this.hashMode);
        }

        public bool VerifyTotp(string totp, out long timeStepMatched, VerificationWindow window = null)
        {
            return this.VerifyTotpForSpecificTime(this.correctedTime.CorrectedUtcNow, totp, window, out timeStepMatched);
        }

        public bool VerifyTotp(DateTime timestamp, string totp, out long timeStepMatched, VerificationWindow window = null)
        {
            return this.VerifyTotpForSpecificTime(this.correctedTime.GetCorrectedTime(timestamp), totp, window, out timeStepMatched);
        }

        private bool VerifyTotpForSpecificTime(DateTime timestamp, string totp, VerificationWindow window, out long timeStepMatched)
        {
            var initialStep = CalculateTimeStepFromTimestamp(timestamp);
            return this.Verify(initialStep, totp, out timeStepMatched, window);
        }

        private long CalculateTimeStepFromTimestamp(DateTime timestamp)
        {
            var unixTimestamp = (timestamp.Ticks - unixEpochTicks) / ticksToSeconds;
            var window = unixTimestamp / (long)this.step;
            return window;
        }

        protected override string Compute(long counter, OtpHashMode mode)
        {
            var data = KeyUtilities.GetBigEndianBytes(counter);
            var otp = this.CalculateOtp(data, mode);
            return Digits(otp, this.totpSize);
        }
    }
}
