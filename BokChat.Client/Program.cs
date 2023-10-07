

using BokChat.Server;
using BokChat.Server.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<Greeter.GreeterClient>(o =>
{
    o.Address = new Uri("https://localhost:7240");
});
builder.Services.AddGrpcClient<ChatMessage.ChatMessageClient>(o =>
{
    o.Address = new Uri("https://localhost:7240");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnableTryItOutByDefault();
        options.EnablePersistAuthorization();
        options.EnableFilter();
        options.EnableValidator();
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("say-hello", async (Greeter.GreeterClient client) =>
{
    var reply = await client.SayHelloAsync(new HelloRequest { Name = "Thai" });
    return TypedResults.Ok(reply.Message);
});

app.MapGet("get-message", async (ChatMessage.ChatMessageClient client) =>
{
    var reply = await client.GetChatMessageAsync(new ChatMessageRequest { UserId = "Thai" });
    return TypedResults.Ok(reply.Content);
});

app.MapGet("get-messages", async (ChatMessage.ChatMessageClient client, CancellationToken cancellationToken) =>
{
    using var call = client.GetChatMessages(new ChatMessagesRequest { UserId = "Thai" }, cancellationToken: cancellationToken);

    var messages = new List<ChatMessageResponse>();

    while (await call.ResponseStream.MoveNext(cancellationToken))
    {
        var currentMessage = call.ResponseStream.Current;
        messages.Add(currentMessage);
    }
    return TypedResults.Ok(messages);
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
