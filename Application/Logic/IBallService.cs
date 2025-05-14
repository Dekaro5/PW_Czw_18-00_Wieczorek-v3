using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Data;

namespace BusinessLogic.Abstractions
{
    public interface IBallService
    {
        double Width { get; }
        double Height { get; }
        ObservableCollection<IBall> Balls { get; }

        Task CreateBalls(int count, double defaultRadius = 20, double defaultMass = 10);
        void UpdateSimulationStep();
    }
}
