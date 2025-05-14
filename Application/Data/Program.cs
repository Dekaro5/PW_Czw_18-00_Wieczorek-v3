using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    public class Ball : IBall
    {
        private double _x;
        private double _y;
        private Vector2D _velocity;
        private readonly double _diameter;
        private readonly double _mass;
        private readonly object _lock = new object();

        public event PropertyChangedEventHandler PropertyChanged;

        public double X
        {
            get { lock (_lock) { return _x; } }
            set
            {
                bool changed = false;
                lock (_lock)
                {
                    if (_x != value)
                    {
                        _x = value;
                        changed = true;
                    }
                }
                if (changed) { OnPropertyChanged(); }
            }
        }

        public double Y
        {
            get { lock (_lock) { return _y; } }
            set
            {
                bool changed = false;
                lock (_lock)
                {
                    if (_y != value)
                    {
                        _y = value;
                        changed = true;
                    }
                }
                if (changed) { OnPropertyChanged(); }
            }
        }

        public double Diameter => _diameter;
        public double Mass => _mass;

        public Vector2D Velocity
        {
            get { lock (_lock) { return _velocity; } }
            set
            {
                bool changed = false;
                lock (_lock)
                {
                    if (_velocity != value)
                    {
                        _velocity = value;
                        changed = true;
                    }
                }
                if (changed) { OnPropertyChanged(); }
            }
        }

        public Ball(double x, double y, double diameter, double mass, Vector2D velocity)
        {
            _x = x;
            _y = y;
            _diameter = diameter;
            _mass = mass;
            _velocity = velocity ?? throw new ArgumentNullException(nameof(velocity));
        }

        public void Move(double timeStep)
        {
            double newX;
            double newY;

            lock (_lock)
            {
                newX = _x + _velocity.X * timeStep;
                newY = _y + _velocity.Y * timeStep;
            }

            X = newX;
            Y = newY;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
