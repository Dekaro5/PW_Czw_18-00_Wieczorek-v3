using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public enum Wall
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public class BallLogger : IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _loggingTask;
        private readonly string _logFilePath;
        private readonly object _fileLock = new();

        public BallLogger()
        {
            _logFilePath = $"ball_log_{Guid.NewGuid():N}.txt";
            _loggingTask = Task.Run(ProcessQueueAsync);
        }

        public void LogBallCreated(Ball ball)
        {
            var logEntry = $"{DateTime.UtcNow:O};CREATED;Id={ball.Id};X={ball.X:F3};Y={ball.Y:F3};Vx={ball.Velocity.X:F3};Vy={ball.Velocity.Y:F3}";
            _logQueue.Enqueue(logEntry);
        }

        public void LogBallDestroyed(Ball ball)
        {
            var logEntry = $"{DateTime.UtcNow:O};DESTROYED;Id={ball.Id};X={ball.X:F3};Y={ball.Y:F3}";
            _logQueue.Enqueue(logEntry);
        }

        public void LogBallPosition(Ball ball)
        {
            var logEntry = $"{DateTime.UtcNow:O};POSITION;Id={ball.Id};X={ball.X:F3};Y={ball.Y:F3};Vx={ball.Velocity.X:F3};Vy={ball.Velocity.Y:F3}";
            _logQueue.Enqueue(logEntry);
        }

        public void LogWallCollision(Ball ball, Wall wall)
        {
            var logEntry = $"{DateTime.UtcNow:O};WALL_COLLISION;Id={ball.Id};Wall={wall};X={ball.X:F3};Y={ball.Y:F3};Vx={ball.Velocity.X:F3};Vy={ball.Velocity.Y:F3}";
            _logQueue.Enqueue(logEntry);
        }

        public void LogBallToBallCollision(Ball ball1, Ball ball2)
        {
            var logEntry = $"{DateTime.UtcNow:O};BALL_COLLISION;" +
                           $"Id1={ball1.Id};X1={ball1.X:F3};Y1={ball1.Y:F3};Vx1={ball1.Velocity.X:F3};Vy1={ball1.Velocity.Y:F3};" +
                           $"Id2={ball2.Id};X2={ball2.X:F3};Y2={ball2.Y:F3};Vx2={ball2.Velocity.X:F3};Vy2={ball2.Velocity.Y:F3}";
            _logQueue.Enqueue(logEntry);
        }
        private async Task ProcessQueueAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                while (_logQueue.TryDequeue(out var entry))
                {
                    try
                    {
                        lock (_fileLock)
                        {
                            File.AppendAllText(_logFilePath, entry + Environment.NewLine, Encoding.ASCII);
                        }
                    }
                    catch (IOException)
                    {
                        await Task.Delay(50);
                        _logQueue.Enqueue(entry);
                    }
                }
                await Task.Delay(100);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _loggingTask.Wait();
        }
    }
}
