using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
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

        public void LogCollision(string info)
        {
            var logEntry = $"{DateTime.UtcNow:O};COLLISION;{info}";
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
