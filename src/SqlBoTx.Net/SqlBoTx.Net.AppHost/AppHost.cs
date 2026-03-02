var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SqlBoTx_Net_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

var dbManager = builder.AddProject<Projects.SqlBoTx_Net_DbManager>("dbManager");

builder.Build().Run();
