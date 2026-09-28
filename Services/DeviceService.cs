using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using zkemkeeper;

namespace AttendanceAPI.Services
{
    public class DeviceService
    {
        private static readonly object _zkLock = new();
        public DeviceService()
        {
        }
        public async Task<List<ZKTecoDevice>> GetDevicesAsync()
        {
            //var filePath = Path.Combine(_dataDirectory, "devices.json");
            //if (!File.Exists(filePath))
            //{
            //    return new List<ZKTecoDevice>();
            //}

            //var json = await File.ReadAllTextAsync(filePath);
            //return JsonSerializer.Deserialize<List<ZKTecoDevice>>(json, _jsonOptions) ?? new List<ZKTecoDevice>();

            using (var context = new AppDbContext())
            {
                return await context.MachineDevices.Where(d => d.IsDelete == false).Select(c => new ZKTecoDevice()
                {
                    DeviceId = c.Id,
                    DeviceName = c.Name,
                    IpAddress = c.IpAddress,
                    Port = c.Port,
                    Location = c.Location,
                    Status = c.Status,
                    LastSync = c.LastSync,
                    MachineNumber = c.MachineNumber,
                    isActive = c.IsActive,
                    CanReadLogs = c.CanReadLogs,
                    CompanyCode = c.CompanyCode
                }).ToListAsync();
            }
        }
        public async Task<List<ZKTecoDevice>> GetActiveDevicesAsync()
        {
            using (var context = new AppDbContext())
            {
                return await context.MachineDevices.Where(c => c.IsActive).Select(c => new ZKTecoDevice()
                {
                    DeviceId = c.Id,
                    DeviceName = c.Name,
                    IpAddress = c.IpAddress,
                    Port = c.Port,
                    Location = c.Location,
                    Status = c.Status,
                    LastSync = c.LastSync,
                    MachineNumber = c.MachineNumber
                }).ToListAsync();
            }
        }
        public async Task UpdateDeviceStatusAsync(Guid deviceId, string status, DateTime? lastSync = null)
        {
            using (var context = new AppDbContext())
            {
                var device = await context.MachineDevices.Where(d => d.Id == deviceId).FirstOrDefaultAsync();
                if (device != null)
                {
                    device.Status = status;
                    if (lastSync.HasValue) device.LastSync = lastSync.Value;
                    context.MachineDevices.Update(device);
                    await context.SaveChangesAsync();
                }
            }
        }
        public async Task<List<ZKTecoDevice>> GetActiveReadAbleLogsDevicesAsync()
         {
            using (var context = new AppDbContext())
            {
                return await context.MachineDevices.Where(c => c.IsActive && c.CanReadLogs).Select(c => new ZKTecoDevice()
                {
                    DeviceId = c.Id,
                    DeviceName = c.Name,
                    IpAddress = c.IpAddress,
                    Port = c.Port,
                    Location = c.Location,
                    Status = c.Status,
                    LastSync = c.LastSync,
                    MachineNumber = c.MachineNumber
                }).ToListAsync();
            }
        }
        public async Task<string> DeleteDevices(string Id)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var getDevice = await context.MachineDevices.FirstOrDefaultAsync(c => c.Id == Guid.Parse(Id));
                    if (getDevice != null)
                    {
                        getDevice.IsDelete = true;
                        getDevice.DeletedAt = DateTime.Today;
                        context.MachineDevices.Update(getDevice);
                        await context.SaveChangesAsync();
                    }
                    return "Delete Device Successfully";
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public async Task<string> EditDevices(DeviceRequest deviceRequest)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var getDevice = await context.MachineDevices.FirstOrDefaultAsync(c => c.Id == Guid.Parse(deviceRequest.DeviceId));
                    if (getDevice != null)
                    {
                        getDevice.Name = deviceRequest.DeviceName;
                        getDevice.IpAddress = deviceRequest.IpAddress;
                        getDevice.Port = deviceRequest.PortNumber;
                        getDevice.MachineNumber = deviceRequest.MachineNumber;
                        getDevice.Location = deviceRequest.Location;
                        getDevice.IsActive = deviceRequest.IsActive;
                        getDevice.Status = deviceRequest.IsActive ? "online" : "offline";
                        if (deviceRequest.IsActive && deviceRequest.CanReadLogs && !getDevice.CanReadLogs)
                        {
                            //getDevice.CanReadLogs = await zKTecoService.ConnectDeviceById(deviceRequest);
                            getDevice.CanReadLogs = await CheckDeviceConnectionAsync(deviceRequest.IpAddress, deviceRequest.PortNumber);
                        }
                        else if (!deviceRequest.CanReadLogs && deviceRequest.CanReadLogs != getDevice.CanReadLogs) getDevice.CanReadLogs = deviceRequest.CanReadLogs;
                            context.MachineDevices.Update(getDevice);
                        await context.SaveChangesAsync();
                    }
                    return "Changes Saved Successfuly";
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<string> AddNewDevice(DeviceRequest deviceRequest)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var newDevice = new MachineDevice
                    {
                        Id = Guid.NewGuid(),
                        Name = deviceRequest.DeviceName,
                        IpAddress = deviceRequest.IpAddress,
                        Port = deviceRequest.PortNumber,
                        Location = deviceRequest.Location,
                        LastSync = null,
                        MachineNumber = deviceRequest.MachineNumber,
                        Status = "online",
                        IsActive = deviceRequest.IsActive,
                        CompanyCode = deviceRequest.CompanyCode
                    };
                    await context.MachineDevices.AddAsync(newDevice);
                    await context.SaveChangesAsync();
                    return "New Device Connected";
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task<bool> CheckDeviceConnectionAsync(
        string ip,
        int port,
        int timeoutMs = 20000)
        {
            if (!await CanReachPortAsync(ip, port, 500))
                return false;

            return await ConnectWithTimeoutAsync(ip, port, timeoutMs);
        }

        private async Task<bool> CanReachPortAsync(string ip, int port, int timeoutMs)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(ip, port);
                var timeoutTask = Task.Delay(timeoutMs);

                var completed = await Task.WhenAny(connectTask, timeoutTask);
                return completed == connectTask && client.Connected;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> ConnectWithTimeoutAsync(
            string ip,
            int port,
            int timeoutMs)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    lock (_zkLock)
                    {
                        var axCZKEM = new CZKEM();

                        try
                        {
                            bool connected = axCZKEM.Connect_Net(ip, port);
                            axCZKEM.Disconnect();
                            return connected;
                        }
                        finally
                        {
                            // Ensure COM object is released
                            System.Runtime.InteropServices.Marshal
                                .ReleaseComObject(axCZKEM);
                        }
                    }
                });

                if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
                    return task.Result;

                return false;
            }
            catch (AccessViolationException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
