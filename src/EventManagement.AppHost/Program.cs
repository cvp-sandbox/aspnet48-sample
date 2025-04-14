var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EventRegistrationSystemCore>("eventregistrationsystemcore");

builder.AddProject<Projects.EventRegistrationSystem>("eventregistrationsystem")
           .WithEndpoint("http", endpoint => {
               
               endpoint.Port = 44372; // Match Kestrel's HTTP port
               endpoint.IsProxied = false; // Bypass Aspire's reverse proxy if needed
               endpoint.UriScheme = "https";
           });

builder.Build().Run();
