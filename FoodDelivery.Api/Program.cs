using FoodDelivery.Api.Data;
using FoodDelivery.Api.Data.Candidatures;
using FoodDelivery.Api.Data.Chefs;
using FoodDelivery.Api.Data.Restaurants;
using FoodDelivery.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("FoodDelivery");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string is missing. Set ConnectionStrings__FoodDelivery or user secrets.");
}

builder.Services.AddControllers();
builder.Services.AddDbContext<FoodDeliveryDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IChefRepository, ChefRepository>();
builder.Services.AddScoped<ICandidatureRepository, CandidatureRepository>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IChefService, ChefService>();
builder.Services.AddScoped<ICandidatureService, CandidatureService>();
builder.Services.AddScoped<IChefProfileService, ChefProfileService>();
builder.Services.AddScoped<IChefCandidatureService, ChefCandidatureService>();
builder.Services.AddScoped<IRestaurantSearchService, RestaurantSearchService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
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

// Log database connection failures with application context without exposing credentials.
app.Logger.LogInformation("FoodDelivery API starting with PostgreSQL-backed restaurant services");

app.Run();
