using SlaytifiedApi.Application.Dtos;
using System.Threading.Tasks;

namespace SlaytifiedApi.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    }
}
