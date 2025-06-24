using CattleBreedsApi.Data;
using CattleBreedsApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});
builder.Services.AddScoped<FileStorage>();
builder.Services.AddScoped<CattleClassifier>();
builder.Services.AddScoped<CattleClassifierApi>(provider =>
{
    var baseUrl = builder.Configuration.GetValue<string>("CattleBreedsApiSettings:BaseUrl");
    if (string.IsNullOrEmpty(baseUrl))
    {
        throw new InvalidOperationException("CattleBreedsApiSettings:BaseUrl configuration is missing.");
    }
    return new CattleClassifierApi(
        provider.GetRequiredService<ApiDbContext>(),
        provider.GetRequiredService<IHttpClientFactory>(),
        provider.GetRequiredService<FileStorage>(),
        baseUrl
    );
});
builder.Services.AddHttpClient();

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