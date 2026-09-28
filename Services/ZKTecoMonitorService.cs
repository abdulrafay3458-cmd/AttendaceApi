namespace AttendanceAPI.Services
{
    using Microsoft.AspNetCore.SignalR;

    public class ZKTecoMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ZKTecoMonitorService> _logger;
        public ZKTecoMonitorService(
            IServiceProvider serviceProvider,
            ILogger<ZKTecoMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync( CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "ZKTeco Monitor Service started"
            );

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var zkService =
                        scope.ServiceProvider.GetRequiredService<ZKTecoService>();

                    await zkService.ConnectToDevices();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to connect to ZKTeco devices during startup"
                );
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var zkService =
                            scope.ServiceProvider.GetRequiredService<ZKTecoService>();

                        var attendanceService =
                            scope.ServiceProvider.GetRequiredService<AttendanceService>();

                        var deviceService =
                            scope.ServiceProvider.GetRequiredService<DeviceService>();

                        var devices =
                            await deviceService.GetActiveReadAbleLogsDevicesAsync();

                        var delayTime = zkService.GetMachineLogsTimer();
                        var days = zkService.GetMachineLogsDays();

                        foreach (var device in devices)
                        {
                            try
                            {
                                var records = await zkService.GetAttendanceFromDevice(
                                    device,
                                    days);

                                if (records.Count > 0)
                                {
                                    await attendanceService.ProcessMachineRecord(
                                        records);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(
                                    ex,
                                    "Error monitoring device {DeviceName}",
                                    device.DeviceName);
                            }
                        }

                        await Task.Delay(
                            TimeSpan.FromMinutes(delayTime),
                            stoppingToken
                        );
                    }
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error in monitor service"
                    );
                }
            }
        }


        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("ZKTeco Monitor Service started");
        //    using (var scope = _serviceProvider.CreateScope())
        //    { 
        //        var zkService = scope.ServiceProvider.GetRequiredService<ZKTecoService>();
        //   //     await zkService.ConnectToDevices();
        //    }
        //        int delayTime = 0;

        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            using (var scope = _serviceProvider.CreateScope())
        //            {
        //                var zkService = scope.ServiceProvider.GetRequiredService<ZKTecoService>();
        //                var attendanceService = scope.ServiceProvider.GetRequiredService<AttendanceService>();
        //                var deviceService = scope.ServiceProvider.GetRequiredService<DeviceService>();
        //                //var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<AttendanceHub>>();

        //                var devices = await deviceService.GetActiveReadAbleLogsDevicesAsync();

        //                delayTime = zkService.GetMachineLogsTimer();
        //                var days = zkService.GetMachineLogsDays();

        //                foreach (var device in devices)
        //                {
        //                    try
        //                    {
        //                        var records = await zkService.GetAttendanceFromDevice(device, days);

        //                        foreach (var record in records)
        //                        {
        //                       if (records.Any())
        //                        {
        //                           await attendanceService.ProcessMachineRecord(records);
        //                        }
        //                        // Broadcast via SignalR
        //                        //await hubContext.Clients.All.SendAsync(
        //                        //    "ReceiveAttendanceUpdate",
        //                        //    record,
        //                        //    stoppingToken);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        _logger.LogError($"Error monitoring device {device.DeviceName}: {ex.Message}");
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Error in monitor service: {ex.Message}");
        //        }

        //        await Task.Delay(TimeSpan.FromMinutes(delayTime), stoppingToken); // Check every 10 seconds
        //    }
        //} 
    }
}