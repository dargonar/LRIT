using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

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

    private MessageQueue mOutQueue = null;
    private MessageQueue mInQueue  = null;

    /// <summary>
    /// Devuelve la referencia a la cola de entrada del DataCenter.
    /// </summary>
    /// <returns>Referencia a la cola</returns>
    public MessageQueue GetInQueue()
    {
      return mInQueue;
    }
    /// <summary>
    /// Establece el nombre de la cola de entrada
    /// </summary>
    /// <param name="name">Nombre a usar</param>
    public void SetIn(string name)
    {
      mInQueue = GetQueue(name);

      ///Tipos de datos que puede contener la cola de entrada
      mInQueue.Formatter = new XmlMessageFormatter
      (new Type[] { 
        typeof(DataCenterLogic.DataCenterTypes.ReceiptType),
        typeof(DataCenterLogic.DataCenterTypes.PricingNotificationType),
        typeof(DataCenterLogic.DataCenterTypes.ShipPositionReportType),
        typeof(DataCenterLogic.DataCenterTypes.PricingUpdateType),
        typeof(DataCenterLogic.DataCenterTypes.SARSURPICType),
        typeof(DataCenterLogic.DataCenterTypes.DDPNotificationType),
        typeof(DataCenterLogic.DataCenterTypes.DDPUpdateType),
        typeof(DataCenterLogic.DataCenterTypes.ShipPositionRequestType),
        typeof(DataCenterLogic.DataCenterTypes.SystemStatusType),
        typeof(Common.PositionMessage),
        typeof(Common.PollResponse),
        typeof(Common.HeartBeatMessage),
      });

    }
    /// <summary>
    /// Devuelve la referencia a la cola de salida del DataCenter.
    /// </summary>
    /// <returns>Referencia a la cola</returns>
    public MessageQueue GetOutQueue()
    {
      return mOutQueue;
    }
    /// <summary>
    /// Establece el nombre de la cola de salida
    /// </summary>
    /// <param name="name">Nombre a usar</param>
    public void SetOut(string name)
    {
      mOutQueue = GetQueue(name);
      
      ///Set data types that the queue can handle 
      mOutQueue.Formatter = new XmlMessageFormatter(
        new Type[]{ 
            typeof(DataCenterLogic.DataCenterTypesIDE.PricingRequestType),
            typeof(DataCenterLogic.DataCenterTypesIDE.SystemStatusType),
            typeof(DataCenterLogic.DataCenterTypesIDE.ReceiptType),
            typeof(DataCenterLogic.DataCenterTypesIDE.JournalReportType),
            typeof(DataCenterLogic.DataCenterTypesIDE.PricingUpdateType),
            typeof(DataCenterLogic.DataCenterTypesIDE.ShipPositionReportType),
            typeof(DataCenterLogic.DataCenterTypesIDE.ShipPositionRequestType),
            typeof(DataCenterLogic.DataCenterTypesIDE.SARSURPICType),
            typeof(DDPServerTypes.DDPRequestType),
        });

    }
    /// <summary>
    /// Obtiene la referencia a una cola de Microsoft Message Queueing
    /// </summary>
    /// <param name="name">Nombre de la cola</param>
    /// <returns>Referencia a la cola</returns>
    public MessageQueue GetQueue(string name)
    {
      MessageQueue queue = null;
      try
      {
        queue = new MessageQueue(name);
      }
      catch(Exception e )
      {
        System.Diagnostics.Debug.WriteLine("Queue Manager, message queue" +  e.ToString() );
      }
      
      return queue;
    }

    /// <summary>
    /// Encola un mensaje en la cola de salida
    /// </summary>
    /// <param name="msg">Mensaje a encolar</param>
    public void EnqueueOut(Message msg)
    {
      MessageQueueTransaction ts = new MessageQueueTransaction();
      ts.Begin();
      mOutQueue.Send(msg, ts);
      ts.Commit();
    }

    public void EnqueueOut(Object obj, string label)
    {
      Message msg = new Message(obj);
      msg.Label = label;

      EnqueueOut(msg);
    }

    /// <summary>
    /// Encola un mensaje en la cola de entrada
    /// </summary>
    /// <param name="msg">Mensaje a encolar</param>
    public void EnqueueIn(Message msg)
    {
      mInQueue.Send(msg, MessageQueueTransactionType.Automatic);
    }
    
  }
}
