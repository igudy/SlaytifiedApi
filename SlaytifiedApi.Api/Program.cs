using Microsoft.EntityFrameworkCore;
using SlaytifiedApi.Application.Interfaces;
using SlaytifiedApi.Application.Services;
using SlaytifiedApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add PostgreSQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services (Make sure to update this if there are any changes in the service interfaces)
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
