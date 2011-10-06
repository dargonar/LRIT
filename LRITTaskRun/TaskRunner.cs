using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Repository;
using log4net.Appender;
using DataCenterLogic;

namespace LRITTaskRun
{
  class TaskRunner
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(TaskRunner));
    private string m_name = String.Empty;

    static void Main(string[] args)
    {
      try
      {
        if (args.Length == 0)
        {
          log.Error("No task passed as parameter");
          return;
        }

        var p = new TaskRunner(args[0]);
        p.run();
      }
      catch (Exception ex)
      {
        log.Error(string.Format("Error running task {0}:{1}", args[0], ex.Message), ex);
        System.Console.Write("Error: " + ex.ToString());
      }

      FlushBuffers();
    }

    TaskRunner(string name)
    {
      m_name = name;
    }

    public void run()
    {
      log.Info( string.Format("About to run {0}", m_name) );

      BasicConfiguration config = BasicConfiguration.FromNameValueCollection(System.Configuration.ConfigurationManager.AppSettings);
      DataCenterDataAccess.Config.ConnectionString = config.ConnectionString;

      if (m_name == "sotask")
      {
        DDPManager mgr = new DDPManager();
        mgr.ProcessPendingUpdates();
        return;
      }

      if (m_name == "ddpupdatetask")
      {
        StandingOrderManager som = new StandingOrderManager();
        som.Process();
        return;
      }

      throw new NotImplementedException(string.Format("{0} task not implemented",m_name));
    }

    static public void FlushBuffers()
    {
      ILoggerRepository rep = LogManager.GetRepository();
      foreach (IAppender appender in rep.GetAppenders())
      {
        var buffered = appender as BufferingAppenderSkeleton;
        if (buffered != null)
        {
          buffered.Flush();
        }
      }
    }
  }
}
