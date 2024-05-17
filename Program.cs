
using AspNetCoreRateLimit;
using CurrencyConverter.Services;
using CurrencyConverter.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = 100,
                    Period = "1m"
                }
            };
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICurrencyConverterService, CurrencyConverterService>();
builder.Services.AddScoped<IHttpAPIClient, HttpAPIClient>();
builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
