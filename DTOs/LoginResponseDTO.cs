namespace TodoApi.DTOs;
public class LoginResponseDTO{
    public string AccessToken{get; set;} = string.Empty;
    public string RefreshToken{get; set;} = string.Empty;

     public LoginResponseDTO(string accessToken,string refreshToken){
        AccessToken = accessToken;
        RefreshToken = refreshToken;

     }
   
     
}
