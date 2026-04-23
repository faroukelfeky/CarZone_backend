using CarZone.Domain;
using CarZone.Infastructure.ResponseClasses;

namespace CarZone.Application.Interfaces
{
    public interface ITokenHandler
    {
        public Task<string> CreateAccessToken(Guid userID, string name, string email, string role);
        public Task<string> CreateRefreshToken(Guid customerID);
        public Task<string?> RefreshingToken(Guid UserID);
        public Task<ResponseToken?> ValidateAccessToken(string token);
        public Task<PagedList<RefreshTokens>> GetAllRefreshTokens(int page, int pageSize);
    }
}