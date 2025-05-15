using System;
using System.ComponentModel;
using System.Windows.Input;
using Presentation.Models;

namespace PresentationViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _ballCount = 10;
        private CancellationTokenSource _simulationCts;
        private Task _simulationTask;
        private readonly Model _model;

        public Model Model => _model;

        public MainViewModel(int initialBallCount = 10)
        {
            _model = new Model(500, 300);
            BallCount = initialBallCount;

            CreateBallsCommand = new RelayCommand(_ => _ = InitializeBalls(BallCount));
            StartCommand = new RelayCommand(_ => _ = StartSimulation());
            StopCommand = new RelayCommand(_ => StopSimulation());
        }

        public int BallCount
        {
            get => _ballCount;
            set
            {
                if (_ballCount != value && value > 0)
                {
                    _ballCount = value;
                    OnPropertyChanged(nameof(BallCount));
                }
            }
        }

        public ICommand CreateBallsCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        private async Task InitializeBalls(int count)
        {
            await _model.InitializeBalls(count);
        }

        public async Task StartSimulation()
        {
            if (_simulationTask != null && !_simulationTask.IsCompleted)
            {
                _simulationCts.Cancel();
                try
                {
                    await _simulationTask;
                }
                catch (OperationCanceledException) { }
                finally
                {
                    _simulationCts?.Dispose();
                }
            }

            _simulationCts = new CancellationTokenSource();
            CancellationToken token = _simulationCts.Token;

            _simulationTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    UpdateSimulation();
                    try
                    {
                        await Task.Delay(16, token); // ~60 FPS
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }, token);
        }

        public void UpdateSimulation()
        {
            _model.UpdateBalls();
        }

        public void StopSimulation()
        {
            _simulationCts?.Cancel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
