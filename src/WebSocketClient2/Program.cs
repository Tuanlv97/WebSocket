using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using WebSocketClient2;

var ws = new ClientWebSocket();
string name;
string jwt;
string secret = "C421AAEE0D114E9C";

while (true)
{
    Console.Write("Input name Client 2: ");
    name = Console.ReadLine();
    string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJtZXNzYWdlIjoiSldUIFJ1bGVzISIsImlhdCI6MTQ1OTQ0ODExOSwiZXhwIjoxNDU5NDU0NTE5fQ.-yIVBD5b73C75osbmwwshQNRC7frWUYrqaTjTpza2y4\r\n";
    jwt = "Bearer " + token;
    break;
}


Console.WriteLine("Connecting to server");

var request = new RequestModel
{
    Authorization = jwt,
    Contents = "Thong tin dien thoai quet Qr bao gom anh va dien thoai",
    Secret = secret,
    SocketIdPc = "2e59377e-8409-43da-b0b6-f13ef045ac83"
};

var requestModel = JsonConvert.SerializeObject(request);

var url = new Uri("ws://localhost:7000/ws");
ws.Options.SetRequestHeader("Content-Type", "application/json");
ws.Options.SetRequestHeader("data", requestModel);
ws.Options.SetRequestHeader("Authorization", jwt);

await ws.ConnectAsync(url, CancellationToken.None);

Console.WriteLine("Connected!");

var receiveTask = Task.Run(async () =>
{
    var buffer = new byte[1024 * 4];
    while (true)
    {
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            break;
        }

        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine(message);
    }
});


var sendTask = Task.Run(async () =>
{
    while (true)
    {
        var message = Console.ReadLine();

        request.Contents = message;
        var requestModel = JsonConvert.SerializeObject(request);


        var bytes = Encoding.UTF8.GetBytes(requestModel);

        if (message == "exit")
        {
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Close, true, CancellationToken.None);
            break;
        }
        await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
});

await Task.WhenAny(sendTask, receiveTask);

if (ws.State != WebSocketState.Closed)
{
    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
}

await Task.WhenAll(sendTask, receiveTask);