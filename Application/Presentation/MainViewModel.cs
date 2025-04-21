using BusinessLogic.Abstractions;
using BusinessLogic.Services;
using Data.Models;
using Presentation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IBallService _ballService;
    private readonly System.Windows.Threading.DispatcherTimer _timer;
    private double _canvasWidth = 500;
    private double _canvasHeight = 300;
    private int _ballCount;

    public event PropertyChangedEventHandler PropertyChanged;

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
        _timer.Tick += (_, __) => {
            Console.WriteLine("Timer tick");
            Update();
            };
    }

    private void Start()
    {
        Console.WriteLine("Start command executed");
        _timer.Start();
    }

    private void CreateBalls()
    {
        Balls.Clear();
        for (int i = 0; i < BallCount; i++)
        {
            var ball = _ballService.CreateBall(100, 100);
            Balls.Add(ball);
        }
    }

    private void Update()
    {
        foreach (var ball in Balls)
        {
            Console.WriteLine($"Before Update: Ball at ({ball.X}, {ball.Y})");
            _ballService.UpdatePosition(ball, _canvasWidth, _canvasHeight);
            Console.WriteLine($"After Update: Ball at ({ball.X}, {ball.Y})");
        }
        OnPropertyChanged(nameof(Balls));
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
