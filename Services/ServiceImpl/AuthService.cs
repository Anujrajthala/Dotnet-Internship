
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using MongoDB.Driver;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.BearerToken;
using TodoApi.Exceptions;

namespace TodoApi.Services.ServiceImpl;
public class AuthService : IAuthService
{
    private readonly IMongoCollection<User> _user;
    private readonly IHttpContextAccessor _accessor;
    private readonly IEmailService _emailService;
    private readonly IOptionsMonitor<JwtSettings> _jwtMonitor;
    public AuthService(ApplicationDbContext applicationDbContext, IEmailService emailService, IOptionsMonitor<JwtSettings> jwtMonitor, IHttpContextAccessor accessor){
        _user = applicationDbContext.Users;
        _emailService =  emailService;
        _jwtMonitor = jwtMonitor;
        _accessor = accessor;
    }
    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtMonitor.CurrentValue.Secret);
        var claims = new List<Claim>{
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Email, user.Email)
        };
        foreach(var role in user.Roles){
            claims.Add(new Claim(ClaimTypes.Role,role));
        }
        var token = new JwtSecurityToken(
            claims : claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

        );
        return tokenHandler.WriteToken(token);
    }
    public RefreshToken GenerateRefreshToken(){
        var token = Guid.NewGuid().ToString();
        return new RefreshToken{
            Token = token,
            Expires = DateTime.UtcNow.AddDays(15),
            CreatedAt = DateTime.UtcNow

        };
    }
    public async Task<ResponseDTO<LoginResponseDTO>> LoginUserAsync(LoginRequestDTO request)
    {   List<string> Errors = new List<string>();
        var traceId = _accessor.HttpContext?.TraceIdentifier??"unknown";
        User user = await _user.Find(u=>u.UserName==request.UserName).FirstOrDefaultAsync();
        if(user==null){
             throw new NotFoundException(traceId,$"User with UserName '{request.UserName} not found'");}
        else{
            if(!user.EmailVerified) {
                // throw new BadRequestExceptionII("Something went wrong");
                throw new ForbiddenException(traceId,"Please verify your email first");
                }
            else{
            if(!user.VerifyPassword(user,request.Password)){
                Errors.Add("Password does not match");
            }
           
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            // return $"accessToken: {accessToken}, refreshToken: {refreshToken}";
            return new ResponseDTO<LoginResponseDTO>( true,"User Logged In Successfully",new LoginResponseDTO(accessToken,refreshToken.Token));
       
        }}

        throw new BadRequestException(traceId,"Errors:",Errors);
        }
    
    public async Task<ResponseDTO<RegisterResponseDTO>> RegisterUserAsync(RegisterRequestDTO registerDTO)
    {   
        var token = Guid.NewGuid().ToString();
        var traceId = _accessor.HttpContext?.TraceIdentifier??"unknown";
        var user = await _user.Find(u=> u.UserName==registerDTO.UserName).FirstOrDefaultAsync();
        if(user!=null) throw new BadRequestException(traceId,"Errors: ",[$"User with UserName '{registerDTO.UserName}' already exists. Please try again with another username"]);
        var newUser = new User{
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            UserName = registerDTO.UserName,
            Email = registerDTO.Email,
            EmailVerificationToken = token,
            EmailVerified = false,
            EmailTokenExpiryTime = DateTime.UtcNow.AddHours(3),
            

        };
        newUser.CreatePasswordHash(registerDTO.Password);
        await _user.InsertOneAsync(newUser);
        var verificationLink = $"http://localhost:5176/api/auth/verify-email?token={token}";
        var body = $@"
            <p>Hello</p>
            <p>Please click the link below for email verification:</p>
            <a href={verificationLink}>Verify Email</a>
            <p>This link will expire in 3 hours</p>
        ";
         await _emailService.SendEmailAsync(newUser.Email,"Verify your email",body);
        return new ResponseDTO<RegisterResponseDTO>(true, "User registered successfully but please verify your email to continue.", new RegisterResponseDTO(true));


    
        
    }

    public async Task<bool> VerifyEmail(
        string token)
    {  var filter = Builders<User>.Filter.Eq(u=> u.EmailVerificationToken, token); 
       var user = await _user.Find(filter).FirstOrDefaultAsync();
       if(user==null||user.EmailTokenExpiryTime < DateTime.UtcNow){
            return false;
       }
       
       var update = Builders<User>.Update
        .Set(u=>u.EmailVerificationToken,null)
        .Set(u=>u.EmailVerified,true)
        .Set(u=>u.EmailTokenExpiryTime,null);
    //    user.EmailVerificationToken = null;
    //    user.EmailVerified = true;
    //    user.TokenExpiryTime = null;
       await _user.UpdateOneAsync(filter,update);
       return true; 
    }

    // public bool isRefreshTokenValid(string refreshToken){
    //    var user = _user.Find(u=>u.RefreshTokens.Any(rt=> rt.Token == refreshToken)).FirstOrDefault();
    //    if(user == null) return false;
       
    // }
    public async Task<ResponseDTO<LoginResponseDTO>> RefreshSession(string refreshToken){
        List<string> Errors = new List<string>();
        var traceId = _accessor.HttpContext?.TraceIdentifier??"unknown";
        var user = _user.Find(u=> u.RefreshTokens.Any(rt=>rt.Token==refreshToken)).FirstOrDefault();
        if(user==null){
            throw new UnAuthorizedException(traceId,"Given Refresh Token does not exist");
        }
        var existingRefreshToken = user.RefreshTokens.FirstOrDefault(rt=> rt.Token==refreshToken);
        if(existingRefreshToken==null ||!existingRefreshToken.IsActive){
            throw new UnAuthorizedException(traceId,"Given Refresh Token not valid");
        }
        existingRefreshToken.Revoked = DateTime.UtcNow;
        user.RefreshTokens = [.. user.RefreshTokens.Where(rt=> rt.IsActive)];
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        var newAccessToken = GenerateAccessToken(user);
        await _user.ReplaceOneAsync(u=>u.Id==user.Id,user);
        return new ResponseDTO<LoginResponseDTO>(true,"Refresh session successfull!! User logged in successfully",new LoginResponseDTO(newAccessToken,newRefreshToken.Token));
        }

}