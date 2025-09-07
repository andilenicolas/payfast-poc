using payfast.integration.poc.Models;
using payfast.integration.poc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PayFast
builder.Services.Configure<PayFastConfig>(
    builder.Configuration.GetSection("PayFast"));

// Add HTTP Client
builder.Services.AddHttpClient<IPayFastService, PayFastService>();

// Register PayFast Service
builder.Services.AddScoped<IPayFastService, PayFastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable static files
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
