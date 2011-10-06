using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;

namespace DataCenterLogic
{
  public class DataCenterCore
  {
    private InputMessageManager inMsgMgr = InputMessageManager.instance();
    private OutputMessageManager outMsgMgr = OutputMessageManager.instance();
    private SchedulerManager schedulerMgr = new SchedulerManager();

    public DataCenterCore(BasicConfiguration config)
    {
      //Configura la instancia de logging
      XmlConfigurator.Configure();

      QueueManager.Instance().SetIn(config.CoreInQueue);
      QueueManager.Instance().SetOut(config.CoreOutQueue);
      
      inMsgMgr.mBasicConfiguration = config;
      outMsgMgr.mBasicConfiguration = config;
      schedulerMgr.mBasicConfiguration = config;
    }

    public void Start()
    {
      inMsgMgr.Start();
      outMsgMgr.Start();
      schedulerMgr.Start();
    }

    public void Stop()
    {
      inMsgMgr.Stop();
      outMsgMgr.Stop();
      schedulerMgr.Stop();
    }
  }
}
