namespace TodoApi.Models;
public class RefreshToken{
    public string Token {get; set;} = string.Empty;
    public DateTime Expires{get; set;}
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime CreatedAt{get; set;}
    public bool IsActive => !IsExpired;
}