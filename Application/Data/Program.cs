using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data.Models
{
    public class Ball : IBall, INotifyPropertyChanged
    {
        private double _x, _y, _diameter;
        private Vector2D _velocity;

        public double X
        {
            get => _x;
            set { if (_x != value) { _x = value; OnPropertyChanged(); } }
        }


        public double Y
        {
            get => _y;
            set { if (_y != value) { _y = value; OnPropertyChanged(); } }
        }

        public double Diameter
        {
            get => _diameter;
            set { if (_diameter != value) { _diameter = value; OnPropertyChanged(); } }
        }

        public Vector2D Velocity
        {
            get => _velocity;
            set { if (_velocity != value) { _velocity = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}