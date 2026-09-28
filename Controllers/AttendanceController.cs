using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Entities;
using AttendanceAPI.Services;
using AttendanceAPI.Utilities;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AttendanceAPI.Controllers
{
    

    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
       
        private readonly AttendanceService _attendanceService;
     
        private readonly ApiClass apiClass;

        public AttendanceController(
          
            AttendanceService attendanceService,
            ApiClass apiClass
            )
        {
           
            _attendanceService = attendanceService;
          
            this.apiClass = apiClass;
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAttendanceHistory(string employeeId, [FromQuery] int days = 30)
        {
            var records = await _attendanceService.GetEmployeeAttendanceAsync(employeeId, days);
            return Ok(records);
        }

        [HttpGet("{employeeId}/today")]
        public async Task<IActionResult> GetTodayAttendance(string employeeId)
        {
            var record = await _attendanceService.GetTodayAttendanceAsync(employeeId);
            
            if (record == null)
            {
                return Ok(new { message = "No attendance record for today" });
            }

            return Ok(record);
        }

 
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            try
            {
                var employee = await _attendanceService.GetEmployeeByIdAsync(request.EmployeeId);
                if (employee == null)
                {
                    return NotFound(new { error = "Employee not found" });
                }

                //// VALIDATION: Check if already checked in
                var existingRecord = await _attendanceService.GetAttendanceByDateAsync(
                    request.EmployeeId,
                    DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year);

                if (existingRecord != null && existingRecord.CheckInTime.HasValue)
                {
                    return BadRequest(new
                    {
                        error = "Already checked in today",
                        checkInTime = existingRecord.CheckInTime
                    });
                }
                byte[] photoBytes = Convert.FromBase64String(request.PhotoBase64);

                //var storedImage = await _storage.GetEmpImageAsync(request.EmployeeId);
                byte[] storedImageBytes = Convert.FromBase64String(employee.FaceImage);
                //float[] value2 = MemoryMarshal.Cast<byte, float>(storedImageBytes).ToArray();

                var response = await apiClass.CompareFacesAsync(photoBytes, storedImageBytes);

                //var faceVerification = await _faceRecognitionService.CompareFaces(
                //    photoBytes,
                //    //request.EmployeeId,
                //    employee.FaceImage
                //);

                if (!response.Match)
                {
                    return BadRequest(new
                    {
                        error = "Face verification failed",
                        message = response.Results,
                        faceVerification = new
                        {
                            verified = false,
                            message = response.Results
                        }
                    });
                }
                double minConfidence = 0.5;
                double maxDistance = 0.6;

                if (!response.Match ||
                    response.BestConfidence < minConfidence) 
                    //|| response.BestConfidence > maxDistance)
                {
                    return Unauthorized(new
                    {
                        error = "Face verification failed",
                        message = "The photo does not match the registered employee face",
                        faceVerification = new
                        {
                            verified = false,
                            confidence = response.BestConfidence,
                            //distance = response.BestConfidence,
                            threshold = minConfidence,
                            message = "Photo does not match registered face"
                        }
                    });
                }
                // Save photo if provided
                //string? photoFileName = null;
                //if (!string.IsNullOrEmpty(request.PhotoBase64))
                //{
                //    photoFileName = await _storage.SavePhotoAsync(
                //        request.PhotoBase64,
                //        request.EmployeeId,
                //        "checkin");
                //}

                //// STEP 6: Create attendance record

                if (existingRecord != null)
                {
                    await _attendanceService.UpdateCheckIn(existingRecord);
                }
                else
                {
                    var record = new AttendanceRecord
                    {
                        EmployeeCode = request.EmployeeId,
                        ////EmployeeName = employee.Name,
                        //AttendanceDate = DateTime.Today,
                        CheckInTime = DateTime.Now,
                        Day = DateTime.Now.Day,
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year,

                        //CheckInSource = "app",
                        CheckInLatitude = request.Latitude,
                        CheckInLongitude = request.Longitude,
                        CheckInAddress = request.CheckInAddress,
                        CheckInPhoto = request.PhotoBase64,// photoFileName ?? string.Empty,

                    };

                    var processedRecord = await _attendanceService.ProcessAttendanceRecord(record);
                }

                return Ok(new
                {
                    success = true,
                    message = "Checked in successfully",
                    //record = processedRecord
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Face Not Recogniced",

                });
            }
        }

        [HttpPost("trigger-absent-check")]
        public async Task<IActionResult> TriggerAbsentCheck()
        {
            try
            {

                var result = await _attendanceService.TriggerAbsentCheckAsync();

                if ((bool)result["success"])
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Absent check completed successfully",
                        timestamp = result["timestamp"]
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        error = result["error"],
                        timestamp = result["timestamp"]
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // ============================================
        // GET ABSENT EMPLOYEES (WITHOUT NOTIFICATION)
        // ============================================
        /// <summary>
        /// Get list of absent employees without sending notifications
        /// </summary>
        [HttpGet("absent-today")]
        public async Task<IActionResult> GetAbsentToday([FromQuery] string? managerId = null)
        {
            try
            {
                // This uses the ReportService instead to just get the list
                // without triggering notifications
                return Ok(new
                {
                    message = "Use /api/report/absent/today endpoint instead",
                    redirectTo = "/api/report/absent/today"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ============================================
        // TEST LATE NOTIFICATION
        // ============================================
        /// <summary>
        /// Test endpoint to trigger a late notification for testing
        /// </summary>
        [HttpPost("test-late-notification")]
        public async Task<IActionResult> TestLateNotification(
            [FromBody] TestNotificationRequest request)
        {
            try
            {
                // This is for testing only - simulates a late check-in
                var testRecord = new AttendanceRecord
                {
                    EmployeeCode = request.EmployeeId,
                    //EmployeeName = request.EmployeeName ?? "Test Employee",
                    CheckInTime = DateTime.Now,
                    AttendanceDate = DateTime.Today,
                    Status = "late"
                };

                // Process as if it were a real check-in
                await _attendanceService.ProcessAttendanceRecord(testRecord);

                return Ok(new
                {
                    success = true,
                    message = "Test late notification sent",
                    testRecord
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // ============================================
        // REQUEST MODELS
        // ============================================
        public class TestNotificationRequest
        {
            public string EmployeeId { get; set; } = "";
            public string? EmployeeName { get; set; }
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
        {
            try
            {
                var employee = await _attendanceService.GetEmployeeByIdAsync(request.EmployeeId);

                var task = await _attendanceService.GetEmployeeActiveTask(request.EmployeeId);
                // VALIDATION: Check if checked in

                var todayRecord = await _attendanceService.GetAttendanceByDateAsync(
                    request.EmployeeId,
                    DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                
                //if (task == false  && request.PhotoBase64 != null) 
                //{
                //    return NotFound(new { error = "Dear please stop your remaing Task." });
                //}
                if (employee == null)
                {
                    return NotFound(new { error = "Employee not found" });
                }

                if (todayRecord == null || !todayRecord.CheckInTime.HasValue)
                {
                    return BadRequest(new { error = "No check-in record found for today" });
                }

                //if (todayRecord.CheckOutTime.HasValue)
                //{
                //    return BadRequest(new
                //    {
                //        error = "Already checked out today",
                //        checkOutTime = todayRecord.CheckOutTime
                //    });
                //}
                byte[] photoBytes = Convert.FromBase64String(request.PhotoBase64);

                //var faceVerification = await _faceRecognitionService.CompareFaces(
                //    photoBytes,
                //    request.EmployeeId
                //    //employee.FaceImage
                //);

                byte[] storedImageBytes = Convert.FromBase64String(employee.FaceImage);
                float[] value2 = MemoryMarshal.Cast<byte, float>(storedImageBytes).ToArray();

                var response = await apiClass.CompareFacesAsync(photoBytes, storedImageBytes);

                //if (!faceVerification.Success)
                //{
                //    return BadRequest(new
                //    {
                //        error = "Face verification failed",
                //        message = faceVerification.Message,
                //        faceVerification = new
                //        {
                //            verified = false,
                //            message = faceVerification.Message
                //        }
                //    });
                //}
                double minConfidence = 0.5;
                double maxDistance = 0.6;

                //if (!faceVerification.IsMatch ||
                //    faceVerification.Confidence < minConfidence ||
                //    faceVerification.Distance > maxDistance)
                //{
                //    return Unauthorized(new
                //    {
                //        error = "Face verification failed",
                //        message = "The photo does not match the registered employee face",
                //        faceVerification = new
                //        {
                //            verified = false,
                //            confidence = faceVerification.Confidence,
                //            distance = faceVerification.Distance,
                //            threshold = minConfidence,
                //            message = "Photo does not match registered face"
                //        }
                //    });
                //}
                // Save photo if provided
                //string? photoFileName = null;
                //if (!string.IsNullOrEmpty(request.PhotoBase64))
                //{
                //    photoFileName = await _storage.SavePhotoAsync(
                //        request.PhotoBase64,
                //        request.EmployeeId,
                //        "checkout");
                //}

                if (!response.Match ||
                    response.BestConfidence < minConfidence)
                //|| response.BestConfidence > maxDistance)
                {
                    return Unauthorized(new
                    {
                        error = "Face verification failed",
                        message = "The photo does not match the registered employee face",
                        faceVerification = new
                        {
                            verified = false,
                            confidence = response.BestConfidence,
                            //distance = response.BestConfidence,
                            threshold = minConfidence,
                            message = "Photo does not match registered face"
                        }
                    });
                }

                if (!response.Match)
                {
                    return BadRequest(new
                    {
                        error = "Face verification failed",
                        message = response.Results,
                        faceVerification = new
                        {
                            verified = false,
                            message = response.Results
                        }
                    });
                }

                if (todayRecord != null)
                {
                    todayRecord.CheckOutTime = DateTime.Now;
                    todayRecord.CheckOutSource = "app";
                    todayRecord.CheckOutLatitude = request.Latitude;
                    todayRecord.CheckOutLongitude = request.Longitude;
                    todayRecord.CheckOutAddress = request.CheckOutAddress;
                    await _attendanceService.UpdateCheckOut(todayRecord);
                }
                else
                {
                    var record = new AttendanceRecord
                    {
                        //Id = todayRecord.Id,
                        EmployeeCode = request.EmployeeId,
                        //EmployeeName = employee.Name,
                        AttendanceDate = DateTime.Today,
                        Day = DateTime.Now.Day,
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year,
                        CheckOutTime = DateTime.Now,
                        CheckOutSource = "app",
                        CheckOutLatitude = request.Latitude,
                        CheckOutLongitude = request.Longitude,
                        CheckOutAddress = request.CheckOutAddress,
                        CheckOutPhoto = request.PhotoBase64 ?? string.Empty
                    };

                    var processedRecord = await _attendanceService.ProcessAttendanceRecord(record);
                    processedRecord.CheckInTime = todayRecord.CheckInTime;
                }
                    
                return Ok(new
                {
                    success = true,
                    message = "Checked out successfully",
                    //record = processedRecord,
                    //totalHours = processedRecord.TotalHours + " hours"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] string? employeeId = null)
        {
            var records = new List<AttendanceRecord>();
            var today = DateTime.Today;

            if (!string.IsNullOrEmpty(employeeId))
            {
                records = await _attendanceService.GetAttendence(employeeId);
            }

            var summary = new
            {
                today = new
                {
                    total = records.Count(r => r.AttendanceDate.Date == today),
                    present = records.Count(r => r.AttendanceDate.Date == today && r.CheckInTime.HasValue),
                    late = records.Count(r => r.AttendanceDate.Date == today && r.Status == "late")
                },
                thisWeek = new
                {
                    total = records.Count(),
                    present = records.Count(r => r.CheckInTime.HasValue),
                    late = records.Count(r => r.Status == "late"),
                    //avgHours = records
                    //    .Where(r => r.AttendanceDate >= thisWeek && r.TotalHours.HasValue)
                    //    .Average(r => r.TotalHours) ?? 0
                }
            };

            return Ok(summary);
        }

        [HttpGet("daywiseStdHrs")]
        public async Task<IActionResult> DayWiseStandardHours(string empCode, DateTime dateTime)
        {
            try
            {
                var daywise = await _attendanceService.DayWiseStandardHour(empCode, dateTime);
                return Ok(daywise);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("ontimeandlaterecords")]
        public async Task<IActionResult> ChartRecord(DateTime dateTime, string EmpCode, DateTime endDate)
        {
            try
            {
                var data = await _attendanceService.ChartRecords(dateTime, EmpCode, endDate);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("manual-checkin")]
        public async Task<IActionResult> ManualCheckIn([FromBody] ManualAttendanceRequest req)
        {
            try
            {
                var data = await _attendanceService.MarkManualCheckIn(req);
                return Ok(data);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }
        [HttpPost("manual-checkout")]
        public async Task<IActionResult> ManualCheckOut([FromBody] ManualAttendanceRequest req)
        {
            try
            {
                var record = await _attendanceService.MarkManualCheckOut(req);
                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errror = ex.Message });
            }

        }

    }
}