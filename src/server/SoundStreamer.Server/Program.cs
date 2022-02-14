using SoundStreamer.Server;

var server = new TcpServer();
await server.InitializeAsync();
server.StartConnectionLoop();
server.StartMessageLoop();
Console.ReadLine();