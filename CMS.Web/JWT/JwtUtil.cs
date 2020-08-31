using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMS.Web.JWT
{
    public class JwtUtil
    {

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="claims">用户信息</param>
        /// <returns></returns>
        public static string CreateToken(IEnumerable<Claim> claims)
        {
            var jwtSetting = new JwtSetting();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //创建令牌
            var token = new JwtSecurityToken(
              issuer: jwtSetting.Issuer,
              audience: jwtSetting.Audience,
              signingCredentials: creds,
              claims: claims,
              notBefore: DateTime.Now,
              expires: DateTime.Now.AddSeconds(jwtSetting.ExpireSeconds)
            );
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }
    }

    public class JwtSetting
    {
        /// <summary>
        /// 颁发者
        /// </summary>
        public  string Issuer = "MatFct|Memory";
        /// <summary>
        /// 接收者
        /// </summary>
        public  string Audience = "Memory|MatFct";
        /// <summary>
        /// 秘钥
        /// </summary>
        public  string Secret = "YsDetMkxcMsQwerPenuatAMDOprDuvEkm";
        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public  int ExpireSeconds = 60;

    }
}
