using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Api.Auth.Jwt
{
    public class JwtManager
    {
        private string SecretKey { get; set; }

        public SymmetricSecurityKey SignKey
        {
            get
            {
                return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
            }
        }

        public SigningCredentials SignCredential
        {
            get
            {
                return new SigningCredentials(SignKey, SecurityAlgorithms.HmacSha256);
            }
        }

        public JwtManager(string secretKey)
        {
            SecretKey = secretKey;
        }

        #region Public Method
        /// <summary>
        /// generate JWT token string
        /// </summary>
        /// <para>usage sample:</para>
        /// <remarks>
        /// <code>
        /// var token = new JwtManager("string of secretKey").GetToken(keyValue, contentValue);
        /// </code>
        /// </remarks>
        /// <param name="key">unique Id. E.g. user Id</param>
        /// <param name="content">content of any object formatted in string</param>
        /// <returns></returns>
        public string TokenCreate(string key, string content, double expiredHour)
        {
            var ops = JwtOptions.Default;
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: ops.issuer,
                audience: ops.audience,
                notBefore: now,
                expires: now.AddHours(expiredHour),
                signingCredentials: SignCredential,
                claims: BuildConent(key, content)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        /// <summary>
        /// get content decrypted from JWT token
        /// </summary>
        /// <remarks>
        /// <para>usage sample:</para>
        /// <code>
        /// var content = JwtManager.GetContent(User);
        /// </code>
        /// </remarks>
        /// <param name="principal">an instance of authorized user named in Microsoft.AspNetCore.Mvc</param>
        /// <returns></returns>
        public static string TokenDecrypt(ClaimsPrincipal principal)
        {
            if (principal != null)
            {
                var accessToken = principal.FindFirst(JwtOptions.Default.fieldtoken)?.Value;
                var jwtToken = new JwtSecurityToken(accessToken);
                return TokenDecrypt(jwtToken);
            }

            return string.Empty;
        }

        /// <summary>
        /// get content decrypted from JWT token
        /// </summary>
        /// <remarks>
        /// <para>usage sample:</para>
        /// <code>
        /// var content = JwtManager.GetContent(JwtSecurityToken);
        /// </code>
        /// </remarks>
        /// <param name="jwtToken">JWT token</param>
        /// <returns></returns>
        public static string TokenDecrypt(JwtSecurityToken jwtToken)
        {
            if (jwtToken != null)
            {
                var content = jwtToken.Payload.Claims.Where(c => c.Type == JwtOptions.Default.fieldcutomized).SingleOrDefault()?.Value;
                return Decrypt(content);
            }

            return string.Empty;
        }

        public static string Encrypt(string value)
        {
            return value;
        }

        public static string Decrypt(string value)
        {
            return value;
        }

        #endregion

        #region Private Method

        private IEnumerable<Claim> BuildConent(string key, string content)
        {
            var list = new List<Claim>();
            list.Add(new Claim(JwtRegisteredClaimNames.UniqueName, key));
            if (!string.IsNullOrEmpty(content))
            {
                list.Add(new Claim(JwtOptions.Default.fieldcutomized, Encrypt(content.ToString())));
            }
            return list;
        }

        #endregion
    }

    public static class AmeJwtExtension
    {
        /// <summary>
        /// default authentication settings
        /// </summary>
        /// <remarks>
        /// <para>usage sample:</para>
        /// <code>
        /// services.UseAuthenticationDefault("string of secrect key");
        /// </code>
        /// </remarks>
        /// <param name="service">an instence of .Net Core service</param>
        /// <param name="secretKey">secret key</param>
        /// <returns></returns>
        public static AuthenticationBuilder UseAuthenticationDefault(this IServiceCollection service, string secretKey)
        {
            if (service != null)
            {
                var builder = service
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.UseJwtOptionsDefault(secretKey);
                    });
                return builder;
            }

            return null;
        }

        /// <summary>
        /// default settings for JWT authentication
        /// </summary>
        /// <remarks>
        /// <para>usage sample:</para>
        /// <code>
        /// IServiceCollection
        /// .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        /// .AddJwtBearer(options => {
        ///     options.UseJwtOptionsDefault("string of secretKey");
        /// });
        /// </code>
        /// </remarks>
        /// <param name="options">JWT options</param>
        /// <param name="secretKey">secret key</param>
        /// <returns></returns>
        public static JwtBearerOptions UseJwtOptionsDefault(this JwtBearerOptions options, string secretKey)
        {
            if (options != null)
            {
                var defOpt = JwtOptions.Default;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = defOpt.issuer,
                    ValidAudience = defOpt.audience,
                    IssuerSigningKey = new JwtManager(secretKey).SignKey
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async ctx =>
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await ctx.Response.WriteAsync(ctx.Exception.Message);
                    },
                    OnTokenValidated = async ctx =>
                    {
                        var accessToken = ctx.SecurityToken as JwtSecurityToken;
                        if (accessToken != null)
                        {
                            var identity = ctx.Principal.Identity as ClaimsIdentity;
                            if (identity != null)
                            {
                                identity.AddClaim(new Claim(defOpt.fieldtoken, accessToken.RawData));
                            }
                        }

                        await Task.CompletedTask;
                    }
                };
            }

            return options;
        }
    }
}
