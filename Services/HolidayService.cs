using System.Reflection.Emit;
using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;

using Microsoft.EntityFrameworkCore;
using System;

namespace AttendanceAPI.Services
{
    public class HolidayService : IHolidayService
    {
        public HolidayService() 
        {

        }

        public async Task<Holiday> DeleteHoliday(DateTime dateTime)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var deleteHoliday = await context.Holiday.FirstOrDefaultAsync(c => c.Year == dateTime.Date.Year && c.Month == dateTime.Date.Month && c.Day == dateTime.Date.Day);
                    if (deleteHoliday == null)
                    {
                        throw new Exception("Not Found");
                    }
                    context.Holiday.Remove(deleteHoliday);
                     await context.SaveChangesAsync();
                    return deleteHoliday;
                }
                catch (Exception ex) 
                {
                    throw ex;
                }
            }
        }

        public async Task<bool> GetOffDay(string empCode, DateTime date)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    bool isHoliday = false;
                    var getStandardHour = await context.Employees.FirstOrDefaultAsync(c => c.Code == empCode);
                    DayOfWeek dayOfWeek = date.DayOfWeek;
                    int weekDayNumber = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
                    var getWeekDay = context.StandardHours.FirstOrDefault(c => c.GroupCode == getStandardHour.StandardHourCode && c.WeekDay == weekDayNumber);
                    var getHoliday = context.Holiday.Where(c => c.Day == date.Day && c.Month == date.Month && c.Year == date.Year).ToList();
                    var checkHolidayExist = getHolidayDetail(getHoliday, date.Date);

                    if (checkHolidayExist && getWeekDay != null && (getWeekDay.Minutes == 0))
                    {
                        isHoliday = true;
                    }
                    else
                    {
                        isHoliday = false;
                    }
                    return isHoliday;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<IList<Holiday>> GetHolidays()
        {
            using (var context = new AppDbContext())
            {
                var getHolidays = await context.Holiday.OrderByDescending(c => c.HolidayDate).ToListAsync();
                return getHolidays;
            }
        }

        public async Task<Holiday> Save(HolidayRequest holidayRequest)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var holiday = await context.Holiday.FirstOrDefaultAsync(c => c.Year == holidayRequest.Date.Year && c.Month == holidayRequest.Date.Month && c.Day == holidayRequest.Date.Day);
                    if (holiday != null)
                    {
                        throw new Exception("Holiday Already Exist");
                    }
                    else
                    {

                        int dayOfWeek = WeekDay(holidayRequest.Date.DayOfWeek);

                        var holidayAdd = new Holiday
                        {
                            Day = holidayRequest.Date.Day,
                            Month = holidayRequest.Date.Month,
                            Year = holidayRequest.Date.Year,
                            Fortnight = Fortnight(holidayRequest.Date),
                            HolidayDate = holidayRequest.Date.Date,
                            Weekday = dayOfWeek,
                            Title = holidayRequest.Title
                        };
                        await context.Holiday.AddAsync(holidayAdd);
                        await context.SaveChangesAsync();
                        return holidayAdd;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        private bool getHolidayDetail(List<Holiday> holidayList, DateTime a)
        {
            var result = true;
            var test = holidayList.Where(c => c.Day == a.Day && c.Year == a.Year && c.Month == a.Month).ToList();
            if (test.Any())
            {
                result = false;
            }
            return result;
        }

        
        public async Task<Holiday> UpdateHoliday(HolidayRequest holidayRequest)
        {
            using (var context = new AppDbContext())
            {
                try
                {
                    var holiday = await context.Holiday.FirstOrDefaultAsync(c => c.Year == holidayRequest.Date.Year && c.Month == holidayRequest.Date.Month && c.Day == holidayRequest.Date.Day);
                    if (holiday == null)
                    {
                        throw new Exception("Holiday not Exist");
                    }
                    holiday.Title = holidayRequest.Title;        
                    await context.SaveChangesAsync();
                    return holiday;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        private int Fortnight(DateTime date)
        {
            return date.Day >= 16 ? 2 : 1;
        }
        private int WeekDay(DayOfWeek day)
        {
            return day == DayOfWeek.Sunday ? 7 : (int)day;
        }
    }
}
