using Shorty.Management.Domain.Entities;

namespace Shorty.Management.Api.Features.Login;

internal sealed class LoginService
{
    private readonly LoginRepository _repository;

    public LoginService(LoginRepository repository)
    {
        _repository = repository;
    }

    public Task<User?> GetUserAsync(string email) =>
        _repository.GetUserAsync(email);
}
