using Avalonia.Threading;
using ChartDemo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace ChartDemo.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider? _provider;
        private readonly IWsServiceProvider _wsProvider;
        Timer  _timer = new(TimeSpan.FromMilliseconds(50));
        public MainWindowViewModel()
        {
            
        }

        [ObservableProperty]
        private string greeting = "Welcome to Avalonia!";
        private bool _isInitialize;

        public MainWindowViewModel(IServiceProvider provider)
        {
            _provider = provider;
            if(_provider is not null)
            {
                _wsProvider = provider.GetService<IWsServiceProvider>() 
                    ?? throw new ArgumentNullException(nameof(_wsProvider));
            }
            
            _timer.Elapsed += async (s, e) =>
            {
                await SendMessage("data");
            };
        }

        
        [RelayCommand]
        private async Task Send(string message)
        {
            await Initialize();
            await SendMessage(message);
        }

        public async Task SendMessage(string message)
            => await _wsProvider.Send(message);

        public async Task Initialize()
        {
            if (_wsProvider is null || _isInitialize ) return;

            await _wsProvider.Provide("ws://localhost:8080/ws/", (message) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if(message is string textMessage)
                        Greeting = textMessage;
                    else if (message is double[] array)
                    {
                        PlotModel.PlotData = array;
                    }
                });
            });

            _timer.Start();

            _isInitialize = true;
        }

        public PlotViewModel PlotModel { get; } = new PlotViewModel();
    }
}
