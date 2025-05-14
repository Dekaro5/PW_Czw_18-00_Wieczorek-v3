using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using BusinessLogic.Services;

namespace Presentation.Models
{
    public class Model
    {
        private readonly IBallService _ballService;
        public IBallService BallService => _ballService;

        public Model(double width, double height)
        {
            _ballService = new BallService(width, height);
        }

        public async Task InitializeBalls(int ballCount)
        {
            await _ballService.CreateBalls(ballCount);
        }

        public void UpdateBalls()
        {
            _ballService.UpdateSimulationStep();
        }
    }
}
