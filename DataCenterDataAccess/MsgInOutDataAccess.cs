using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  public class MsgInOutDataAccess : BaseDataAccess
  {
    public MsgInOutDataAccess() : base() { }
    public MsgInOutDataAccess(DBDataContext context) : base(context) { }
    
    public void Create(MsgInOut msg)
    {
      context.MsgInOuts.InsertOnSubmit(msg);
      context.SubmitChanges();
    }

  }
}
