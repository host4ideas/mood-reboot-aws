using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace APIMoodReboot.Helpers
{
    public class HelperOAuthToken
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public HelperOAuthToken(IConfiguration configuration)
        {
            Issuer = configuration.GetValue<string>("ApiOAuth:Issuer");
            Audience = configuration.GetValue<string>("ApiOAuth:Audience");
            SecretKey = configuration.GetValue<string>("ApiOAuth:SecretKey");
        }

        public SymmetricSecurityKey GetKeyToken()
        {
            byte[] data = Encoding.UTF8.GetBytes(this.SecretKey);
            return new SymmetricSecurityKey(data);
        }

        public Action<JwtBearerOptions> GetJwtOptions()
        {
            Action<JwtBearerOptions> options = new(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = this.Issuer,
                    ValidAudience = this.Audience,
                    IssuerSigningKey = this.GetKeyToken(),
                };
            });
            return options;
        }

        public Action<AuthenticationOptions> GetAuthenticationOptions()
        {
            Action<AuthenticationOptions> options = new(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            return options;
        }
    }
}
