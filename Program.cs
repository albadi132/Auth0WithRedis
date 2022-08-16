using Auth0WithRedis.Data;
using Auth0WithRedis.Services.Redis;
using Auth0WithRedis.Services.Tokens;
using Auth0WithRedis.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/* Database Conaction */
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<DimoDBContext>(options =>
    options.UseSqlServer(connectionString));

/* End Database Conaction */

/**/
var Redis = builder.Configuration["RedisCacheUrl"] ?? throw new InvalidOperationException("Redis Url not found.");
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = Redis; });
/**/

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Jwt */
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
/* Jwt */

/* Interfaces */

builder.Services.AddTransient<IUserServices, UserServices>();
builder.Services.AddTransient<ITokenServices, TokenServices>();
builder.Services.AddTransient<IRedisServices, RedisServices>();
/* End Interfaces */




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


app.Run();
