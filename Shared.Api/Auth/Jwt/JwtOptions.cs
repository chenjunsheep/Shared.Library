using System;

namespace Shared.Api.Auth.Jwt
{
    public class JwtOptions
    {
        public string issuer { get; set; } = "www.ceejay.com";
        public string audience { get; set; } = "www.ceejay.com";
        public double expiration { get; set; } = TimeSpan.FromHours(24).TotalSeconds;
        public string fieldtoken { get; } = "access_token";
        public string fieldcutomized { get; } = "ceejay_field";

        /// <summary>
        /// create a new instance of AmeOptions object with default values
        /// </summary>
        public static JwtOptions Default
        {
            get { return new JwtOptions(); }
        }
    }
}
