using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using DataCenterDataAccess;

namespace LRITUi.Utils
{
  public static class EntityHelper
  {
    public static void SimpleCopyFrom<T>(this T me, T he, string[] fields)
    {
      foreach(var prop in me.GetType().GetProperties())
      {
        if(!fields.Contains(prop.Name))
          continue;

        prop.SetValue(me, prop.GetValue(he, null), null);
      }
    }
  }
}
