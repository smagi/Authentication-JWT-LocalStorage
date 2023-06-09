
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebAuth.Api.Entities;
using WebAuth.Api.Infrasturcture;
using WebAuth.Api.Services;

public class Programm
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApplicationDbContext>();

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddOptions<JwtTokenSettings>();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtTokenSettings = builder.Configuration
                .GetSection(nameof(JwtTokenSettings))
                .Get<JwtTokenSettings>() ?? throw new ArgumentNullException(nameof(JwtTokenSettings));

            //builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("JwtTokenSettings"));

            byte[] jwtSecretsBytes = jwtTokenSettings.GetSecretBytes();

            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtTokenSettings.ValidIssuer,
                ValidAudience = jwtTokenSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSecretsBytes),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddTransient<IClaimsService, ClaimsService>();
        builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

        builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection(nameof(JwtTokenSettings)));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) 
            .AllowCredentials());               

        app.Run();
    }
}