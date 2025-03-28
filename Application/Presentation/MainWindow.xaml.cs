using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BusinessLogic.Services;

namespace Presentation
{
    public partial class MainWindow : Window
    {
        private readonly BallService _ballService;
        private readonly Ellipse _ballEllipse;
        private const double TableWidth = 500;
        private const double TableHeight = 300;

        public MainWindow()
        {
            InitializeComponent();

            _ballService = new BallService();
            var ball = _ballService.CreateBall(100, 100);

            _ballEllipse = new Ellipse
            {
                Width = ball.Diameter,
                Height = ball.Diameter,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(_ballEllipse, ball.X);
            Canvas.SetTop(_ballEllipse, ball.Y);
            TableCanvas.Children.Add(_ballEllipse);
        }
    }
}