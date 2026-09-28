namespace AttendanceAPI.Services
{
    using AttendanceAPI.Data;
    using AttendanceAPI.Entities;
    using AttendanceAPI.Interface;
    using AttendanceAPI.Utilities;
    using Azure.Core;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Serilog;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;

    public class NotificationService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IHubContext<AttendanceHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _fcmServerKey; // Add to appsettings.json

        public NotificationService(
            IHubContext<AttendanceHub> hubContext,
            ILogger<NotificationService> logger,
            IConfiguration configuration,
            IEmployeeService employeeService)
        {
            _hubContext = hubContext;
            _logger = logger;
            _httpClient = new HttpClient();
            _fcmServerKey = configuration["Firebase:ServerKey"] ?? "";
            _employeeService = employeeService;
        }

        // Send notification to employee
        public async Task SendAttendanceNotificationAsync(
            string employeeId,
            string title,
            string message,
            Dictionary<string, object>? data = null)
        {
            try
            {
                _logger.LogInformation($"📨 Sending notification to {employeeId}: {title}");

                // 1. Save to storage
                var notification = new Notification
                {
                    UserCode = employeeId,
                    Title = title,
                    Message = message,
                    Type = "attendance",
                    //Data = data ?? new Dictionary<string, object>()
                    CheckType = data.FirstOrDefault(c => c.Key == "type").Value.ToString(),
                    CheckTime = (DateTime)data.FirstOrDefault(c => c.Key == "time").Value,
                    Location = data.FirstOrDefault(c => c.Key == "location").Value.ToString()
                };
                //await _storage.AddNotificationAsync(notification);

                // 2. Send via SignalR (for active app users)
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
                {
                    employeeId = employeeId,
                    title = title,
                    message = message,
                    type = "attendance",
                    data = data,
                    timestamp = DateTime.Now
                });

                // 3. Send FCM push notification (for closed app)
                var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
                if (employee != null && !string.IsNullOrEmpty(employee.FcmToken))
                {
                    await SendFCMNotificationAsync(employee.FcmToken, title, message, data);
                }

                _logger.LogInformation($"✅ Notification sent successfully to {employeeId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Failed to send notification: {ex.Message}");
            }
        }

        // Send notification to manager (new method)
        public async Task SendManagerNotificationAsync(
            string managerId,
            string title,
            string message,
            Dictionary<string, object>? data = null)
        {
            try
            {
                _logger.LogInformation($"📨 Sending manager notification to {managerId}: {title}");

                // 1. Save to storage
                var notification = new Notification
                {
                    UserCode = managerId,
                    Title = title,
                    Message = message,
                    Type = "manager-alert",
                    //Data = data ?? new Dictionary<string, object>()
                    CheckType = data.FirstOrDefault(c => c.Key == "type").Value.ToString(),
                    CheckTime = (DateTime)data.FirstOrDefault(c => c.Key == "time").Value,
                    Location = data.FirstOrDefault(c => c.Key == "location").Value.ToString()
                };
                //await _storage.AddNotificationAsync(notification);

                // 2. Send via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
                {
                    employeeId = managerId,
                    title = title,
                    message = message,
                    type = "manager-alert",
                    data = data,
                    timestamp = DateTime.Now,
                    priority = "high" // Mark as high priority for managers
                });

                // 3. Send FCM push notification (IMPORTANT - even if app closed)
                var manager = await _employeeService.GetEmployeeByIdAsync(managerId);
                if (manager != null && !string.IsNullOrEmpty(manager.FcmToken))
                {
                    await SendFCMNotificationAsync(
                        manager.FcmToken,
                        title,
                        message,
                        data,
                        priority: "high");
                }

                _logger.LogInformation($"✅ Manager notification sent to {managerId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Failed to send manager notification: {ex.Message}");
            }
        }

        // Send FCM Push Notification (Enhanced)
        private async Task SendFCMNotificationAsync(
            string fcmToken,
            string title,
            string message,
            Dictionary<string, object>? data = null,
            string priority = "normal")
        {
            try
            {
                if (string.IsNullOrEmpty(_fcmServerKey))
                {
                    _logger.LogWarning("⚠️ FCM Server Key not configured");
                    return;
                }

                var fcmData = new Dictionary<string, object>(data ?? new Dictionary<string, object>())
                {
                    { "click_action", "FLUTTER_NOTIFICATION_CLICK" }
                };

                var payload = new
                {
                    to = fcmToken,
                    priority = priority,
                    notification = new
                    {
                        title = title,
                        body = message,
                        sound = "default",
                        badge = "1",
                        click_action = "FLUTTER_NOTIFICATION_CLICK"
                    },
                    data = fcmData
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"key={_fcmServerKey}");

                var response = await _httpClient.PostAsync(
                    "https://fcm.googleapis.com/fcm/send",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"📱 FCM notification sent successfully");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"⚠️ FCM send failed: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ FCM send error: {ex.Message}");
            }
        }

        // Send leave notification
        public async Task SendLeaveNotificationAsync(
            string employeeId,
            string title,
            string message)
        {
            var notification = new Notification
            {
                UserCode = employeeId,
                Title = title,
                Message = message,
                Type = "leave"
            };
            //await _storage.AddNotificationAsync(notification);

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                employeeId = employeeId,
                title = title,
                message = message,
                type = "leave",
                timestamp = DateTime.Now
            });

            // Send FCM
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee != null && !string.IsNullOrEmpty(employee.FcmToken))
            {
                await SendFCMNotificationAsync(employee.FcmToken, title, message);
            }
        }

        public async Task SendTaskNotificationAsync(
            string userCode,
            string title,
            string message,
            AppDbContext context,
            Dictionary<string, object>? data = null)
        {
            //var notification = new Notification
            //{
            //    EmployeeId = employeeId,
            //    Title = title,
            //    Message = message,
            //    Type = "task",
            //    Data = data ?? new Dictionary<string, object>()
            //};
            var notification = new Notification()
            {
                Id = Guid.NewGuid(),
                UserCode = userCode,
                Title = title,
                Message = message,
                Type = "task",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await context.Notifications.AddAsync(notification);
            //await _storage.AddNotificationAsync(notification);

            await _hubContext.Clients.User(userCode).SendAsync("ReceiveNotification", new
            {
                employeeId = userCode,
                title = title,
                message = message,
                type = "task",
                data = data,
                timestamp = DateTime.Now
            });

            // Send FCM
            //var employee = await _storage.GetEmployeeByIdAsync(userCode);
            //var employee = await context.AppUsers.Where(c => c.Code == userCode).FirstOrDefaultAsync();
            //if (employee != null && !string.IsNullOrEmpty(employee.FcmToken))
            //{
                //await SendFCMNotificationAsync(employee.FcmToken, title, message, data);
            //}

            _logger.LogInformation($"📨 Task notification sent to {userCode}");
        }

        public async Task SendFcmNotificationAsync(string empCode, string userFcmToken, string title, string body)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var message = new FirebaseAdmin.Messaging.Message()
                    {
                        Token = userFcmToken,
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = title,
                            Body = body
                        },
                        Android = new FirebaseAdmin.Messaging.AndroidConfig
                        {
                            Priority = FirebaseAdmin.Messaging.Priority.High,
                            Notification = new FirebaseAdmin.Messaging.AndroidNotification
                            {
                                ChannelId = "high_importance_channel"
                            }
                        }
                    };
                    var notification = new Notification()
                    {
                        Id = Guid.NewGuid(),
                        UserCode = empCode,
                        Title = title,
                        Message = body,
                        Type = "task",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await context.Notifications.AddAsync(notification);
                    await context.SaveChangesAsync();
                    string response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendAsync(message);
                }
            } catch (Exception e)
            {
                Log.Error(e.Message);
                //throw new Exception("Exception In FCM handling");
            }
        }
    }
}