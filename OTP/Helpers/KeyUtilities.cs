
using System;

namespace OTP
{
    internal class KeyUtilities
    {

        static internal byte[] GetBigEndianBytes(long input)
        {
            // Since .net uses little endian numbers, we need to reverse the byte order to get big endian.
            var data = BitConverter.GetBytes(input);
            Array.Reverse(data);
            return data;
        }

        static internal byte[] GetBigEndianBytes(int input)
        {
            var data = BitConverter.GetBytes(input);
            Array.Reverse(data);
            return data;
        }
    }
}
