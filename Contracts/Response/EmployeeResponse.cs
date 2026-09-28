namespace AttendanceAPI.Contracts.Response
{
    public class EmployeeResponse
    {
            public string EmployeeId { get; set; } = string.Empty;
            public string ZkUserId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string CompanyCode { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string Designation { get; set; } = string.Empty;
            public int DesignationCode { get; set; } 
            public string CardNumber { get; set; } = string.Empty;
            public string EmployeeStatus { get; set; } = string.Empty;
            public bool IsActive { get; set; } = true;
            public string Status { get; set; } = "active";
            public string FcmToken { get; set; } = string.Empty;
            public DateTime JoinDate { get; set; }

            // HIERARCHY FIELDS
            public string ManagerId { get; set; } = string.Empty; // Reports to this manager
            public string Role { get; set; } = "employee"; // employee, team_lead, manager, admin
            public List<string> TeamMemberIds { get; set; } = new(); // For team leads/managers

            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string? FaceImageBase64 { get; set; }
        
    }
}
