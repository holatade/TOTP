using OTP.Enums;
using System;
using System.Text;

namespace OTP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var secret = "dnjT%7fbhh0ns-cnjsbcsY%";

            var totp = new Totp(secretKey: Encoding.UTF8.GetBytes(secret) , step : 600 ,  mode: OtpHashMode.Sha512);
            var totpCode = totp.ComputeTotp();
            Console.WriteLine("OTP Code" + totpCode);

            var verify = totp.VerifyTotp(totpCode, out long time);
            Console.WriteLine("Verification Status" + verify);
            Console.WriteLine("Time Count" + time);

        }
    }
}
