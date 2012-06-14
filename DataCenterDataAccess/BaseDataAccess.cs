using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace DataCenterDataAccess
{
  public class BaseDataAccess : IDisposable
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(BaseDataAccess));
    protected DBDataContext context = null;
    public BaseDataAccess()
    {
      //log.Info("Will use " + Config.ConnectionString);
      context = new DBDataContext(Config.ConnectionString);
    }

    public BaseDataAccess(DBDataContext aContext)
    {
      context = aContext;
    }

    public void Dispose()
    {
      if (context == null)
        context.Dispose();
    }
  }
}
