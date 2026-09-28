
namespace AttendanceAPI.Entities
{
    public class UserTask
    {
        public Guid Id { get; set; }
        public string? EntityCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedBy { get; set; }
        public string? AssignedByName { get; set; }
        public string AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        public string? TaskPreferance { get; set; }
        public double? TotalHoursWorked { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsActive {get;set;}
        public bool IsDelete {get;set;}
        public DateTime? DeletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt {get; set; }
        public string Priority { get; set; }  // low, medium, high
        public string Status { get; set; } // assigned, in_progress, completed, on_hold
        public ICollection<TaskTimeLogs> TaskTimeLogs { get; set; }
        public Entity Entity { get; set; }
        public ICollection<TaskOvertime> TaskOvertimes { get; set; }

        public ICollection<TeamTask> TeamTasks { get; set; }
        = new List<TeamTask>();


        //public string AssignedToMultiple { get; set; }
        //public AppUser AssignByUser { get; set; }
        //public AppUser AssignToUser { get; set; }
    }
}
