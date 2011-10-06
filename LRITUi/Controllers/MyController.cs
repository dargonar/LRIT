using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataCenterDataAccess;

namespace LRITUi.Controllers
{
  public class MyController : Controller
  {
    private DBDataContext _context = null;

    public DBDataContext context
    {
      get
      {
        if (_context == null)
        {
          _context = new DBDataContext(Config.ConnectionString);
        }

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
