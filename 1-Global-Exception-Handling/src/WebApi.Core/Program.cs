using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApi.Data;
using WebApi.Service;
using WebApi.Core.SharedFramework;

var builder = WebApplication.CreateBuilder(args);

#region --- SQL Connection ---
var Database = "WindowMSSQL";
var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(
    builder.Configuration.GetConnectionString(Database) ?? throw new InvalidOperationException("Connection to '" + Database + "' cannot be establish.")
);

sqlConnectionStringBuilder.UserID = builder.Configuration["Database:" + Database + ":User"];
sqlConnectionStringBuilder.Password = builder.Configuration["Database:" + Database + ":Password"];

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString,
            opt => opt.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: System.TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)
        );
    });
#endregion

#region --- ASP.NET Identity ---
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();         // To supports MFA TOTP (Time-based One-time Password)


builder.Services
    .Configure<IdentityOptions>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(365250, 0, 0, 0); // 1000 years
        //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
        options.User.RequireUniqueEmail = true;
    });
#endregion

builder.Services.AddScoped<IProductsService, ProductsService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();