using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de las colas de entrada y salida del data center.
  /// Esta clase proporciona las referencias a las colas IN y OUT del data center.
  /// </summary>
  public class QueueManager
  {
    static private QueueManager mInstance = null;
    static public QueueManager Instance()
    {
      if (mInstance == null)
        mInstance = new QueueManager();

      return mInstance;
    }

    public void EnqueueOut(string label, string xmlmsg)
    {
      using (var context = new DBDataContext(DataCenterDataAccess.Config.ConnectionString))
      {
        var msg = new core_out();
        msg.message = xmlmsg;
        msg.msgtype = label;
        msg.created_at = DateTime.UtcNow;
        context.core_outs.InsertOnSubmit(msg);
        context.SubmitChanges();
      }
    }

    public void EnqueueIn(string label, string xmlmsg)
    {
      using (var context = new DBDataContext(DataCenterDataAccess.Config.ConnectionString))
      {
        var msg = new core_in();
        msg.message = xmlmsg;
        msg.msgtype = label;
        msg.created_at = DateTime.UtcNow;
        context.core_ins.InsertOnSubmit(msg);
        context.SubmitChanges();
      }
    }
    
  }
}
