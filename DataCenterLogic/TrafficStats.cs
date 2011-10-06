using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  public class TrafficStats
  {
    public int InMonth    { get; set; }
    public int OutMonth   { get; set; }
    public int MaxMonth   { get; set; }
    public int MinMonth   { get; set; }
    public int AvgMonth   { get; set; }
    public int InWeek     { get; set; }
    public int OutWeek    { get; set; }
    public int MaxWeek    { get; set; }
    public int MinWeek    { get; set; }
    public int AvgWeek    { get; set; }
    public int InDay      { get; set; }
    public int OutDay     { get; set; }
    public int MaxDay     { get; set; }
    public int MinDay     { get; set; }
    public int AvgDay     { get; set; }
  }
  
  public class CurrentDates
  {
    public DateTime Today { get; set; }
    public DateTime MonthStart { get; set; }
    public DateTime MonthEnd { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd{ get; set; }
    
    public CurrentDates()
    {
      Today = DateTime.Today;
      MonthStart = new DateTime(Today.Year, Today.Month, 1);
      MonthEnd = MonthStart.AddMonths(1);

      DayOfWeek day = DateTime.UtcNow.DayOfWeek;
      int days = day - DayOfWeek.Monday;
      WeekStart = DateTime.UtcNow.AddDays(-days);
      WeekEnd = WeekStart.AddDays(6);
    }
  }

  public class TrafficManager
  {
    public TrafficStats GetTrafficStats()
    {
      var ts = new TrafficStats();
      var cd = new CurrentDates();

      using (DBDataContext context = new DBDataContext(Config.ConnectionString))
      {
        ts.InDay = context.MsgInOuts.Count(t => t.InOut == 0 && t.TimeStamp >= cd.Today);
        ts.InMonth = context.MsgInOuts.Count(t => t.InOut == 0 && t.TimeStamp >= cd.MonthStart && t.TimeStamp < cd.MonthEnd);
        ts.InWeek = context.MsgInOuts.Count(t => t.InOut == 0 && t.TimeStamp >= cd.WeekStart && t.TimeStamp < cd.MonthEnd);
        ts.OutDay = context.MsgInOuts.Count(t => t.InOut == 1 && t.TimeStamp >= cd.Today);
        ts.OutMonth = context.MsgInOuts.Count(t => t.InOut == 1 && t.TimeStamp >= cd.MonthStart && t.TimeStamp < cd.MonthEnd);
        ts.OutWeek = context.MsgInOuts.Count(t => t.InOut == 1 && t.TimeStamp >= cd.WeekStart && t.TimeStamp < cd.MonthEnd);
        return ts;
      }

     

    }



  }

}
