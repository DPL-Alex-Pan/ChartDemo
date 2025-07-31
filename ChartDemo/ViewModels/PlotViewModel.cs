using ScottPlot.Avalonia;

namespace ChartDemo.ViewModels
{
    public partial class PlotViewModel : ViewModelBase
    {
        private bool _isInit;
        public PlotViewModel()
        {
            
        }

        
        public AvaPlot? Plot 
        {
            get => _plot;
            set
            {
                _plot ??= value;
            }
        }
        private double[]? _plotData;
        private AvaPlot? _plot;

        public double[]? PlotData
        {
            get => _plotData;
            set
            {
                SetProperty(ref _plotData, value);
                RenderData();
            }
        }

        private void RenderData()
        {
            if(Plot is null || PlotData is null) return;

            lock (Plot.Plot.Sync)
            {
                Plot.Plot.Clear();
                Plot.Plot.Add.Signal(PlotData);

                Plot.Refresh();
            }
            if(!_isInit)
                Plot.Plot.Axes.AutoScaleX();

            _isInit = true;
        }
    }
}
