using Domain.Entities;

public interface ITokenService
{
    string GenerateAccessToken(User user, IList<string>? roles = null);
}