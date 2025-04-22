namespace TodoApi.DTOs;
public class RegisterResponseDTO{
    public bool IsVerificationEmailSent{get; set;}

    public RegisterResponseDTO(bool isVerificationEmailSent){
        IsVerificationEmailSent = isVerificationEmailSent;
    }
}