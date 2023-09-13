using HotelListing.API.Configuration;
using HotelListing.API.Data;
using HotelListing.API.Entity;
using HotelListing.API.Repository.Implimentation;
using HotelListing.API.Repository.IRepostitory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CORS Configurtaion
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowAll", b =>
    b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

//Serilog Configuration
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));


//Database Connection
builder.Services.AddDbContext<HotelListingDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("HotelListingDbConnectionString"));
});

builder.Services.AddIdentityCore<ApplicationUser>()
      .AddRoles<IdentityRole>()
      .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("HotelListingApi")
     .AddEntityFrameworkStores<HotelListingDbContext>()
     .AddDefaultTokenProviders();

//Adding AutoMapper
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IHotelsRepository, HotelsRepository>();
builder.Services.AddScoped<IAuthManager, AuthManager>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;// gives use defualt bearer constant "Bearer "
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.MapPost("/", () =>
//{

//}).WithName("Home");
app.UseSerilogRequestLogging(); //Logs the requests and reponse time
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
