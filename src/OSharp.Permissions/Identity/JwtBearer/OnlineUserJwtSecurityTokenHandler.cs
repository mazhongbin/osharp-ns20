﻿// -----------------------------------------------------------------------
//  <copyright file="OnlineUserJwtSecurityTokenHandler.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2018 OSharp. All rights reserved.
//  </copyright>
//  <site>http://www.osharp.org</site>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2018-07-09 15:01</last-date>
// -----------------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using OSharp.Collections;
using OSharp.Dependency;
using OSharp.Secutiry.Claims;


namespace OSharp.Identity.JwtBearer
{
    /// <summary>
    /// 使用在线用户信息和JwtToken生成在线ClaimsIdentity
    /// </summary>
    public class OnlineUserJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        /// <summary>
        /// Creates a <see cref="T:System.Security.Claims.ClaimsIdentity" /> from a <see cref="T:System.IdentityModel.Tokens.Jwt.JwtSecurityToken" />.
        /// </summary>
        /// <param name="jwtToken">The <see cref="T:System.IdentityModel.Tokens.Jwt.JwtSecurityToken" /> to use as a <see cref="T:System.Security.Claims.Claim" /> source.</param>
        /// <param name="issuer">The value to set <see cref="P:System.Security.Claims.Claim.Issuer" /></param>
        /// <param name="validationParameters"> Contains parameters for validating the token.</param>
        /// <returns>A <see cref="T:System.Security.Claims.ClaimsIdentity" /> containing the <see cref="P:System.IdentityModel.Tokens.Jwt.JwtSecurityToken.Claims" />.</returns>
        protected override ClaimsIdentity CreateClaimsIdentity(JwtSecurityToken jwtToken,
            string issuer,
            TokenValidationParameters validationParameters)
        {
            ClaimsIdentity identity = base.CreateClaimsIdentity(jwtToken, issuer, validationParameters);

            if (identity.IsAuthenticated)
            {
                //由用户名获取在线缓存的角色赋给Identity
                IOnlineUserCache onlineUserCache = ServiceLocator.Instance.GetService<IOnlineUserCache>();
                OnlineUser user = onlineUserCache.GetOrRefresh(identity.Name);
                Claim roleClaim = identity.Claims.FirstOrDefault(m => m.Type == ClaimTypes.Role);
                if (roleClaim != null)
                {
                    identity.RemoveClaim(roleClaim);
                }
                if (user.Roles.Length > 0)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, user.Roles.ExpandAndToString()));
                }
            }

            return identity;
        }
    }
}