using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  public class SARServiceManager
  {
    static public bool ServiceExists(string lritID)
    {
      using (var ssda = new SARServiceDataAccess())
      {
        var sars = ssda.GetServiceByLRITId(lritID);
        if (sars == null)
          return false;

        return true;
      }
    }
  }

}
