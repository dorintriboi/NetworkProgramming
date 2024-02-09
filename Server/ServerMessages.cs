using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class ServerMessages
{
    private static List<Socket> _activeConnections = new List<Socket>();

    public static Socket CreateSocket(string address, int portNumber, int maxConnection)
    {
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress ipAddress = IPAddress.Parse(address);

        serverSocket.Bind(new IPEndPoint(ipAddress, portNumber));

        serverSocket.Listen(maxConnection);

        return serverSocket;
    }

    public static void AddConnection(Socket connection)
    {
        _activeConnections.Add(connection);
        Console.WriteLine($"Task secundar: Conexiuni curente {_activeConnections.Count}");
    }

    public static async Task GetAndSendMessageAsync(Socket connection)
    {
        bool ok = true;
        while (ok)
        {
            string message = "";
            do
            {
                byte[] buffer = new byte[1024];

                int bytesReceived = 0;

                try
                {
                    bytesReceived = await connection.ReceiveAsync(buffer);
                    if (bytesReceived == 0)
                    {
                        ok = false;
                    }
                }
                catch (Exception e)
                {
                    ok = false;
                }

                message += Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            } while (connection.Available > 0);

            if (ok == false)
            {
                break;
            }
            
            Console.WriteLine($"Task secundar: Am primit mesajul {message}");

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            foreach (var activeConnection in _activeConnections)
            {
                try
                {
                    await activeConnection.SendAsync(messageBytes);
                }
                catch (Exception e)
                {
                    _activeConnections.Remove(activeConnection);
                    Console.WriteLine("Task secundar: am inchis o conexiune conexiunea");
                }
            }
            
            Console.WriteLine($"Task secundar: Am trimis mesajul {message}");
        }

        Console.WriteLine(
            $"Task secundar: Conexiuna a fost inchisa ({connection.RemoteEndPoint})");
        _activeConnections.Remove(connection);

        Console.WriteLine($"Task secundar: Conexiuni curente {_activeConnections.Count}");
    }
}