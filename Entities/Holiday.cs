namespace AttendanceAPI.Entities
{
    public class Holiday
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Fortnight { get; set; }
        public int Weekday { get; set; }
        public DateTime HolidayDate { get; set; }
        public string Title { get; set; }
    }
}
