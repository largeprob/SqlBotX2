using Newtonsoft.Json.Serialization;
using SqlBoTx.Net.ApiService;
using SqlBoTx.Net.ApiService.SqlPlugin;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        );
    });


builder.Services.AddScoped<SqlServerDatabaseService>();
builder.AddKernelCompletion();

builder.Services.AddTransient<SqlBotPlugin>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FirstAgent", builder =>
    {
        builder.WithOrigins("http://localhost:5173", "https://ai.largeprob.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.ChatApi().SystemApi();



// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi.json");
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi.json", "v1");
    });
}

app.MapDefaultEndpoints();

app.UseCors("FirstAgent");
app.MapControllers();
app.Run();

 