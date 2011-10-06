using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de DDPUpdate
  /// </summary>
  public class DDPUpdateDataAccess : BaseDataAccess
  {

    public DDPUpdateDataAccess() : base() { }
    public DDPUpdateDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Crea un nuevo DDPUpdate en base de datos
    /// </summary>
    /// <param name="ddpUpdate">DDPUpdate</param>
    public DDPUpdate Create(DDPUpdate ddpUpdate, int inOut)
    {
        ddpUpdate.MsgInOut = new MsgInOut();
        ddpUpdate.MsgInOut.DDPVersion = ddpUpdate.DDPFileVersionNum;
        ddpUpdate.MsgInOut.Destination = "";
        ddpUpdate.MsgInOut.InOut = inOut;
        ddpUpdate.MsgInOut.MsgId = ddpUpdate.MessageId;
        ddpUpdate.MsgInOut.MsgType = ddpUpdate.MessageType;
        ddpUpdate.MsgInOut.RefId = "";
        ddpUpdate.MsgInOut.Source = "";
        ddpUpdate.MsgInOut.TimeStamp = ddpUpdate.TimeStamp;

        context.DDPUpdates.InsertOnSubmit(ddpUpdate);
        context.SubmitChanges();
        return ddpUpdate;
    }



  }
}
