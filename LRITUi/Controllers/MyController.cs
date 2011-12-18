using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataCenterDataAccess;
using log4net;

namespace LRITUi.Controllers
{
  public class MyController : Controller
  {
    private DBDataContext _context = null;
    protected static readonly ILog log = LogManager.GetLogger(typeof(MyController));

    public DBDataContext context
    {
      get
      {
        //log.Debug("Piden contexto");
        if (_context == null)
        {
          //log.Debug("Lo creo con: " + Config.ConnectionString);
          _context = new DBDataContext(Config.ConnectionString);
        }
        //log.Debug("Lo retorno");
        return _context;
      }
    }

    protected override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      if (_context != null)
      {
        _context.Dispose();
      }
    }

    public void FlashOK(string msg)
    {
      ViewData["flash"] = msg;
      ViewData["flash_type"] = "success";
    }
    
    public void FlashError(string error)
    {
      ViewData["flash"] = error;
      ViewData["flash_type"] = "error";
    }


  }
}
