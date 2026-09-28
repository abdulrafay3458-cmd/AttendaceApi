using AttendanceAPI.Entities;

namespace AttendanceAPI.Contracts.Response
{
    public class TaskResponse
    {
        public string Id { get; set; }
        public string TaskTitle { get; set; }
        public string? ClientName { get; set; }
        public bool IsExtraHours { get; set; }
        public string TaskDescription { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string TaskPreference { get; set; }
        public string Priority { get; set; } // low, medium, high
        public string Status { get; set; } // assigned, in_progress, completed, on_hold
        public string? ClientCode { get; set; } // assigned, in_progress, completed, on_hold
        public Guid GroupTaskId { get; set; }

        // TIME TRACKING
        public double? TotalHoursWorked { get; set; } = 0;
        public long? CurrentDayStartTime { get; set; }
        public List<TaskTimeLogResponse> TimeLogs { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime ServerNow { get; set; } = DateTime.Now;
        public ClientLocation? ClientLocation { get; set; }
        public List<TaskAssigneeResponse> Assignees { get; set; }
    }
    public class ClientLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AllowedRadiusMeters { get; set; }
    }
    public class TaskAssigneeResponse
    {
        public string AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public string Status { get; set; }
        public double? TotalHoursWorked { get; set; }
        public Guid TaskId { get; set; }
    }
}
