namespace AttendanceAPI.Contracts.Response
{
    public class EmployeeAbsentSummaryResponse
    {
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; } = "";
        public string ManagerId { get; set; } = "";
        public List<AbsentDateRecord> AbsentRecords { get; set; } = new();
        public int TotalAbsentDays => AbsentRecords.Count;
    }

    public class AbsentDateRecord
    {
        public DateTime Date { get; set; }
        public string Status { get; set; } = "absent";
    }
}
