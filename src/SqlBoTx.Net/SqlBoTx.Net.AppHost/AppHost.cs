var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SqlBoTx_Net_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");
 

builder.Build().Run();
