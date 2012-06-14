﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace DataCenterCore
{
  static class Program
  {
    static public string VERSION = "1.7.8";

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
      ServiceBase[] ServicesToRun;
      ServicesToRun = new ServiceBase[] 
			{ 
				new DataCenterCore() 
			};
      ServiceBase.Run(ServicesToRun);
    }
  }
}
