using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using log4net;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de las tareas periodicas que se realizan en el DataCenter.
  /// Esta clase es la encargada de enviar SystemStatus al IDE, Procesar los requerimientos de posisiones activos,
  /// procesar los StandingOrders.
  /// </summary>
  public class SchedulerManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(ActiveShipPositionRequestManager));
    private Thread mMainThread          = new Thread(MainThreadStub);
    private Thread mProcessThread;
    private Thread mDDPUpdateThread;
    private bool   mRun                 = false;
    private long   mLastSystemStatus    = 0;
    private long   mLastStandingOrderCheck = 0;
    private long   mLastDDPUpdateCheck  = 0;

    public delegate void SystemStatusSent(bool code, object desc);
    public event SystemStatusSent OnSystemStatusSent;

    public BasicConfiguration mBasicConfiguration;

    /// <summary>
    /// Inicia el proceso de scheduling
    /// </summary>
    public void Start()
    {
      mMainThread.Start(this);

      if (System.Configuration.ConfigurationManager.AppSettings["run_sotask"] == "yes")
      {
        log.Info("ProcessStandingOrders task will run inside Core");
        mProcessThread = new Thread(new ThreadStart(ProcessStandingOrders));
        mProcessThread.Start();
      }
      else
      {
        log.Warn("ProcessStandingOrders task not running inside Core");
      }

      if (System.Configuration.ConfigurationManager.AppSettings["run_ddpupdatetask"] == "yes")
      {
        log.Info("ProcessDDPUpdate task will run inside Core");
        mDDPUpdateThread = new Thread(new ThreadStart(ProcessDDPUpdate));
        mDDPUpdateThread.Start();
      }
      else
      {
        log.Warn("ProcessDDPUpdate task not running inside Core");
      }

    }

    /// <summary>
    /// Frena el proceso de scheduling
    /// </summary>
    public void Stop()
    {
      mRun = false;
      mMainThread.Join();
      mProcessThread.Join();
      mDDPUpdateThread.Join();
    }

    /// <summary>
    /// Thread principal donde todas las acciones ocurren
    /// </summary>
    private void MainThread()
    {
      mRun = true;
      long t0 = 0;

      TimeSpan tsTimeOut = TimeSpan.FromMinutes(1);
      if (mBasicConfiguration.DCDebug == "true")
      {
        tsTimeOut = TimeSpan.FromSeconds(5);

      }
      DateTime DateTimeIn5MinutesInc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 0, 0);

      while (mRun == true)
      {

        //Each minute
        if (TimeSpan.FromTicks(DateTime.Now.Ticks - t0) > tsTimeOut)
        {
          log.Debug(string.Format("Ejecutando Scheduled Task cada {0} {1}", tsTimeOut, mBasicConfiguration.DCDebug == "true" ? "Segundos" : "Minutos"));

          try
          {
            //Check if we need to send a new system status
            SendSystemStatus();


            int min;
            int hour;

            if (mBasicConfiguration.DCDebug == "true")
            {
              min = DateTimeIn5MinutesInc.Minute;
              hour = DateTimeIn5MinutesInc.Hour;
            }
            else
            {
              min = DateTime.UtcNow.Minute;
              hour = DateTime.UtcNow.Hour;
            }

            //Check for other actions in the database
            //solo en los minutos 0,15,30,45
            //if (new int[] { 0, 15, 30, 45 }.Contains(min))
            //{
            //log.Debug("Cuarto de hora .. procesando ASPR y SO");
            ActiveShipPositionRequestManager ardao = new ActiveShipPositionRequestManager();
            ardao.DCDebug = mBasicConfiguration.DCDebug;
            ardao.Process(hour, min);

            //Check standing orders
            //ProcessStandingOrders();
            //}

            //Check delayed reports and retry programming if necessary
            var sm = new ShipManager();
            sm.CheckShipStatus();

            if (mBasicConfiguration.DCDebug == "true")
            {
              DateTimeIn5MinutesInc = DateTimeIn5MinutesInc.AddMinutes(5);
              log.Debug(DateTimeIn5MinutesInc.ToString());
            }
          }
          catch (Exception ex)
          {
            log.Error("Secheduler Error ", ex);
          }

          t0 = DateTime.Now.Ticks;
        }

        //Delay 1s
        Thread.Sleep(TimeSpan.FromSeconds(1));
      }
    }
    /// <summary>
    /// Procesa las ultimas posiciones de mis barcos para saber si estan sobre alguna standing order.
    /// </summary>
    private void ProcessStandingOrders()
    {
      while (mRun == true)
      {
        //Each 30mins
        var now = DateTime.Now.Ticks;
        if (TimeSpan.FromTicks(now - mLastStandingOrderCheck).TotalMinutes < 30)
        {
          Thread.Sleep(TimeSpan.FromSeconds(1));
          continue;
        }

        StandingOrderManager som = new StandingOrderManager();
        som.Process();

        mLastStandingOrderCheck = now;
      }
    }

    private void ProcessDDPUpdate()
    {
      while (mRun == true)
      {
        //Each 1min
        var now = DateTime.Now.Ticks;
        if (TimeSpan.FromTicks(now - mLastDDPUpdateCheck).TotalSeconds < 60)
        {
          Thread.Sleep(TimeSpan.FromSeconds(1));
          continue;
        }

        log.Info("ProcessDDPUpdate: Checking for pending updates");
        DDPManager mgr = new DDPManager();
        mgr.ProcessPendingUpdates();

        mLastDDPUpdateCheck = now;
      }
    }

    


    /// <summary>
    /// Verifica si es tiempo de enviar un SystemStatus al IDE
    /// </summary>
    private void SendSystemStatus()
    {
      try
      {
        //Each 30mins
        if( TimeSpan.FromTicks(DateTime.Now.Ticks - mLastSystemStatus).TotalMinutes < 30 )
          return;

        var configMgr = new ConfigurationManager();

        DataCenterTypesIDE.SystemStatusType status = new DataCenterTypesIDE.SystemStatusType();
        status.DDPVersionNum = DDPVersionManager.currentDDP();
        status.Message       = "System OK";
        status.MessageId     = DataCenterLogic.MessageIdManager.Generate();
        status.MessageType   = DataCenterTypesIDE.messageTypeType4.Item11;
        status.Originator    = configMgr.Configuration.DataCenterID;
        status.schemaVersion = decimal.Parse(configMgr.Configuration.SchemaVersion);
        status.SystemStatus  = DataCenterTypesIDE.systemStatusIndicatorType.Item0;
        status.test          = DataCenterTypesIDE.testType.Item1;
        status.TimeStamp     = DateTime.UtcNow;

        //Message msg = new Message(status);
        //msg.Label = "systemStatus";

        //using (TransactionScope ts = new TransactionScope())
        //{
        QueueManager.Instance().EnqueueOut("systemStatus", new XmlSerializerHelper<DataCenterTypesIDE.SystemStatusType>().ToStr(status));
        //}

        mLastSystemStatus = DateTime.Now.Ticks;

        try {
          //Fire event
          if( OnSystemStatusSent != null )
            OnSystemStatusSent(true, "OK");
        }
        catch{ }
      }
      catch(Exception e)
      {
        try {
          //Fire event
          if( OnSystemStatusSent != null )
            OnSystemStatusSent(false, e);
        } catch { }
      }
    }

    static private void MainThreadStub(object par)
    {
      SchedulerManager mgr = par as SchedulerManager;
      try
      {
        mgr.MainThread();
      }
      catch /*(Exception e)*/
      {
        //TODO: Log
      }
    }

  }
}
