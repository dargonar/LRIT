using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  public class UserDataAccess
  {
    public bool UserExist(string name)
    {
      using( DBDataContext context = new DBDataContext(Config.ConnectionString) )
      {
        int count = context.Users.Where( u => u.UserName == name ).Count();
        if( count == 0 )
          return false;

        return true;
      }
    }
  }
}
