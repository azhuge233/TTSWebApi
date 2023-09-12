namespace TTSWebApi.Services.QueueService {
	public sealed class QueuedHostedService: BackgroundService {
		private readonly IBackgroundTaskQueue _taskQueue;
		private readonly ILogger<QueuedHostedService> _logger;

		private static int Counter = 0;

		public QueuedHostedService( IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger) =>
			(_taskQueue, _logger) = (taskQueue, logger);

		protected override Task ExecuteAsync(CancellationToken stoppingToken) {
			_logger.LogInformation($"{nameof(QueuedHostedService)} is running.");

			return ProcessTaskQueueAsync(stoppingToken);
		}

		private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken) {
			while (!stoppingToken.IsCancellationRequested) {
				try {
					Func<CancellationToken, ValueTask> workItem = await _taskQueue.DequeueAsync(stoppingToken);

					_logger.LogInformation($"- Job {++Counter} executing -");

					await workItem(stoppingToken);
				} catch (OperationCanceledException) {
					// Prevent throwing if stoppingToken was signaled
				} catch (Exception ex) {
					_logger.LogError(ex, "Error occurred executing task work item.");
				}
			}
		}

		public override async Task StopAsync(CancellationToken stoppingToken) {
			_logger.LogInformation($"{nameof(QueuedHostedService)} is stopping.");

			await base.StopAsync(stoppingToken);
		}
	}
}
