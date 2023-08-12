using WebSocketService.Middlewares;
using WebSocketService.Middlewares.Handlers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebSocketManager();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://localhost:7000");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();
var webSocketHandler = app.Services.GetService<WebSocketMessageHandler>();
app.MapWebSocketManager("/ws", webSocketHandler);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
