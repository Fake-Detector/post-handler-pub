using Fake.Detection.Post.Handler;
using Microsoft.AspNetCore.Hosting;

var builder = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(x => x.UseStartup<Startup>());

builder.Build().Run();