

using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sample_net6_api_service.Models;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "3000"; // grab port from ENV; if not present, set to 3000


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(hc => new HttpClient { BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon/") });

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Logger.LogInformation($"Listening on {port}!"); // drop a line that the server is listening at startup!
app.Urls.Add("http://0.0.0.0:3000"); // Ensures the service listens on ALL ports and works in a container

app.MapGet("/health", () =>
{
    return Results.Ok("Sample .NET6 service is healthy!"); // Minimal API version of Ok();
});


app.MapGet("/api/echo", (string? term) =>
{
    return Results.Ok($"You said: {term}"!);
});


app.MapGet("/api/env", () =>
{
    /** 
     * When I run the app inside of Visual Studio, it does not detect that I exported environment variable ENV 
     * the only way to make this work is to run the dll via `dotnet` command. When it does this way .NET
     * picks up the env variables set within the shell that the dll was invoked in
     * **/

    //Environment.SetEnvironmentVariable("ENV","Unclassified",EnvironmentVariableTarget.Process);
    //var environment = Environment.GetEnvironmentVariable("ENV",EnvironmentVariableTarget.Process);

    var environment = Environment.GetEnvironmentVariable("ENV");
    Console.WriteLine($"Environment Var => {environment}");

    return Results.Ok($"Looks like you are running in an {environment} environment!");
});


app.MapGet("/api/kubernetes", () =>
{
    var nodeName = Environment.GetEnvironmentVariable("NODE_NAME");
    var podName = Environment.GetEnvironmentVariable("POD_NAME");
    var podIp = Environment.GetEnvironmentVariable("POD_IP");
    var podNamespace = Environment.GetEnvironmentVariable("POD_NAMESPACE");
    var podServiceAccount = Environment.GetEnvironmentVariable("POD_SERVICE");

    if(nodeName == null)
    {
        return Results.Problem("It does not look like you are running in a Kubernetes Environment!");

    }

    PodMetadada metadata = new PodMetadada
    {
        Node = nodeName,
        PodName = podName,
        PodIP = podIp,
        PodNamespace = podNamespace,
        PodServiceAccount = podServiceAccount

    };

    return Results.Ok(metadata);

});


app.MapPost("/api/sum", ([FromBody] Sum sum) =>
{
    var total = sum.val_1 + sum.val_2;
    return Results.Ok(total);

});


// async example w/ deserialization into a Pokemon object
app.MapGet("/api/pokemon", async (HttpClient http) =>
{
    var response = await http.GetFromJsonAsync<Pokemon>("https://pokeapi.co/api/v2/pokemon/ditto");
    Console.WriteLine(response.Id);
    return Results.Ok(response);
});


// passing a url parameter and using it in the handler
app.MapGet("/api/square/{num}", (int num) =>
{

    var result = 2 * num;
    Console.WriteLine("Done the math. Here is the result!");
    return Results.Ok($"{num} squared equals {result}!"); // string interpolation
});

//app.Run($"http://localhost:{port}"); // explicitly define your own port to listen on; it will be respected at start up
app.Run();
