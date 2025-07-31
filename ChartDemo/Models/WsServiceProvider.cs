using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChartDemo.Models
{
    public class WsServiceProvider : IWsServiceProvider
    {
        private Action<string>? _func;
        ClientWebSocket _clientWebSocket = new ClientWebSocket();
        byte[] _receiveBuffer = new byte[1024];
        CancellationTokenSource _cts = new CancellationTokenSource();
        public async Task Provide(string wsConnection, Action<string> func)
        {
            await _clientWebSocket.ConnectAsync(new Uri(wsConnection), _cts.Token);
            _func = func;

            _ = Task.Run(async () => 
            {
                while (_clientWebSocket.State == WebSocketState.Open
                    && _cts.IsCancellationRequested == false)
                {
                    WebSocketReceiveResult result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(_receiveBuffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(_receiveBuffer, 0, result.Count);
                        _func?.Invoke(receivedMessage);

                        Console.WriteLine($"Received message from server: {receivedMessage}");
                    }
                }
            });
        }

        public async Task Send(string message)
        {
            await _clientWebSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, _cts.Token);
        }
    }
}
