using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Services;

public interface IAuthService{
    public Task<ResponseDTO<RegisterResponseDTO>> RegisterUserAsync(RegisterRequestDTO registerRequestDTO);
    public Task<ResponseDTO<LoginResponseDTO>> LoginUserAsync(LoginRequestDTO loginRequestDTO);
    public Task<bool> VerifyEmail(string token);
    public string GenerateAccessToken(User user);
     public RefreshToken GenerateRefreshToken();
    public Task<ResponseDTO<LoginResponseDTO>> RefreshSession(string refreshToken);
}