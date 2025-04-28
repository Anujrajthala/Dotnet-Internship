namespace TodoApi.Models;
using System.Security.Cryptography;
using System.Text.Encodings;
using MongoDB.Bson;
using  MongoDB.Bson.Serialization.Attributes;
public class User{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id{get; set;} = null!;

    [BsonElement("firstname")]
    public string FirstName{get; set;} = string.Empty;

    [BsonElement("lastname")]
    public string LastName{get; set;} = string.Empty;

    [BsonElement("username")]
    public string UserName{get; set;} = string.Empty;

    [BsonElement("passwordhash")]
    public byte[] PasswordHash{get; set;} = Array.Empty<byte>();

    [BsonElement("passwordsalt")]
    public byte[] PasswordSalt{get; set;} = Array.Empty<byte>();

    [BsonElement("email")]

    public string Email{get; set;} = string.Empty;

    [BsonElement("emailverified")]
    public bool EmailVerified{get; set;} = false;

    [BsonElement("emailverificationtoken")]
    public string? EmailVerificationToken{get; set;}

    [BsonElement("emailtokenexpirytime")]
    public DateTime? EmailTokenExpiryTime{get; set;}

    [BsonElement("roles")]
    public List<string> Roles{get; set;}= ["User"];

    [BsonElement("refreshtoken")]
    public RefreshToken? RefreshToken{get; set;}

    public void CreatePasswordHash(string password){
        byte[] salt = new byte[64];
        RandomNumberGenerator.Fill(salt);
        using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
        PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        PasswordSalt = salt;
    }

    public bool VerifyPassword(User user,string password){
        byte[] passwordHash = user.PasswordHash;
        byte[] passwordSalt= user.PasswordSalt;
        using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
        var requestPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return requestPasswordHash.SequenceEqual(passwordHash);
    }


} 
