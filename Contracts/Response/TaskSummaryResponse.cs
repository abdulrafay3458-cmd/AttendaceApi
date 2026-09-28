namespace AttendanceAPI.Contracts.Response
{
    public class TaskSummaryResponse
    {
        public int ToDoCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompleteCount { get; set; }
    }
}
