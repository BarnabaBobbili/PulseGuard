using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using PulseGuard.Models;
using PulseGuard.Services;
using SkiaSharp;

namespace PulseGuard.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly VitalSimulatorService _simulator = new();
        private readonly AlertSystem _alertSystem = new();

        private CancellationTokenSource? _cts;
        private Task? _simulationTask;
        private readonly Dispatcher _dispatcher = Application.Current.Dispatcher;

        private readonly DispatcherTimer _flashTimer;
        private bool _flashState = false;

        private readonly ObservableCollection<ObservableValue> _ecgValues = new();
        private readonly ObservableCollection<ObservableValue> _spo2Values = new();

        public ISeries[] EcgSeries { get; }
        public ISeries[] SpO2Series { get; }

        public Axis[] EcgYAxes { get; } = new Axis[]
        {
            new Axis { MinLimit = -3, MaxLimit = 4, IsVisible = false }
        };

        public Axis[] SpO2YAxes { get; } = new Axis[]
        {
            new Axis
            {
                MinLimit = 80,
                MaxLimit = 100,
                LabelsPaint = new SolidColorPaint(SKColors.White),
                TextSize = 11
            }
        };

        public Axis[] HiddenXAxes { get; } = new Axis[]
        {
            new Axis { IsVisible = false }
        };

        [ObservableProperty] private int _heartRateBpm = 75;
        [ObservableProperty] private double _spO2Percentage = 97.0;
        [ObservableProperty] private string _alertMessage = "✓ All Vitals Normal";
        [ObservableProperty] private bool _isAlerting = false;
        [ObservableProperty] private bool _isRunning = false;
        [ObservableProperty] private string _startStopButtonText = "▶  Start Monitoring";

        [ObservableProperty]
        private SolidColorBrush _alertBarBrush = new SolidColorBrush(Color.FromRgb(26, 71, 42));

        public string PatientName { get; } = "John Mitchell";
        public string PatientAge { get; } = "Age: 52";
        public string PatientBed { get; } = "Bed: ICU-04";
        public string PatientId { get; } = "ID: #PM-00142";

        public MainViewModel()
        {
            for (int i = 0; i < 120; i++)
            {
                _ecgValues.Add(new ObservableValue(0));
                _spo2Values.Add(new ObservableValue(97));
            }

            EcgSeries = new ISeries[]
            {
                new LineSeries<ObservableValue>
                {
                    Values = _ecgValues,
                    Stroke = new SolidColorPaint(SKColors.LimeGreen, 2),
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3,
                    AnimationsSpeed = TimeSpan.Zero,
                    Name = "ECG"
                }
            };

            SpO2Series = new ISeries[]
            {
                new LineSeries<ObservableValue>
                {
                    Values = _spo2Values,
                    Stroke = new SolidColorPaint(new SKColor(0, 191, 255), 2),
                    Fill = new LinearGradientPaint(
                        new SKColor(0, 191, 255, 60),
                        new SKColor(0, 191, 255, 0),
                        new SKPoint(0.5f, 0),
                        new SKPoint(0.5f, 1)
                    ),
                    GeometrySize = 0,
                    LineSmoothness = 0.5,
                    AnimationsSpeed = TimeSpan.Zero,
                    Name = "SpO2"
                }
            };

            _flashTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _flashTimer.Tick += OnFlashTimerTick;
        }

        [RelayCommand]
        private void StartStop()
        {
            if (IsRunning)
                StopSimulation();
            else
                StartSimulation();
        }

        private void StartSimulation()
        {
            IsRunning = true;
            StartStopButtonText = "⏹  Stop Monitoring";

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            _simulationTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    VitalRecord record = _simulator.GenerateNext();
                    AlertState alert = _alertSystem.Evaluate(record);

                    await _dispatcher.InvokeAsync(() =>
                    {
                        UpdateCharts(record);
                        UpdateDisplayValues(record, alert);
                    }, DispatcherPriority.Render);

                    await Task.Delay(16, token);
                }
            }, token);
        }

        private void StopSimulation()
        {
            _cts?.Cancel();
            IsRunning = false;
            StartStopButtonText = "▶  Start Monitoring";
            StopFlashing();
        }

        private void UpdateCharts(VitalRecord record)
        {
            _ecgValues.Add(new ObservableValue(record.EcgValue));
            if (_ecgValues.Count > 120) _ecgValues.RemoveAt(0);

            _spo2Values.Add(new ObservableValue(record.SpO2Value));
            if (_spo2Values.Count > 120) _spo2Values.RemoveAt(0);
        }

        private void UpdateDisplayValues(VitalRecord record, AlertState alert)
        {
            HeartRateBpm = record.HeartRateBpm;
            SpO2Percentage = record.SpO2Value;
            AlertMessage = alert.Message;
            IsAlerting = alert.IsAlerting;

            if (alert.IsAlerting) StartFlashing();
            else StopFlashing();
        }

        private void StartFlashing()
        {
            if (!_flashTimer.IsEnabled) _flashTimer.Start();
        }

        private void StopFlashing()
        {
            if (_flashTimer.IsEnabled) _flashTimer.Stop();
            AlertBarBrush = new SolidColorBrush(Color.FromRgb(26, 71, 42));
        }

        private void OnFlashTimerTick(object? sender, EventArgs e)
        {
            _flashState = !_flashState;
            AlertBarBrush = _flashState
                ? new SolidColorBrush(Color.FromRgb(220, 20, 20))
                : new SolidColorBrush(Color.FromRgb(100, 10, 10));
        }
    }
}
