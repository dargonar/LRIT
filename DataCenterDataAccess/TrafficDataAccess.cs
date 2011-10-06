using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace DataCenterDataAccess
{
  public class TrafficDataAccess : BaseDataAccess
  {
    public TrafficDataAccess() : base() { }
    public TrafficDataAccess(DBDataContext context) : base(context) { }

    public IQueryable<MonthStat> ReportMonth()
    {
      return context.MonthStats;
    }

    public IQueryable<DayStat> ReportDay()
    {
      return context.DayStats;
    }

    public IQueryable<WeekStat> ReportWeek()
    {
      return context.WeekStats;
    }

    public List<DayStat> GetDayStat()
    {
      return context.DayStats.Where(s => s.ano == DateTime.UtcNow.Year && s.dia >= DateTime.UtcNow.Day - 7).ToList();
    }

    public List<WeekStat> GetWeekStat(out int woy)
    {
      
      // Gets the Calendar instance associated with a CultureInfo.
      CultureInfo myCI = new CultureInfo("");
      Calendar myCal = myCI.Calendar;

      // Gets the DTFI properties required by GetWeekOfYear.
      CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
      DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
      woy = myCal.GetWeekOfYear( DateTime.UtcNow, myCWR, myFirstDOW );
      var woy1 = woy;
      return context.WeekStats.Where(s => s.semana >= woy1 - 8 ).ToList();
    }

    public List<MonthStat> GetMonthStat()
    {
      var currentMonth = DateTime.Today.Month;
      var currentYear = DateTime.Today.Year;
      if (currentMonth <= 6)
        return context.MonthStats.Where(s => s.mes >= currentMonth + 6 && s.ano >= currentYear - 1).ToList();
      else 
        return context.MonthStats.Where(s => s.mes >= currentMonth - 6 && s.ano == currentYear).ToList();    
    }

  }
}
