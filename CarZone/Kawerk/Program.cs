using Kawerk.Application.Authorization;
using Kawerk.Application.Interfaces;
using Kawerk.Application.Services;
using Kawerk.Infastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IBranchSevice,BranchService>();
builder.Services.AddScoped<ISalesmanService, SalesmanService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITokenHandler, Kawerk.Application.Services.TokenHandler>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthorizationHandler, SameUserHandler>();

builder.Services.AddDbContext<DbBase>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:VpsConnection"]);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Token"]!)),
            ValidateIssuerSigningKey = true,

        };
    });
Console.WriteLine("Connection String = " + builder.Configuration["ConnectionStrings:VpsConnection"]);
Console.WriteLine("Issuer = " + builder.Configuration["JwtSettings:Issuer"]);
Console.WriteLine("Audience = " + builder.Configuration["JwtSettings:Audience"]);
Console.WriteLine("Token = " + (builder.Configuration["JwtSettings:Token"] != null));


builder.Services.ConfigureHttpJsonOptions(x =>
{
    x.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    x.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SameUserAuth", policy =>
        policy.Requirements.Add(new SameUserRequirement(allowAdmins: true)));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SalesmanPolicy", policy =>
        policy.RequireRole("Salesman", "BranchManager", "ManufacturerAdministrator", "Admin"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BranchPolicy", policy =>
        policy.RequireRole("BranchManager","Admin"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManufacturerPolicy", policy =>
        policy.RequireRole("ManufacturerAdministrator", "Admin"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllPolicy", policy =>
        policy.RequireRole("Customer","Salesman", "BranchManager", "ManufacturerAdministrator", "Admin"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<DbBase>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log but don’t crash startup
        Console.WriteLine($"Migration failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
