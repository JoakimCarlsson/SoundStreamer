using SoundStreamer.Server;

var server = new TcpServer(); 
server.Initialize();
server.StartConnectionLoop();
server.StartMessageLoop();
Console.ReadLine();