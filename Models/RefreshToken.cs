namespace TodoApi.Models;
public class RefreshToken{
    public string Token {get; set;} = string.Empty;
    public DateTime Expires{get; set;}
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime CreatedAt{get; set;}
    public DateTime? Revoked{get; set;}
    public string? ReplacedByToken{get; set;}
    public bool IsRevoked => Revoked!=null;
    public bool IsActive => !IsRevoked && !IsExpired;
}