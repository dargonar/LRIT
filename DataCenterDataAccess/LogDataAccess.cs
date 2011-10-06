using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  public class LogDataAccess : BaseDataAccess
  {
    public LogDataAccess() : base() {}
    public LogDataAccess(DBDataContext context) : base(context){} 

    public IQueryable<Log> GetAll()
    {
      return context.Logs;
    }


    public IQueryable<Log> GetAllBetween(DateTime fromDate, DateTime toDate)
    {
      return context.Logs.Where(r => r.Date> fromDate && r.Date < toDate);
    }
  }
}
