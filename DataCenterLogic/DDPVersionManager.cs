using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  public class DDPVersionManager
  {
    public DDPVersion DDPFromDate(DateTime startTime)
    {
      using (var ddpa = new DDPVersionDataAccess())
      {
        return ddpa.DDPForDate(startTime);
      }
    }

    public DDPVersion GetCurrentDDPVersion()
    {
      return DDPFromDate(DateTime.UtcNow);
    }

    public DDPVersion GetDDPVersion(string version)
    {
      using (var ddpda = new DDPVersionDataAccess())
      {
        return ddpda.GetDDPVersion(version);
      }
    }

    public DDPVersion GetInmediateDDPVersion(string version)
    {
      using (var ddpda = new DDPVersionDataAccess())
      {
        return ddpda.GetInmediateDDPVersion(version);
      }
    }

    public DDPVersion GetRegularDDPVersion(string version)
    {
      using (var ddpda = new DDPVersionDataAccess())
      {
        return ddpda.GetRegularDDPVersion(version);
      }
    }

    public static string currentDDP()
    {
      using (var ddpda = new DDPVersionDataAccess())
      {
        var dv = ddpda.TodaysDDP();
        return dv.regularVer + ":" + dv.inmediateVer;
      }
    }
  }
}
