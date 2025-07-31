using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChartDemo.Models
{
    public class WsServiceProvider : IWsServiceProvider
    {
        const int numberOfPoints = 19_000_000;
        private Action<object>? _func;
        ClientWebSocket _clientWebSocket = new ClientWebSocket();
        ArraySegment<byte> _receiveBuffer = new(new byte[numberOfPoints]);
        CancellationTokenSource _cts = new CancellationTokenSource();
        public async Task Provide(string wsConnection, Action<object> func)
        {
            await _clientWebSocket.ConnectAsync(new Uri(wsConnection), _cts.Token);
            _func = func;

            _ = Task.Run(async () => 
            {
                while (_clientWebSocket.State == WebSocketState.Open
                    && _cts.IsCancellationRequested == false)
                {
                    for (int i = 0; i < numberOfPoints; i++)
                        _receiveBuffer[i] = (byte)' ';

                    WebSocketReceiveResult result = await _clientWebSocket.ReceiveAsync(_receiveBuffer, _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(_receiveBuffer.Array, 0, result.Count);
                        _func?.Invoke(receivedMessage);

                        Console.WriteLine($"Received message from server: {receivedMessage}");
                    }
                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        if (result.EndOfMessage == true)
                        {
                            try
                            {
                                var data = UTF8Encoding.UTF8.GetString(_receiveBuffer).TrimEnd();
                                var array = JsonConvert.DeserializeObject<double[]>(data);
                                _func?.Invoke(array);
                                
                            }
                            catch(Exception x)
                            {
                                ;
                            }
                            
                        }
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
