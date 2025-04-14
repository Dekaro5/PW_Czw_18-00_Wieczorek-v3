using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BusinessLogic.Abstractions;
using BusinessLogic.Services;
using Data.Models;
using System.Collections.ObjectModel;

namespace Presentation
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IBallService _ballService;
        private readonly System.Windows.Threading.DispatcherTimer _timer;
        private double _canvasWidth = 500;
        private double _canvasHeight = 300;
        private int _ballCount;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<IBall> Balls { get; } = new();
        public ICommand CreateBallsCommand { get; }
        public ICommand StartCommand { get; }

        public int BallCount
        {
            get => _ballCount;
            set
            {
                _ballCount = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _ballService = new BallService();
            CreateBallsCommand = new RelayCommand(CreateBalls);
            StartCommand = new RelayCommand(Start);

            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(16);
            _timer.Tick += (_, __) => Update();
        }

        private void Start()
        {
            _timer.Start();
        }

        private void CreateBalls()
        {
            Console.WriteLine($"Canvas Width: {_canvasWidth}, Canvas Height: {_canvasHeight}");
            Balls.Clear();
            for (int i = 0; i < BallCount; i++)
            {
                var ball = _ballService.CreateBall(
                    _canvasWidth / 2,
                    _canvasHeight / 2
                );
                Balls.Add(ball);
            }
        }

        private void Update()
        {
            foreach (var ball in Balls)
            {
                _ballService.UpdatePosition(ball, _canvasWidth, _canvasHeight);
            }
            OnPropertyChanged(nameof(Balls));
        }
    }
}
