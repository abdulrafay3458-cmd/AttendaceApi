namespace AttendanceAPI.Services
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AbsentCheckService : BackgroundService
    {
        private readonly ILogger<AbsentCheckService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkTime = new TimeSpan(10, 30, 0); // 10:30 AM

        public AbsentCheckService(
            ILogger<AbsentCheckService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 AbsentCheckService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var nextRun = DateTime.Today.Add(_checkTime);

                    // If already past check time today, schedule for tomorrow
                    if (now.TimeOfDay > _checkTime)
                    {
                        nextRun = nextRun.AddDays(1);
                    }

                    var delay = nextRun - now;
                    _logger.LogInformation($"⏰ Next absent check scheduled for: {nextRun:yyyy-MM-dd HH:mm:ss}");

                    await Task.Delay(delay, stoppingToken);

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        await CheckAbsentEmployees();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error in AbsentCheckService: {ex.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Retry after 5 min
                }
            }
        }

        private async Task CheckAbsentEmployees()
        {
            try
            {
                _logger.LogInformation("🔍 Running absent employee check...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var attendanceService = scope.ServiceProvider
                        .GetRequiredService<AttendanceService>();

                    //await attendanceService.CheckAndNotifyAbsentEmployeesAsync();
                }

                _logger.LogInformation("✅ Absent check completed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Failed to check absent employees: {ex.Message}");
            }
        }
    }
}