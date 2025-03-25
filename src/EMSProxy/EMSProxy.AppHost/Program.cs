var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.EMSProxy_ApiService>("apiservice");

builder.AddProject<Projects.EMSProxy_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
