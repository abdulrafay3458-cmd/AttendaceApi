namespace AttendanceAPI.Services
{
    using AttendanceAPI.Contracts.Request;
    using AttendanceAPI.Contracts.Response;
    using AttendanceAPI.Data;
    using AttendanceAPI.Entities;
    using AttendanceAPI.Interface;

    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using zkemkeeper;

    public class ZKTecoService
    {
        public readonly IEmployeeService _employeeService;
        public readonly DeviceService _deviceService;
        private readonly Dictionary<Guid, CZKEMClass> _deviceConnections;
        private readonly ILogger<ZKTecoService> _logger;
        private readonly Guid _instanceId = Guid.NewGuid();
        private static readonly object _zkLock = new object();
        //private readonly IMapper mapper;

        public ZKTecoService(ILogger<ZKTecoService> logger, IEmployeeService employeeService, DeviceService deviceService)
        {
            _logger = logger;
            _deviceConnections = new Dictionary<Guid, CZKEMClass>();
            _logger.LogInformation($"ZKTecoService Created: {_instanceId}");
            _employeeService = employeeService;
            _deviceService = deviceService;
            //this.mapper = mapper;
        }

        public async Task ConnectToDevices()
        {
            var devices = await _deviceService.GetActiveReadAbleLogsDevicesAsync();

            foreach (var device in devices)
            {
                try
                {
                    await ConnectToDevice(device);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to connect to {device.DeviceName}: {ex.Message}");
                    throw;
                }
            }
        }
        private async Task<bool> ConnectToDevice(ZKTecoDevice device)
        {
            try
            {
                _logger.LogInformation(
                    "Connecting to {DeviceName} at {Ip}:{Port}",
                    device.DeviceName,
                    device.IpAddress,
                    device.Port
                );

                var axCZKEM = new CZKEMClass();

                bool connected = axCZKEM.Connect_Net(
                    device.IpAddress,
                    device.Port
                );

                if (connected)
                {
                    _deviceConnections[device.DeviceId] = axCZKEM;

                    await _deviceService.UpdateDeviceStatusAsync(
                        device.DeviceId,
                        "online",
                        DateTime.Now
                    );

                    _logger.LogInformation(
                        "Connected to {DeviceName}",
                        device.DeviceName
                    );

                    axCZKEM.RegEvent(
                        device.MachineNumber,
                        65535
                    );

                    return true;
                }

                int errorCode = 0;
                axCZKEM.GetLastError(ref errorCode);

                _logger.LogWarning(
                    "Failed to connect to {DeviceName}. ErrorCode={ErrorCode}",
                    device.DeviceName,
                    errorCode
                );

                await _deviceService.UpdateDeviceStatusAsync(
                    device.DeviceId,
                    "offline"
                );

                return false;
            }
            catch (Exception ex)
            {
                await _deviceService.UpdateDeviceStatusAsync(
                    device.DeviceId,
                    "elusive"
                );

                _logger.LogError(
                    ex,
                    "Error connecting to {DeviceName}",
                    device.DeviceName
                );

                return false;
            }
        }

        //private async Task<bool> ConnectToDevice(ZKTecoDevice device)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Connecting to {device.DeviceName} at {device.IpAddress}:{device.Port}");

        //        var axCZKEM = new CZKEMClass();

        //        bool connected = TryConnect(device.IpAddress, device.Port);

        //        if (connected)
        //        {
        //            _deviceConnections[device.DeviceId] = axCZKEM;
        //            await _deviceService.UpdateDeviceStatusAsync(device.DeviceId, "online", DateTime.Now);
        //            _logger.LogInformation($"✅ Connected to {device.DeviceName}");

        //            // Enable real-time events
        //            axCZKEM.RegEvent(device.MachineNumber, 65535);

        //            return true;
        //        }
        //        else
        //        {
        //            await _deviceService.UpdateDeviceStatusAsync(device.DeviceId, "offline");
        //            _logger.LogWarning($"❌ Failed to connect to {device.DeviceName}");
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await _deviceService.UpdateDeviceStatusAsync(device.DeviceId, "elusive");
        //        _logger.LogError($"Error connecting to {device.DeviceName}: {ex.Message}");
        //        return false;
        //    }
        //}

        //public async Task<bool> ConnectDeviceById(DeviceRequest device)
        //{
        //    try
        //    {
        //        var mappedDevice = mapper.Map<ZKTecoDevice>(device);
        //        mappedDevice.Port = device.PortNumber;
        //        var axCZKEM = new CZKEMClass();
        //        bool connected = axCZKEM.Connect_Net(device.IpAddress, device.PortNumber);
        //        return connected;
        //    } catch (Exception e) { throw; }
        //}

        //public async Task<List<AttendanceRecord>> GetAttendanceFromDevice(ZKTecoDevice device, int noOfDays)
        //{
        //    if (device == null)
        //    {
        //        throw new Exception("Device not found");
        //    }

        //    if (!_deviceConnections.ContainsKey(device.DeviceId))
        //    {
        //        var isConnect = await ConnectToDevice(device);

        //        if (!isConnect) throw new Exception("Device not connected");

        //        await ConnectToDevices();
        //    }


        //    var axCZKEM = _deviceConnections[device.DeviceId];
        //    var records = new List<AttendanceRecord>();

        //    try
        //    {
        //        bool result = axCZKEM.ReadGeneralLogData(device.MachineNumber);

        //        if (!result)
        //        {
        //            int errorCode = 0;
        //            axCZKEM.GetLastError(ref errorCode);

        //            Console.WriteLine(
        //                $"ReadGeneralLogData failed. " +
        //                $"DeviceId={device.DeviceId}, " +
        //                $"MachineNumber={device.MachineNumber}, " +
        //                $"ErrorCode={errorCode}"
        //            );

        //            return null;
        //        }

        //        Console.WriteLine("ReadGeneralLogData succeeded.");
        //        axCZKEM.ReadGeneralLogData(device.MachineNumber);

        //        string enrollNumber = "";
        //        int verifyMode = 0;
        //        int inOutMode = 0;
        //        int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
        //        int workCode = 0;

        //        Get current month and year
        //        int currentDay = DateTime.Now.Day;
        //        int currentMonth = DateTime.Now.Month;
        //        int currentYear = DateTime.Now.Year;
        //        var recFromDate = DateTime.Now.AddDays(-noOfDays);

        //        while (axCZKEM.SSR_GetGeneralLogData(
        //            device.MachineNumber,
        //            out enrollNumber,
        //            out verifyMode,
        //            out inOutMode,
        //            out year,
        //            out month,
        //            out day,
        //            out hour,
        //            out minute,
        //            out second,
        //            ref workCode))
        //        {
        //            Skip if not current month
        //            if (year != recFromDate.Year || month != recFromDate.Month || day < recFromDate.Day)
        //                continue;

        //            var timestamp = new DateTime(year, month, day, hour, minute, second);
        //            var employee = await _employeeService.GetEmployeeByZkUserIdAsync(enrollNumber);
        //            if (employee != null)
        //            {
        //                var record = new AttendanceRecord
        //                {
        //                    EmployeeCode = employee.Code.ToString(),
        //                    EmployeeName = employee.Name,
        //                    Year = timestamp.Year,
        //                    Month = timestamp.Month,
        //                    Fortnight = timestamp.Day <= 15 ? 1 : 2,
        //                    Day = timestamp.Day,
        //                    StandardHourCode = employee.StandardHourCode,
        //                    StandardHourWeekday = ((int)timestamp.DayOfWeek + 6) % 7 + 1,
        //                    AttendanceDate = timestamp.Date,
        //                };
        //                if (inOutMode == 0) // Check-in
        //                {
        //                    record.CheckInTime = timestamp;
        //                    record.CheckInSource = "zkteco";
        //                    record.CheckInDeviceId = device.DeviceId;
        //                }
        //                else // Check-out
        //                {
        //                    record.CheckOutTime = timestamp;
        //                    record.CheckOutSource = "zkteco";
        //                    record.CheckOutDeviceId = device.DeviceId;
        //                }
        //                records.Add(record);
        //            }
        //        }

        //        await _deviceService.UpdateDeviceStatusAsync(device.DeviceId, "online", DateTime.Now);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error reading attendance: {ex.Message}");
        //        throw;
        //    }

        //    return records;
        //}
        //    public async Task<List<AttendanceRecord>> GetAttendanceFromDevice(
        //ZKTecoDevice device,
        //int noOfDays)
        //    {
        //        if (device == null)
        //        {
        //            throw new ArgumentNullException(nameof(device));
        //        }

        //        // Make sure the device is connected
        //        if (!_deviceConnections.TryGetValue(
        //                device.DeviceId,
        //                out var axCZKEM))
        //        {
        //            _logger.LogInformation(
        //                "Device {DeviceName} is not connected. Connecting...",
        //                device.DeviceName);

        //            var isConnected = await ConnectToDevice(device);

        //            if (!isConnected)
        //            {
        //                throw new Exception(
        //                    $"Device {device.DeviceName} could not be connected.");
        //            }

        //            if (!_deviceConnections.TryGetValue(
        //                    device.DeviceId,
        //                    out axCZKEM))
        //            {
        //                throw new Exception(
        //                    $"Connection object not found for device {device.DeviceName}.");
        //            }
        //        }

        //        var records = new List<AttendanceRecord>();

        //        try
        //        {
        //            _logger.LogInformation(
        //                "Reading attendance logs from {DeviceName}. MachineNumber={MachineNumber}",
        //                device.DeviceName,
        //                device.MachineNumber);

        //            // Load logs from device into SDK memory.
        //            bool result = axCZKEM.ReadGeneralLogData(
        //                device.MachineNumber);

        //            if (!result)
        //            {
        //                int errorCode = 0;

        //                axCZKEM.GetLastError(ref errorCode);

        //                _logger.LogError(
        //                    "ReadGeneralLogData failed. " +
        //                    "DeviceId={DeviceId}, " +
        //                    "DeviceName={DeviceName}, " +
        //                    "MachineNumber={MachineNumber}, " +
        //                    "ErrorCode={ErrorCode}",
        //                    device.DeviceId,
        //                    device.DeviceName,
        //                    device.MachineNumber,
        //                    errorCode);

        //                await _deviceService.UpdateDeviceStatusAsync(
        //                    device.DeviceId,
        //                    "offline");

        //                return records;
        //            }

        //            _logger.LogInformation(
        //                "ReadGeneralLogData succeeded for {DeviceName}",
        //                device.DeviceName);

        //            // Date from which records should be imported.
        //            var recFromDate = DateTime.Now.AddDays(-noOfDays);

        //            _logger.LogInformation(
        //                "Reading attendance from {FromDate}",
        //                recFromDate);

        //            string enrollNumber = string.Empty;

        //            int verifyMode = 0;
        //            int inOutMode = 0;

        //            int year = 0;
        //            int month = 0;
        //            int day = 0;
        //            int hour = 0;
        //            int minute = 0;
        //            int second = 0;

        //            int workCode = 0;

        //            int totalLogs = 0;
        //            int skippedLogs = 0;
        //            int employeeNotFound = 0;

        //            bool resData = (axCZKEM.SSR_GetGeneralLogData(
        //                device.MachineNumber,
        //                out enrollNumber,
        //                out verifyMode,
        //                out inOutMode,
        //                out year,
        //                out month,
        //                out day,
        //                out hour,
        //                out minute,
        //                out second,
        //                ref workCode));


        //            bool readResult = axCZKEM.ReadGeneralLogData(device.MachineNumber);

        //            int error = 0;
        //            axCZKEM.GetLastError(ref error);

        //            Console.WriteLine($"ReadGeneralLogData: {readResult}");
        //            Console.WriteLine($"Error: {error}");

        //            while (resData)
        //            {
        //                totalLogs++;

        //                DateTime timestamp;

        //                try
        //                {
        //                    timestamp = new DateTime(
        //                        year,
        //                        month,
        //                        day,
        //                        hour,
        //                        minute,
        //                        second);
        //                }
        //                catch (Exception ex)
        //                {
        //                    _logger.LogWarning(
        //                        ex,
        //                        "Invalid attendance timestamp from device {DeviceName}. " +
        //                        "User={EnrollNumber}, Date={Year}-{Month}-{Day} {Hour}:{Minute}:{Second}",
        //                        device.DeviceName,
        //                        enrollNumber,
        //                        year,
        //                        month,
        //                        day,
        //                        hour,
        //                        minute,
        //                        second);

        //                    continue;
        //                }

        //                // IMPORTANT:
        //                // Compare the complete DateTime instead of
        //                // comparing year/month/day separately.
        //                if (timestamp < recFromDate)
        //                {
        //                    skippedLogs++;
        //                    continue;
        //                }

        //                _logger.LogDebug(
        //                    "Attendance log found. Device={DeviceName}, " +
        //                    "User={EnrollNumber}, Timestamp={Timestamp}, InOutMode={InOutMode}",
        //                    device.DeviceName,
        //                    enrollNumber,
        //                    timestamp,
        //                    inOutMode);

        //                // Find employee using ZKTeco enrollment/user ID.
        //                var employee =
        //                    await _employeeService.GetEmployeeByZkUserIdAsync(
        //                        enrollNumber);

        //                if (employee == null)
        //                {
        //                    employeeNotFound++;

        //                    _logger.LogWarning(
        //                        "Employee not found for ZKTeco User ID {EnrollNumber} " +
        //                        "on device {DeviceName}",
        //                        enrollNumber,
        //                        device.DeviceName);

        //                    continue;
        //                }

        //                var record = new AttendanceRecord
        //                {
        //                    EmployeeCode = employee.Code.ToString(),

        //                    Year = timestamp.Year,
        //                    Month = timestamp.Month,

        //                    Fortnight = timestamp.Day <= 15
        //                        ? 1
        //                        : 2,

        //                    Day = timestamp.Day,

        //                    StandardHourCode =
        //                        employee.StandardHourCode,

        //                    StandardHourWeekday =
        //                        ((int)timestamp.DayOfWeek + 6) % 7 + 1,

        //                    AttendanceDate =
        //                        timestamp.Date
        //                };

        //                // ZKTeco InOutMode:
        //                // 0 = Check In
        //                // 1 = Check Out
        //                if (inOutMode == 0)
        //                {
        //                    record.CheckInTime = timestamp;
        //                    record.CheckInSource = "zkteco";
        //                    record.CheckInDeviceId = device.DeviceId;
        //                }
        //                else
        //                {
        //                    record.CheckOutTime = timestamp;
        //                    record.CheckOutSource = "zkteco";
        //                    record.CheckOutDeviceId = device.DeviceId;
        //                }

        //                records.Add(record);
        //            }

        //            _logger.LogInformation(
        //                "Attendance reading completed for {DeviceName}. " +
        //                "TotalLogs={TotalLogs}, " +
        //                "SkippedLogs={SkippedLogs}, " +
        //                "EmployeeNotFound={EmployeeNotFound}, " +
        //                "RecordsCreated={RecordsCreated}",
        //                device.DeviceName,
        //                totalLogs,
        //                skippedLogs,
        //                employeeNotFound,
        //                records.Count);

        //            await _deviceService.UpdateDeviceStatusAsync(
        //                device.DeviceId,
        //                "online",
        //                DateTime.Now);

        //            return records;
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(
        //                ex,
        //                "Error reading attendance from device {DeviceName}",
        //                device.DeviceName);

        //            await _deviceService.UpdateDeviceStatusAsync(
        //                device.DeviceId,
        //                "offline");

        //            throw;
        //        }
        //    }

        public async Task<List<AttendanceRecord>> GetAttendanceFromDevice(
    ZKTecoDevice device,
    int noOfDays)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            // Make sure the device is connected
            if (!_deviceConnections.TryGetValue(device.DeviceId, out var axCZKEM))
            {
                _logger.LogInformation("Device {DeviceName} is not connected. Connecting...", device.DeviceName);

                var isConnected = await ConnectToDevice(device);

                if (!isConnected)
                {
                    throw new Exception($"Device {device.DeviceName} could not be connected.");
                }

                if (!_deviceConnections.TryGetValue(device.DeviceId, out axCZKEM))
                {
                    throw new Exception($"Connection object not found for device {device.DeviceName}.");
                }
            }

            var records = new List<AttendanceRecord>();

            try
            {
                _logger.LogInformation(
                    "Reading attendance logs from {DeviceName}. MachineNumber={MachineNumber}",
                    device.DeviceName,
                    device.MachineNumber);

                // Enable device first
                bool enabled = axCZKEM.EnableDevice(device.MachineNumber, true);
                if (!enabled)
                {
                    _logger.LogWarning("Failed to enable device {DeviceName}", device.DeviceName);
                }

                // Load logs from device into SDK memory.
                bool result = axCZKEM.ReadGeneralLogData(device.MachineNumber);

                if (!result)
                {
                    int errorCode = 0;
                    axCZKEM.GetLastError(ref errorCode);

                    _logger.LogError(
                        "ReadGeneralLogData failed. " +
                        "DeviceId={DeviceId}, " +
                        "DeviceName={DeviceName}, " +
                        "MachineNumber={MachineNumber}, " +
                        "ErrorCode={ErrorCode}",
                        device.DeviceId,
                        device.DeviceName,
                        device.MachineNumber,
                        errorCode);

                    await _deviceService.UpdateDeviceStatusAsync(
                        device.DeviceId,
                        "offline");

                    return records;
                }

                _logger.LogInformation(
                    "ReadGeneralLogData succeeded for {DeviceName}",
                    device.DeviceName);

                // Date from which records should be imported.
                var recFromDate = DateTime.Now.AddDays(-noOfDays);

                _logger.LogInformation(
                    "Reading attendance from {FromDate}",
                    recFromDate);

                string enrollNumber = string.Empty;
                int verifyMode = 0;
                int inOutMode = 0;
                int year = 0;
                int month = 0;
                int day = 0;
                int hour = 0;
                int minute = 0;
                int second = 0;
                int workCode = 0;

                int totalLogs = 0;
                int skippedLogs = 0;
                int employeeNotFound = 0;

                // CORRECTED: Loop through all records
                while (axCZKEM.SSR_GetGeneralLogData(
                    device.MachineNumber,
                    out enrollNumber,
                    out verifyMode,
                    out inOutMode,
                    out year,
                    out month,
                    out day,
                    out hour,
                    out minute,
                    out second,
                    ref workCode))
                {
                    totalLogs++;

                    DateTime timestamp;

                    try
                    {
                        timestamp = new DateTime(
                            year,
                            month,
                            day,
                            hour,
                            minute,
                            second);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Invalid attendance timestamp from device {DeviceName}. " +
                            "User={EnrollNumber}, Date={Year}-{Month}-{Day} {Hour}:{Minute}:{Second}",
                            device.DeviceName,
                            enrollNumber,
                            year,
                            month,
                            day,
                            hour,
                            minute,
                            second);
                        continue;
                    }

                    // Skip old records
                    if (timestamp < recFromDate)
                    {
                        skippedLogs++;
                        continue;
                    }

                    _logger.LogDebug(
                        "Attendance log found. Device={DeviceName}, " +
                        "User={EnrollNumber}, Timestamp={Timestamp}, InOutMode={InOutMode}",
                        device.DeviceName,
                        enrollNumber,
                        timestamp,
                        inOutMode);

                    // Find employee using ZKTeco enrollment/user ID.
                    var employee = await _employeeService.GetEmployeeByZkUserIdAsync(enrollNumber);

                    if (employee == null)
                    {
                        employeeNotFound++;
                        _logger.LogWarning(
                            "Employee not found for ZKTeco User ID {EnrollNumber} " +
                            "on device {DeviceName}",
                            enrollNumber,
                            device.DeviceName);
                        continue;
                    }

                    var record = new AttendanceRecord
                    {
                        EmployeeCode = employee.Code.ToString(),
                        Year = timestamp.Year,
                        Month = timestamp.Month,
                        Fortnight = timestamp.Day <= 15 ? 1 : 2,
                        Day = timestamp.Day,
                        StandardHourCode = employee.StandardHourCode,
                        StandardHourWeekday = ((int)timestamp.DayOfWeek + 6) % 7 + 1,
                        AttendanceDate = timestamp.Date
                    };

                    // ZKTeco InOutMode: 0 = Check In, 1 = Check Out
                    if (inOutMode == 0)
                    {
                        record.CheckInTime = timestamp;
                        record.CheckInSource = "zkteco";
                        record.CheckInDeviceId = device.DeviceId;
                    }
                    else
                    {
                        record.CheckOutTime = timestamp;
                        record.CheckOutSource = "zkteco";
                        record.CheckOutDeviceId = device.DeviceId;
                    }

                    records.Add(record);
                }

                _logger.LogInformation(
                    "Attendance reading completed for {DeviceName}. " +
                    "TotalLogs={TotalLogs}, " +
                    "SkippedLogs={SkippedLogs}, " +
                    "EmployeeNotFound={EmployeeNotFound}, " +
                    "RecordsCreated={RecordsCreated}",
                    device.DeviceName,
                    totalLogs,
                    skippedLogs,
                    employeeNotFound,
                    records.Count);

                await _deviceService.UpdateDeviceStatusAsync(
                    device.DeviceId,
                    "online",
                    DateTime.Now);

                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error reading attendance from device {DeviceName}",
                    device.DeviceName);

                await _deviceService.UpdateDeviceStatusAsync(
                    device.DeviceId,
                    "offline");

                throw;
            }
        }


        public async Task<List<Employee>> SyncUsersFromDevice(Guid deviceId)
        {
            if (!_deviceConnections.ContainsKey(deviceId))
            {
                throw new Exception("Device not connected");
            }

            var device = (await _deviceService.GetActiveDevicesAsync()).FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null)
            {
                throw new Exception("Device not found");
            }

            var axCZKEM = _deviceConnections[deviceId];
            var employees = new List<Employee>();

            try
            {
                axCZKEM.ReadAllUserID(device.MachineNumber);

                string enrollNumber = "";
                string name = "";
                string password = "";
                int privilege = 0;
                bool enabled = false;
                string cardNumber = "";

                while (axCZKEM.SSR_GetAllUserInfo(
                    device.MachineNumber,
                    out enrollNumber,
                    out name,
                    out password,
                    out privilege,
                    out enabled))
                {
                    // Get card number
                    axCZKEM.GetStrCardNumber(out cardNumber);

                    var employee = new Employee
                    {
                        Code = "", //$"EMP{enrollNumber.PadLeft(3, '0')}",
                        ZkUserId = enrollNumber,
                        Name = name,
                        CardNumber = cardNumber,
                        IsActive = enabled ? true : false,
                        JoinDate = DateTime.Today
                    };

                    employees.Add(employee);
                }

                // Save to JSON
                var existingEmployees = await _employeeService.GetEmployeesAsync();
                
                foreach (var emp in employees)
                {
                    var existing = existingEmployees.FirstOrDefault(e => e.ZkUserId == emp.ZkUserId);
                    if (existing == null)
                    {
                        existingEmployees.Add(emp);
                    }
                    else
                    {
                        existing.Name = emp.Name;
                        existing.CardNumber = emp.CardNumber;
                        existing.IsActive = emp.IsActive;
                    }
                }

                //await _storage.SaveEmployeesAsync(existingEmployees);
                _logger.LogInformation($"Synced {employees.Count} users from {device.DeviceName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error syncing users: {ex.Message}");
                throw;
            }

            return employees;
        }

        public async Task DisconnectAll()
        {
            using (var context = new AppDbContext())
            {
                var devices = await context.MachineDevices.Where(c => _deviceConnections.ContainsKey(c.Id)).ToListAsync();
                foreach (var kvp in _deviceConnections)
                {
                    try
                    {
                        kvp.Value.Disconnect();
                        var device = devices.FirstOrDefault(d => d.Id == kvp.Key);
                        if (device != null)
                        {
                            device.Status = "offline";
                            context.MachineDevices.Update(device);
                        }
                        _logger.LogInformation($"Disconnected from device {kvp.Key}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error disconnecting device {kvp.Key}: {ex.Message}");
                    }
                }
                await context.SaveChangesAsync();
                _deviceConnections.Clear();
            }
        }

        public int GetMachineLogsTimer()
        {
            using (var context = new AppDbContext()) {
                var config = context.Configurations.FirstOrDefault(a => a.Heading == "AttendanceApp" && a.Title == "ReadMachineLogsTime");
                return Convert.ToInt32(config.Value);
            }
        }
        public int GetMachineLogsDays()
        {
            using (var context = new AppDbContext()) {
                var config = context.Configurations.FirstOrDefault(a => a.Heading == "AttendanceApp" && a.Title == "ReadMachineLogsDays");
                return Convert.ToInt32(config.Value);
            }
        }

        public bool TryConnect(string ip, int port)
        {
            lock (_zkLock)
            {
                zkemkeeper.CZKEM zk = null;
                try
                {
                    zk = new zkemkeeper.CZKEM();
                    bool isConnected = zk.Connect_Net(ip, port);
                    if (!isConnected)
                    {
                        int errorCode = 0;
                        zk.GetLastError(ref errorCode);
                    }

                    return isConnected;
                }
                catch (AccessViolationException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    if (zk != null)
                    {
                        try { zk.Disconnect(); } catch { }
                    }
                }
            }
        }
    }
}
