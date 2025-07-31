using Avalonia.Threading;
using ChartDemo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ChartDemo.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider? _provider;
        private readonly IWsServiceProvider _wsProvider;

        public MainWindowViewModel()
        {
            
        }

        [ObservableProperty]
        private string greeting = "Welcome to Avalonia!";
        public MainWindowViewModel(IServiceProvider provider)
        {
            _provider = provider;
            if(_provider is not null)
            {
                _wsProvider = provider.GetService<IWsServiceProvider>() 
                    ?? throw new ArgumentNullException(nameof(_wsProvider));
            }
            
        }

        [RelayCommand]
        private async Task Init()
        {
            await Initialize();
        }

        [RelayCommand]
        private async Task Send(string message)
        {
            await SendMessage(message);
        }

        public async Task SendMessage(string message)
            => await _wsProvider.Send(message);
        public async Task Initialize()
        {
            if (_wsProvider is null) return;

            await _wsProvider.Provide("ws://localhost:8080/ws/", (message) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Greeting = message;
                });
            });
        }
    }
}
