

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

app.Logger.LogInformation($"Listening on {port}!"); // drop a line that the server is listening at startup!S

app.MapGet("/health", () =>
{
    return Results.Ok("Sample .NET6 service is healthy!"); // Minimal API version of Ok();
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

app.Run($"http://localhost:{port}"); // explicitly define your own port to listen on; it will be respected at start up


public class Pokemon
{
    public int Id { get; set; }
    public string Name { get; set; }    

}