using FlightManegement.Data;
using FlightManegement.Interfaces;
using FlightManegement.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Sql server connect
builder.Services.AddDbContext<FlightManagementDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("flightmangagment"));
});
//Sql server connect
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IFlightService, FlightService>();
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
