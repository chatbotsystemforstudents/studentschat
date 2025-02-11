using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tcp
{
    public class Client
    {
        private readonly ILogger<Client> _logger;
        public Client(ILogger<Client> logger)
        {
            _logger = logger;
        }
        public async Task<bool> Send(TcpRequest tcpRequest,string serverIp,int serverPort)
        {
            try
            {
                // Create a TCP client and connect asynchronously
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(serverIp, serverPort);
                    _logger.LogInformation($"Connected to server {serverIp}:{serverPort}");

                    // Get the network stream
                    using (NetworkStream stream = client.GetStream())
                    {
                        // serialize the TcpRequest object to a JSON string
                        string payload = JsonSerializer.Serialize(tcpRequest);
                        byte[] data = Encoding.UTF8.GetBytes(payload);

                        // Send the payload asynchronously
                        await stream.WriteAsync(data, 0, data.Length);
                        _logger.LogInformation($"Payload sent: {payload}");                        

                        // Optionally, receive a response
                        byte[] buffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        _logger.LogInformation($"Response from server: {response}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while sending data to server {serverIp}:{serverPort}");
                return false;
            }
        }
    }
}
