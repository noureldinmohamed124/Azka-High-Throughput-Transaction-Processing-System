using Azka_Transaction_Processing_System.Infrastructure.Presistance;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();



// 1. DbContext
builder.Services.AddDbContext<TPSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AspTest")));












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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



/*
 
    Install-Package Microsoft.EntityFrameworkCore -version 8.0.20 (Infrastructure)
    Install-Package Microsoft.EntityFrameworkCore.Tools -version 8.0.20 (Infrastructure)
    Install-Package Microsoft.EntityFrameworkCore.SqlServer -version 8.0.20 (Infrastructure)
    Install-Package Microsoft.AspNetCore.Cryptography.KeyDerivation -version 8.0.20 (Infrastructure)
    Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -version 8.0.20 (API)
    Install-Package Microsoft.EntityFrameworkCore.Design -version 8.0.20 (API)
    

    Add-Migration InitialCreate -OutputDir Presistance/Migrations (مش لازم تعملوها)
 

*/