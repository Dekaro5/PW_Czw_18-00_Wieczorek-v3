using System;
using System.ComponentModel;
using System.Windows.Input;
using Presentation.Models;

namespace ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _ballCount = 10;
        private CancellationTokenSource _simulationCts;
        private Task _simulationTask;
        private readonly Model _model;

        public Model ModelClass => _model;

        public MainViewModel(int initialBallCount = 10)
        {
            _model = new Model(500, 300); // Domyślne wymiary stołu
            BallCount = initialBallCount;

            // Komendy
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

        // Komendy
        public ICommand CreateBallsCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        // Inicjalizacja kul
        private async Task InitializeBalls(int count)
        {
            await _model.InitializeBalls(count);
        }

        // Rozpoczęcie symulacji
        public async Task StartSimulation()
        {
            // Kończy poprzednią symulację, jeśli jeszcze trwa
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
                    _simulationCts?.Dispose(); // Zwalnia zasoby
                }
            }

            // Inicjalizuj kule
            await InitializeBalls(BallCount);

            // Tworzenie i uruchamianie zadania symulacji
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

        // Aktualizacja symulacji
        public void UpdateSimulation()
        {
            _model.UpdateBalls();
        }

        // Zatrzymanie symulacji
        public void StopSimulation()
        {
            _simulationCts?.Cancel();
        }

        // Powiadomienie o zmianach w modelu
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
