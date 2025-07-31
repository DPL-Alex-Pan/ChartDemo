using System;
using System.Threading.Tasks;

namespace ChartDemo.Models
{
    public interface IWsServiceProvider
    {
        Task Provide(string wsConnection, Action<object> func);
        Task Send(string message);
    }
}
