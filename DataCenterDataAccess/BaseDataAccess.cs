using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  public class BaseDataAccess : IDisposable
  {
    protected DBDataContext context = null;
    public BaseDataAccess()
    {
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
