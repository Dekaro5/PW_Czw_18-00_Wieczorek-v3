using System.ComponentModel;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {
        double X { get; set; }
        double Y { get; set; }
        double Diameter { get; }
        double Mass { get; }
        Vector2D Velocity { get; set; }

        void Move(double timeStep);
    }
}