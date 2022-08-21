
using System.Collections.Generic;

namespace OTP
{
    public class VerificationWindow
    {
        private readonly int previous;
        private readonly int future;

        public VerificationWindow(int previous = 0, int future = 0)
        {
            this.previous = previous;
            this.future = future;
        }

        public IEnumerable<long> ValidationCandidates(long initialFrame)
        {
            yield return initialFrame;
            for(int i = 1; i <= previous; i++)
            {
                var val = initialFrame - i;
                if(val < 0)
                    break;
                yield return val;
            }

            for(int i = 1; i <= future; i++)
                yield return initialFrame + i;
        }

        public static readonly VerificationWindow RfcSpecifiedNetworkDelay = new VerificationWindow(previous: 1, future: 1);
    }
}
