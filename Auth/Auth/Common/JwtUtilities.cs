using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Common
{
	public static class JwtUtilities
	{
		private const string Payload = "payload";
		private const string Iat = "iat";
		
		public static string Generate(string payload, string secret = "secret")
		{
			var key = new byte[128];
			Encoding.UTF8.GetBytes(secret, 0, secret.Length, key, 0);
			SecurityKey securityKey = new SymmetricSecurityKey(key);

			var now = DateTime.Now;

			var claims = new Claim[]
			{
				new Claim(Payload, payload),
			};

			var token = new JwtSecurityToken(
				claims: claims, 
				notBefore: now, 
				expires: now.AddMonths(1),
				signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

			var handler = new JwtSecurityTokenHandler();

			return handler.WriteToken(token);
		}
	}
}