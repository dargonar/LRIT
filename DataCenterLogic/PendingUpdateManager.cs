using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  class PendingUpdateManager
  {
    static public List<PendingDDPUpdate> GetPendingUpdates()
    {
      using (var pada = new PendingUpdatesDataAccess())
      {
        var ts = DateTime.UtcNow.AddMinutes(50);
        return pada.WithTimeStampLess(ts);
      }
    }

    public static void Insert(List<PendingDDPUpdate> pendingUpdates)
    {
      using (var pada = new PendingUpdatesDataAccess())
      {
        pada.Create(pendingUpdates);
      }
    }
  }
}

