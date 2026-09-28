namespace AttendanceAPI.Entities
{
    public class TeamTask
    {
        public Guid TeamTaskId {  get; set; }
        public Guid TaskId { get; set; }
        public UserTask Task { get; set; }
    }
}
