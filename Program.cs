using TodoApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApi.Exceptions;
using TodoApi.Services.ServiceImpl;
using TodoApi.Data;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      
              .AllowAnyMethod()       
              .AllowAnyHeader();      
    });
});
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<ApplicationDbContext>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmailService,SmtpEmailService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddControllers();
builder.Services.AddAuthentication(options=>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
)
.AddJwtBearer(options=>
{
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        // ValidIssuer = builder.Configuration["Jwt:Issuer"],
        // ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
    };
    options.Events = new JwtBearerEvents{
        OnAuthenticationFailed= context=>{
            var traceId = context.HttpContext.TraceIdentifier;
            throw new UnAuthorizedException(traceId,"Authentication Failed: Invalid token");
        },

        OnChallenge= context=>{
            if(!context.Handled){
                var traceId = context.HttpContext.TraceIdentifier;
                context.HandleResponse();
                throw new UnAuthorizedException(traceId,"Authentication Failed: Token missing or invalid token");
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.Run();


