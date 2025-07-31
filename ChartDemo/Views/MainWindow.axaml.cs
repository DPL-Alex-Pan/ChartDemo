using Avalonia;
using Avalonia.Controls;
using ChartDemo.ViewModels;
using System;

namespace ChartDemo.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            var dc = (MainWindowViewModel)DataContext;
            dc.PlotModel.Plot = this.AvaPlot;
            base.OnDataContextChanged(e);
        }
    }
}