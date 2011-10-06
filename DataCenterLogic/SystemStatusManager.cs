using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  public class SystemStatusManager
  {
    public void ProcessSystemStatus(DataCenterTypes.SystemStatusType systemStatus)
    {
      using (var ssda = new SystemStatusDataAccess())
      {
        ssda.Create(TypeHelper.Map2DB(systemStatus), 0);
      }
    }

    public bool IsIdeOk()
    {
      using (var ssda = new SystemStatusDataAccess())
      {
        var systemStatus = ssda.GetLastIncomingStatus();
        if (DateTime.UtcNow - systemStatus.TimeStamp > TimeSpan.FromMinutes(30))
        {
          return false;
        }
        if (systemStatus.SystemStatus1 == 1)
        {
          return false;
        }

        return true;
      }
    }

    public void getLastStatus(out int status, out TimeSpan since)
    {
      using (var ssda = new SystemStatusDataAccess())
      {
        var systemStatus = ssda.GetLastIncomingStatus();

        status = 0;
        since = TimeSpan.FromSeconds(0);
        if (systemStatus != null)
        {
          status = systemStatus.SystemStatus1;
          since = DateTime.UtcNow - systemStatus.TimeStamp;
        }
      }
    }

    
    public void ProcessAspHeartBeat(Common.HeartBeatMessage aspHb)
    {
      var sda = new SystemStatusDataAccess();
      var sysst = new SystemStatus();
      sysst.DDPVersionNum = "0";
      sysst.Message = "ASP OK";
      sysst.MessageId = "0";
      sysst.MessageType = 0;
      sysst.Originator = "ASP";
      sysst.schemaVersion = 0;
      sysst.SystemStatus1 = 0;
      sysst.test = 0;
      sysst.TimeStamp = DateTime.UtcNow;

      sda.Create(sysst, 0);
    }

    public void GetLastAspStatus(out int aspStatus, out TimeSpan aspSince)
    {
      using (var sda = new SystemStatusDataAccess())
      {
        var aspst = sda.GetLastAspStatus();

        aspStatus = 1;
        aspSince = TimeSpan.FromSeconds(0);
        if (aspst != null)
        {
          aspStatus = 0;
          aspSince = DateTime.UtcNow - aspst.TimeStamp;
        }
      }
    }


  }
}
