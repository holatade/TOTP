using System;
using System.Collections.Generic;
using System.Text;

namespace OTP
{
    public class TimeCorrection
    {
        
        public static readonly TimeCorrection UncorrectedInstance = new TimeCorrection();

        private readonly TimeSpan timeCorrectionFactor;

       
        private TimeCorrection()
        {
            this.timeCorrectionFactor = TimeSpan.FromSeconds(0);
        }

        public TimeCorrection(DateTime correctUtc)
        {
            this.timeCorrectionFactor = DateTime.UtcNow - correctUtc;
        }

        public TimeCorrection(DateTime correctTime, DateTime referenceTime)
        {
            this.timeCorrectionFactor = referenceTime - correctTime;
        }

        public DateTime GetCorrectedTime(DateTime referenceTime)
        {
            return referenceTime - timeCorrectionFactor;
        }

       
        public DateTime CorrectedUtcNow
        {
            get { return GetCorrectedTime(DateTime.UtcNow); }
        }

        public TimeSpan CorrectionFactor
        {
            get { return this.timeCorrectionFactor; }
        }
    }
}
