using System.Net;

namespace WSServer
{
    internal class Program
    {
        HttpListener HttpListener { get; }= new HttpListener();
        static void Main(string[] args)
        {
            _ = Task.Run(() =>
            {

            });
        }
    }
}
