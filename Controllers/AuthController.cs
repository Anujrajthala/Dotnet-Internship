using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;
using TodoApi.Services.ServiceImpl;

[Route("api/auth")]
[ApiController]
public class AuthController:ControllerBase{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService){
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDTO registerRequestDTO){
        var response = await _authService.RegisterUserAsync(registerRequestDTO);
        return Ok(response);
    }
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequestDTO loginRequestDTO){
        var response  = await _authService.LoginUserAsync(loginRequestDTO);
        return Ok(response);
    }
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyUserEmail([FromQuery] string token){
        var result =await _authService.VerifyEmail(token);
        return result?Ok("Email verified successfully"):BadRequest("Token invalid");
    }
    [Authorize]
    [HttpPost("refreshsession")]
    public async Task<IActionResult> RefreshSession([FromBody] string refreshToken){
        var result = await _authService.RefreshSession( refreshToken);
        return Ok(result);
    }
}