var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.EventManagement_Api>("api");

// builder.AddProject<Projects.EventRegistrationSystem>("legacy-mvc")
//            .WithEndpoint("http", endpoint => {

//                endpoint.Port = 44372; // Match Kestrel's HTTP port
//                endpoint.IsProxied = false; // Bypass Aspire's reverse proxy if needed
//                endpoint.UriScheme = "https";
//            });

builder.Build().Run();
