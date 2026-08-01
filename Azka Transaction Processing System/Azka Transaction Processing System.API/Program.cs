using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Abstractions.Services;
using Azka_Transaction_Processing_System.Application.Modules.Transactions.CreateTransaction;
using Azka_Transaction_Processing_System.Infrastructure.Presistence;
using Azka_Transaction_Processing_System.Infrastructure.Repositories;
using Azka_Transaction_Processing_System.Infrastructure.Security;
using Azka_Transaction_Processing_System.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();


// 1. DbContext
builder.Services.AddDbContext<TPSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AspTest")));



// 2. Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepo<>),typeof(GenericRepo<>));
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<IBranchRepo, BranchRepo>();
builder.Services.AddScoped<IPaymentMethodRepo, PaymentMethodRepo>();
builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
builder.Services.AddScoped<IReceiptSequenceRepo, ReceiptSequenceRepo>();




// 3. Services
builder.Services.AddScoped<IReceiptGenerator, ReceiptGenerator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();



// 4. Use Cases
builder.Services.AddScoped<CreateTransactionUseCase>();



// 5. JWT Configurations
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey!))
    };
});



// 6. Cors Configurations
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("")
            .AllowAnyMethod().AllowAnyHeader();
    });
});



// 7. Enforce Enums to accept only String
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters
        .Add(new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
});




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Show Swagger Authorize Button
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Enterprise Transaction Processing API",
        Version = "v1",
        Description = "High-Throughput Transaction Processing System built with ASP.NET Core 8."
    });

    // JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = """
                      Enter your JWT token.

                      Example:
                      eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

                      ⚠️ Do NOT write 'Bearer'.
                      Swagger automatically adds it.
                      """,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



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



/*
 
    Install-Package Microsoft.EntityFrameworkCore -version 8.0.20 (Infrastructure)
    Install-Package Microsoft.EntityFrameworkCore.Tools -version 8.0.20 (Infrastructure)
    Install-Package Microsoft.EntityFrameworkCore.SqlServer -version 8.0.20 (Infrastructure)
    Install-Package Microsoft.AspNetCore.Cryptography.KeyDerivation -version 8.0.20 (Infrastructure)
    Install-Package Microsoft.AspNetCore.Http.Abstractions (Infrastructure)

    Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -version 8.0.20 (API)
    Install-Package Microsoft.EntityFrameworkCore.Design -version 8.0.20 (API)
    

    Add-Migration InitialCreate -OutputDir Presistence/Migrations (مش لازم تعملوها)
 

*/
