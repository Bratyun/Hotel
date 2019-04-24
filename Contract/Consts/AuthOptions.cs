using System;

namespace Contract.Consts
{
    public class AuthOptions
    {
        public const string ISSUER = "HotelServer";
        public static readonly TimeSpan LIFETIME = new TimeSpan(24, 0, 0);
    }
}
