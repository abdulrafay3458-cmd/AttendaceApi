using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Entities;

namespace AttendanceAPI.Interface
{
    public interface IHolidayService
    {
        Task<IList<Holiday>> GetHolidays();
        Task<bool> GetOffDay(string empCode, DateTime date);
        Task<Holiday> Save(HolidayRequest holidayRequest);
        Task<Holiday> UpdateHoliday(HolidayRequest holidayRequest);
        Task<Holiday> DeleteHoliday(DateTime holidayRequest);
    }
}
