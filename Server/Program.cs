using Server;

var serverSocket = ServerMessages.CreateSocket("127.0.0.1", 9000, 5);

Console.WriteLine("Task principal: Serverul a fost creat");

while (true)
{
     await ServerConnections.AcceptConnectionsAsync(serverSocket);   
}
