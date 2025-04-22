using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using BusinessLogic.Abstractions;
using BusinessLogic.Services;
using Data.Models;

namespace Presentation
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IBallService    _ballService;
        private readonly DispatcherTimer _timer;

        // rozmiary Canvas:
        private const double CanvasWidth  = 500;
        private const double CanvasHeight = 300;

        private ObservableCollection<IBall> _balls = new();
        public ObservableCollection<IBall> Balls
        {
            get => _balls;
            set { _balls = value; OnPropertyChanged(); }
        }

        private int _ballCount;
        public int BallCount
        {
            get => _ballCount;
            set { _ballCount = value; OnPropertyChanged(); }
        }

        public ICommand CreateBallsCommand { get; }
        public ICommand StartCommand       { get; }

        public MainViewModel()
        {
            _ballService = new BallService();

            CreateBallsCommand = new RelayCommand(_ => CreateBalls());
            // Start zawsze aktywny – nie blokujemy CanExecute
            StartCommand       = new RelayCommand(_ => _timer.Start());

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)  // ~60 FPS
            };
            _timer.Tick += (_, __) => OnTick();
        }

        private void CreateBalls()
        {
            Balls.Clear();
            var rnd = new Random();
            for (int i = 0; i < BallCount; i++)
            {
                double x = rnd.NextDouble() * (CanvasWidth  - 20);
                double y = rnd.NextDouble() * (CanvasHeight - 20);
                Balls.Add(_ballService.CreateBall(x, y));
            }
        }

        private void OnTick()
        {
            foreach (var ball in Balls)
            {
                // **PRZYWRÓCONE** logi do konsoli:
                Console.WriteLine($"Before Update: Ball at ({ball.X:F2}, {ball.Y:F2})");

                // ruch i odbicia:
                _ballService.UpdatePosition(ball, CanvasWidth, CanvasHeight);

                Console.WriteLine($"After Update:  Ball at ({ball.X:F2}, {ball.Y:F2})");
            }
            // **UWAGA**: nie musimy tu już robić OnPropertyChanged(nameof(Balls)),
            // bo każdy Ball przy zmianie X/Y zrobi to sam.
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
