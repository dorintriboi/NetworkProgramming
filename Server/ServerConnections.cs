using System.Net.Sockets;

namespace Server;

public class ServerConnections
{
    public static async Task AcceptConnectionsAsync(Socket  currentSocket)
    {
        Socket connection = await currentSocket.AcceptAsync();
        
        Console.WriteLine("Task principal: O noua conexiune a fost creata");
        
        Task.Run(async () =>
        {
            ServerMessages.AddConnection(connection);
            await ServerMessages.GetAndSendMessageAsync(connection);
        });
    }
}