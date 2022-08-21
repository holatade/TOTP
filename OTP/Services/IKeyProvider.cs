

using OTP.Enums;

namespace OTP
{
    public interface IKeyProvider
    {
        byte[] ComputeHmac(OtpHashMode mode, byte[] data);
    }
}
