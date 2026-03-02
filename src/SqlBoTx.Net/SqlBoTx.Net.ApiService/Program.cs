 
using SqlBoTx.Net.ApiService;
using SqlBoTx.Net.ApiService.SqlBotX;
using SqlBoTx.Net.ApiService.SqlPlugin;
using SqlBoTx.Net.Application;
using SqlBoTx.Net.Application.BusinessObjectives;
using SqlBoTx.Net.Core.Controller;
using SqlBoTx.Net.Core.ExceptionHandler;
using SqlBoTx.Net.Core.Exceptions;
using SqlBoTx.Net.Core.Startups;
using SqlBoTx.Net.Domain;
using SqlBoTx.Net.EFCore;
using SqlBoTx.Net.Share.Exceptions;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.SqlServer;


var builder = WebApplication.CreateBuilder(args);
// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// 异常处理
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

//统一验证服务 给Minimal API用的
builder.Services.AddValidation();
builder.Services.AddControllers();
//    .AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.Converters.Add(
//        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
//    );
//});


//Open Api 规范
builder.Services.AddOpenApi();

//Domain服务
builder.Services.AddDomainManagers();

//EFCore服务
builder.Services.AddEFCore(builder.Configuration);

//注册 Wolverine
builder.UseWolverine(opts =>
{
    string connectStr = builder.Configuration.GetConnectionString("SqlBotx").ThrowIfNull();
    opts.ApplicationAssembly = typeof(SqlBoTx.Net.Application.ApplicationExtensions).Assembly;

    //自动应用事务中间件
    //opts.Services.AddDbContextWithWolverineIntegration<SqlBotxDBContext>(x =>
    //{
    //    x.UseSqlServer(connectStr);
    //});
    //opts.Policies.AutoApplyTransactions();

    //手动应用事务中间件
    opts.PersistMessagesWithSqlServer(connectStr);
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.UseDurableLocalQueues();
});

//Application 服务
builder.Services.AddApplicationService();
 
builder.Services.AddScoped<SqlServerDatabaseService>();
builder.AddKernelCompletion();
builder.Services.AddTransient<SQLChatXService>();

builder.Services.AddTransient<SqlBotPlugin>();

//跨域
builder.Services.AddDefaultCors(builder.Configuration);
 
var app = builder.Build();

app.SystemApi();

// 异常处理
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
 
    });
}

app.MapDefaultEndpoints();
//跨域
app.UseDefaultCors();
app.MapControllers();
app.Run();

 