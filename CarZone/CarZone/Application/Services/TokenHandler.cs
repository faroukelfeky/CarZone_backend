using Kawerk.Application.Interfaces;
using Kawerk.Domain;
using Kawerk.Infastructure.Context;
using Kawerk.Infastructure.ResponseClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kawerk.Application.Services
{
    public class TokenHandler : ITokenHandler
    {
        private readonly DbBase _db;
        private readonly IConfiguration _config;
        //private readonly ILogger _logger;
        public TokenHandler(DbBase db, IConfiguration config)
        {
            _db = db;
            _config = config;
            //_logger = logger;
        }

        //        *********** Setters ***********
        public async Task<string> CreateAccessToken(Guid userID, string name, string email, string role) // For creating access Tokens
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, name),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Sub,userID.ToString()),
                new Claim(ClaimTypes.Role,role)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var TokenDescriptor = new JwtSecurityToken(
                issuer: _config.GetValue<string>("JwtSettings:Issuer"),
                audience: _config.GetValue<string>("JwtSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),//EXPIRATION DATE
                signingCredentials: creds
                );
            //_logger.LogInformation("Json Web Token created");

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(TokenDescriptor));
        }
        public async Task<string> CreateRefreshToken(Guid customerID)//For creating new RefreshTokens
        {
            string token = GenerateRandomToken();
            var RefreshToken = new RefreshTokens
            {
                RefreshToken = token,
                CustomerID = customerID,
                Expires = DateTime.UtcNow.AddDays(4)
            };
            await _db.RefreshTokens.AddAsync(RefreshToken);
            //await _db.SaveChangesAsync();
            return token;
        }
        public async Task<string?> RefreshingToken(Guid CustomerID) //For logging in
        {
            var isTokenExists = (from token in _db.RefreshTokens
                                 where token.CustomerID == CustomerID
                                 select token).FirstOrDefault();

            string refreshToken = GenerateRandomToken();
            
            isTokenExists.Expires = DateTime.UtcNow.AddDays(4);
            isTokenExists.RefreshToken = refreshToken;
            _db.RefreshTokens.Update(isTokenExists);
            await _db.SaveChangesAsync();
            return refreshToken;
        }
        public static string GenerateRandomToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        //-----------------------------------------------------------------------


        //        *********** Extra Validation Function ***********

        public async Task<ResponseToken?> ValidateAccessToken(string Refreshtoken) // for logging in with Tokens
        {
            var result = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.RefreshToken == Refreshtoken);
            if (result == null || result.Expires < DateTime.UtcNow)
            {
                return null;
            }
            var user = await (from u in _db.Customers.Include(u => u.Role)
                              where u.CustomerID == result.CustomerID
                              select u).FirstOrDefaultAsync();

            if (user == null)
                return null;

            var Token = await CreateAccessToken(user.CustomerID, user.Name, user.Email, user.Role);

            result.Expires = DateTime.UtcNow.AddDays(4);
            result.RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var Response = new ResponseToken
            {
                status = 1,
                AccessToken = Token,
                RefreshToken = result.RefreshToken,
                msg = "Token refreshed successfully"
            };

            _db.RefreshTokens.Update(result);
            await _db.SaveChangesAsync();
            return Response;
        }

        //-----------------------------------------------------------------------

        //        *********** Getters ***********
        public async Task<PagedList<RefreshTokens>> GetAllRefreshTokens(int page, int pageSize)
        {
            if (page == 0)
                page = 1;

            if (pageSize == 0)
                pageSize = 10;

            var tokensQuery = from t in _db.RefreshTokens
                              select t;

            var tokens = await PagedList<RefreshTokens>.CreateAsync(tokensQuery, page, pageSize);
            return tokens;
        }
    }
}
