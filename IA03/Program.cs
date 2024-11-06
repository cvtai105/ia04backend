using IA03.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IA03.Services;
using IA03.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<JwtService>();

// Retrieve CORS origins from configuration
var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomPolicy", policy =>
        policy.WithOrigins(allowedOrigins ?? ["http://localhost:3000"])
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//             ValidAudience = builder.Configuration["JwtSettings:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]??string.Empty)),
//             ClockSkew = TimeSpan.Zero // remove delay of token when expire
//         };
//     });
// builder.Services.AddAuthorization();
    
var app = builder.Build();  

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CustomPolicy");
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddeware>();
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/profile"), appBuilder =>
{
    appBuilder.UseMiddleware<JwtAuthenticationMiddleware>();
});
// app.UseAuthentication();
// app.UseAuthorization();
app.Run();

