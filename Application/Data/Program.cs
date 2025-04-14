namespace Data.Models
{
    public class Ball : IBall
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Diameter { get; set; } = 20;
        public Vector2D Velocity { get; set; }
    }
}