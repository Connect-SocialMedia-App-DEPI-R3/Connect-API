using Infrastructure.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Middleware;
using Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
    
    // Configure JWT for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
});



// Dependency Injection (AppDbContext)
// Repositories (Infrastructure)
builder.Services.AddInfrastructure(builder.Configuration);

// Services (Application)
builder.Services.AddApplication();

// SignalR
builder.Services.AddSignalR();

// Filters
builder.Services.AddScoped<ExtractUserIdFilter>();
builder.Services.AddScoped<ValidateModelFilter>();
// builder.Services.AddScoped<ValidateUserNotDeletedFilter>();

//  CORS (Allow local development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()     // frontend dev
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
    
    // CORS for SignalR
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "https://connect-depi.vercel.app")  
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // Required for SignalR
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseSwaggerConfiguration();
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("SignalRPolicy");  // Use SignalR CORS policy
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<Api.Hubs.ChatHub>("/chatHub");

// seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var context = services.GetRequiredService<AppDbContext>();

    await DbInitializer.SeedAsync(userManager, roleManager, context);
}
app.Run();
