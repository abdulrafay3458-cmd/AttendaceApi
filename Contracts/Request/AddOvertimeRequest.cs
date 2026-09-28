namespace AttendanceAPI.Contracts.Request
{
    public class AddOvertimeRequest
    {
        public Guid UserTaskId { get; set; }

        public DateTime OvertimeDate { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeId { get; set; }


        public double OvertimeHours { get; set; }

        public string? Reason { get; set; }
    }
    public class OvertimeSummaryDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime OvertimeDate { get; set; }
        public double TotalHours { get; set; }
        public List<OvertimeEntryDto> Entries { get; set; } = new();
    }

    public class OvertimeEntryDto
    {
        public Guid OvertimeId { get; set; }
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public double OvertimeHours { get; set; }
        public string? Reason { get; set; }
    }
}
