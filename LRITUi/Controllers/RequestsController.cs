using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data.Linq;
using DataCenterLogic;
using DataCenterDataAccess;
using System.Linq.Dynamic;
using GridDemo.Models;
using System.Threading;
using System.Globalization;
using System.Messaging;
using System.Text.RegularExpressions;
using System.Configuration;

namespace LRITUi.Controllers
{
  public class ItemDrop 
  {
    public ItemDrop (){}
    
    public ItemDrop(string argName, string argId) 
    {
      this.NAME = argName;
      this.ID = argId;
    }

    private string mNAME = string.Empty;
    private string mID = string.Empty;

    public string NAME  
    {
      get{return mNAME;}
      set{mNAME=value;}
    } 

    public string ID  
    {
      get { return mID; }
      set { mID = value; }
    } 
  }
    
    [Authorize(Roles = "Administrador, Operador, SARUser")]
    public class RequestsController : MyController
    {
      public ActionResult List(int msgInOut, string refid)
      {
        DDPVersionManager v = new DDPVersionManager();
        
        if (refid != null)
          ViewData["referenceId"] = refid;

        ViewData["msgInOut"] = msgInOut;
        ViewData["LritIDNamePairs"] = ContractingGovermentManager.LritIdNamePairs(v.GetCurrentDDPVersion().Id);
        ViewData["is_sar"] = ControllerContext.HttpContext.User.IsInRole("SARUser") ? "true" : "false";

        return View("List");
      }

      public ActionResult ListSarsurpic(int msgInOut)
      {
        DDPVersionManager v = new DDPVersionManager();

        ViewData["msgInOut"] = msgInOut;
        ViewData["LritIDNamePairs"] = ContractingGovermentManager.LritIdNamePairs(v.GetCurrentDDPVersion().Id);
        return View("ListSarsurpic");
      }

      public List<ItemDrop> BuildPlaceDropDown(int ddpVersion)
      {
        var DropDownList = new List<ItemDrop>();
        List<Place> ddp = context.Places.Where(p => p.ContractingGoverment.DDPVersionId==ddpVersion &&( p.AreaType == "port" || p.AreaType == "portfacility") ).OrderBy(p => p.ContractingGoverment.Name).OrderBy(p=>p.PlaceStringId).ToList();
        foreach (var row in ddp)
        {
          var item = new ItemDrop();
          item.ID = row.PlaceStringId;
          var puntos = row.Name.Length > 50 ? "..." : "";
          item.NAME = row.ContractingGoverment.Name + " - " + string.Format("{0,-62}", row.Name).Remove(50) + puntos + " (" + string.Format("{0,0}",row.PlaceStringId) + ")";
          DropDownList.Add(item);
        }
        return DropDownList;
      }




      [NonAction]
      public ActionResult ShowNewView(int? requestType, int? accessType)
      {
        List<ItemDrop> AccessTypeIndex = new List<ItemDrop>();
        List<ItemDrop> RequestTypeIndex = new List<ItemDrop>();
        List<string> ItemElementIndex = new List<string>();

        AccessTypeIndex.Clear();
        RequestTypeIndex.Clear();
        ItemElementIndex.Clear();

        if (ControllerContext.HttpContext.User.IsInRole("Administrador") || ControllerContext.HttpContext.User.IsInRole("Operador"))
        {
          AccessTypeIndex.Add(new ItemDrop("0 - Reset entitlement", "0"));
          AccessTypeIndex.Add(new ItemDrop("1 - Coastal State", "1"));
          AccessTypeIndex.Add(new ItemDrop("2 - Flag State", "2"));
          AccessTypeIndex.Add(new ItemDrop("3 - Port State with distance trigger", "3"));
          AccessTypeIndex.Add(new ItemDrop("5 - Port with time trigger", "5"));
        }

        AccessTypeIndex.Add(new ItemDrop("6 - SAR", "6"));

        ItemElementIndex.Add("Place");
        ItemElementIndex.Add("Port");
        ItemElementIndex.Add("PortFacility");

        RequestTypeIndex.Add(new ItemDrop("0 - Reset", "0"));
        RequestTypeIndex.Add(new ItemDrop("1 - One Time Poll", "1"));
        RequestTypeIndex.Add(new ItemDrop("2 - 15 min Period", "2"));
        RequestTypeIndex.Add(new ItemDrop("3 - 30 min Period", "3"));
        RequestTypeIndex.Add(new ItemDrop("4 - 1 hour Period", "4"));
        RequestTypeIndex.Add(new ItemDrop("5 - 3 hour Period", "5"));
        RequestTypeIndex.Add(new ItemDrop("6 - 6 hour Period", "6"));
        RequestTypeIndex.Add(new ItemDrop("7 - Archived LRIT info request", "7"));
        RequestTypeIndex.Add(new ItemDrop("8 - Stop sending position reports", "8"));
        RequestTypeIndex.Add(new ItemDrop("9 - Most recent Position", "9"));
        RequestTypeIndex.Add(new ItemDrop("10 - 12 hour Period", "10"));
        RequestTypeIndex.Add(new ItemDrop("11 - 24 hour Period", "11"));


        if (accessType == 0)
        {
          RequestTypeIndex.Clear();
          RequestTypeIndex.Add(new ItemDrop("Reset", "0"));
        }

        ViewData["AccessTypeIndex"] = new SelectList(AccessTypeIndex, "ID", "NAME", accessType);
        ViewData["RequestTypeIndex"] = new SelectList(RequestTypeIndex, "ID", "NAME", requestType);
        ViewData["ItemElement"] = new SelectList(ItemElementIndex);


        var ddpda = new DDPVersionDataAccess();
        ViewData["ItemField"] = new SelectList(BuildPlaceDropDown(ddpda.TodaysDDP().Id), "ID", "NAME");

        ViewData["AccessType"] = accessType.ToString();
        ViewData["RequestType"] = requestType.ToString();

        return View("New");
      }
      
      public ActionResult New(int? requestType, int? accessType)
      {
        //HACK: Por que tengo que hacerlo a mano aca?
        //DataCenterDataAccess.Config.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        //log.Debug("RequestController::New => aca vale:" + System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

        if (this.ControllerContext.HttpContext.User.IsInRole("SARUser"))
          accessType = 6;

        return ShowNewView(requestType, accessType);
      }

      public ActionResult CreateAndSend(DataCenterLogic.DataCenterTypesIDE.ShipPositionRequestType spr, int accessTypeIndex, int requestTypeIndex, string itemElementIndex, string strStartTime, string strStopTime)
      {
        var cfgman = new DataCenterLogic.ConfigurationManager();

        DateTime dateStartTime = DateTime.UtcNow;
        DateTime dateStopTime = DateTime.UtcNow;

        if (spr.IMONum.Trim().Length == 0)
          ModelState.AddModelError("IMONum", "Numero OMI requerido");
        if (spr.DataUserProvider.Trim().Length == 0)
          ModelState.AddModelError("DataUserProvider", "LRIT Id del DataCenter requerido");

        if ( new int[] { 2,3,4,5,6,7,10,11 }.Contains(requestTypeIndex) )
        {
          if (DateTime.TryParse(strStartTime, out dateStartTime) == false)
            ModelState.AddModelError("StartTime", "Fecha Inicio requerida");
          if (DateTime.TryParse(strStopTime, out dateStopTime) == false)
            ModelState.AddModelError("StopTime", "Fecha Fin requerida");
        }

        if (accessTypeIndex == 3 || accessTypeIndex == 5)
        {
          string ItemField = this.Request.Params["ItemField"];
          string ItemElement = this.Request.Params["ItemElement"];
          if (ItemElement == "Port")
            spr.ItemElementName = DataCenterLogic.DataCenterTypesIDE.ItemChoiceType.Port;
          if (ItemElement == "PortFacility")
            spr.ItemElementName = DataCenterLogic.DataCenterTypesIDE.ItemChoiceType.PortFacility;

          spr.Item = ItemField;
        }



        ViewData["AccessType"] = accessTypeIndex.ToString();
        ViewData["RequestType"] = requestTypeIndex.ToString();
        if (!ModelState.IsValid)
          return ShowNewView(requestTypeIndex, accessTypeIndex);

        if( accessTypeIndex != 6 )
          spr.MessageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType1.Item4;
        else
          spr.MessageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType1.Item5;

        spr.MessageId = MessageIdManager.Generate("1005");
        //spr.IMONum;
        //spr.DataUserProvider

        if (accessTypeIndex == 0)
          spr.AccessType = DataCenterLogic.DataCenterTypesIDE.accessTypeType.Item0;
        else if (accessTypeIndex == 1)
          spr.AccessType = DataCenterLogic.DataCenterTypesIDE.accessTypeType.Item1;
        else if (accessTypeIndex == 2)
          spr.AccessType = DataCenterLogic.DataCenterTypesIDE.accessTypeType.Item2;
        else if (accessTypeIndex == 3)
          spr.AccessType = DataCenterLogic.DataCenterTypesIDE.accessTypeType.Item3;
        else if (accessTypeIndex == 5)
          spr.AccessType = DataCenterLogic.DataCenterTypesIDE.accessTypeType.Item5;
        else if (accessTypeIndex == 6)
          spr.AccessType = DataCenterLogic.DataCenterTypesIDE.accessTypeType.Item6;

        //Prevet XSS
        if( ControllerContext.HttpContext.User.IsInRole("SARUser") )
          spr.AccessType = DataCenterLogic.DataCenterTypesIDE.accessTypeType.Item6;

        //spr.Item
        if (itemElementIndex != null)
          spr.ItemElementName = (DataCenterLogic.DataCenterTypesIDE.ItemChoiceType)Enum.Parse(typeof(DataCenterLogic.DataCenterTypesIDE.ItemChoiceType), itemElementIndex);
        
        //this.Request.Params["
        //spr.Distance = ;
        spr.RequestType = (DataCenterLogic.DataCenterTypesIDE.requestTypeType)Enum.Parse(typeof(DataCenterLogic.DataCenterTypesIDE.requestTypeType), "Item" + requestTypeIndex.ToString());
        spr.RequestDuration = new DataCenterLogic.DataCenterTypesIDE.requestDurationType();

        if (spr.RequestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item1 || spr.RequestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item9)
          spr.RequestDuration.startTimeSpecified = false;
        else 
        {
          spr.RequestDuration.startTimeSpecified = true;
          spr.RequestDuration.startTime = dateStartTime;
        }

        if (spr.RequestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item1 || spr.RequestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item9)
          spr.RequestDuration.stopTimeSpecified = false;
        else 
        {
          spr.RequestDuration.stopTimeSpecified= true;
          spr.RequestDuration.stopTime = dateStopTime;
        }

        spr.DataUserRequestor = "1005";//;cfgman.Configuration.DataCenterID;
        spr.TimeStamp = DateTime.UtcNow;
        spr.DDPVersionNum = DDPVersionManager.currentDDP();
        spr.schemaVersion = decimal.Parse(cfgman.Configuration.SchemaVersion);
        spr.test = DataCenterLogic.DataCenterTypesIDE.testType.Item1;

        Message msgout = new Message(spr);
        msgout.Label = "shipPositionRequest";

        string outQueue = System.Configuration.ConfigurationManager.AppSettings["CoreOutQueue"];
        QueueManager.Instance().SetOut(outQueue);
        QueueManager.Instance().EnqueueOut(msgout);

        return View("Sent");
      }

      [NonAction]
      public ActionResult ShowNewSarsurpic(int area)
      {
        MakeComboSARService();

        List<ItemDrop> AreaIndex = new List<ItemDrop>();
        List<string> NumberOfPositions = new List<string>();

      
        AreaIndex.Add(new ItemDrop("Circular", "0"));
        AreaIndex.Add(new ItemDrop("Rectangular", "1"));

        NumberOfPositions.Add("1");
        NumberOfPositions.Add("2");
        NumberOfPositions.Add("3");
        NumberOfPositions.Add("4");

        ViewData["AreaIndex"] = new SelectList(AreaIndex, "ID", "NAME", area);
        ViewData["NumberOfPositions"] = new SelectList(NumberOfPositions);

        ViewData["area"] = area.ToString();

        return View("NewSarsurpic");
      }

      public ActionResult NewRectangularSarsurpic()
      {
        return ShowNewSarsurpic(1);
      }

      public ActionResult NewCircularSarsurpic()
      {
        return ShowNewSarsurpic(0);
      }
      
      [Authorize(Roles = "Administrador, Operador")]
      public ActionResult NewDDPRequest()
      {
        ViewData["archivedVersion"] = "";
        ViewData["updateType"] = 0;
        ViewData["archivedTimeStamp"] = "";

        return View("NewDDPRequest");
      }

      public ActionResult CreateAndSendDDPRequest()
      {
        var ddpRequest = new DataCenterLogic.DDPServerTypes.DDPRequestType();
        
        string archivedVersion = Request.Params["archivedVersion"];
        int updateType = int.Parse(Request.Params["updateType"]);
        string archivedTimeStamp = Request.Params["archivedTimeStamp"];

        ViewData["archivedVersion"] = archivedVersion;
        ViewData["updateType"] = updateType;
        ViewData["archivedTimeStamp"] = archivedTimeStamp;

        DateTime test = DateTime.UtcNow;
        if (updateType == 4 && DateTime.TryParse(archivedTimeStamp, out test) == false)
        {
          ViewData["error"] = "Fecha invalida";
          return View("NewDDPRequest");
        }

        if (updateType == 4 && archivedVersion.Trim().Length == 0)
        {
          ViewData["error"] = "Version no especificada";
          return View("NewDDPRequest");
        }

        ddpRequest.ArchivedDDPVersionNum = null;
        ddpRequest.ArchivedDDPTimeStamp = DateTime.UtcNow;
        ddpRequest.ArchivedDDPTimeStampSpecified = false;

        if (updateType == 4)
        {
          ddpRequest.ArchivedDDPTimeStampSpecified = true;
          ddpRequest.ArchivedDDPTimeStamp = DateTime.Parse(archivedTimeStamp).ToUniversalTime();
        }

        switch(updateType)
        {
          case 0:
            ddpRequest.UpdateType = DataCenterLogic.DDPServerTypes.DDPRequestTypeUpdateType.Item0;
            break;
          case 1:
            ddpRequest.UpdateType = DataCenterLogic.DDPServerTypes.DDPRequestTypeUpdateType.Item1;
            break;
          case 2:
            ddpRequest.UpdateType = DataCenterLogic.DDPServerTypes.DDPRequestTypeUpdateType.Item2;
            break;
          case 3:
            ddpRequest.UpdateType = DataCenterLogic.DDPServerTypes.DDPRequestTypeUpdateType.Item3;
            break;
          case 4:
            ddpRequest.UpdateType = DataCenterLogic.DDPServerTypes.DDPRequestTypeUpdateType.Item4;
            break;
        }

        string outQueue = System.Configuration.ConfigurationManager.AppSettings["CoreOutQueue"];
        QueueManager.Instance().SetOut(outQueue);

        var mgr = new DataCenterLogic.DDPManager();
        QueueManager.Instance().EnqueueOut(mgr.MakeDDPRequest(ddpRequest));

        return View("Sent");
      }

      public ActionResult CreateAndSendSarsurpic(DataCenterLogic.DataCenterTypesIDE.SARSURPICType SARSURPICMsg, int areaIndex, int NumberOfPositions, string Lat, string Long, string var1, string var2)
      {
        var cfgman = new DataCenterLogic.ConfigurationManager();
        var ddpVer = new DDPVersionDataAccess();
        var sprda = new SARSURPICRequestDataAccess(context);
        string strItem = string.Empty;

        

        MakeComboSARService();
        

        //\


        if (!Regex.IsMatch(Lat, @"(([0-8][0-9]\.[0-5][0-9]\.[nNsS])|(90\.00\.[nNsS]))"))
          ModelState.AddModelError("Lat", "Latitud: El formato correcto es 00.00.N/S");
        
        if (!Regex.IsMatch(Long, @"(([0-1][0-7][0-9]\.[0-5][0-9]\.[eEwW])|(180\.00\.[eEwW]))"))
          ModelState.AddModelError("Long", "Longitud: El formato correcto es 000.00.E/W");

        //SARSURPICMsg.DataUserRequestor = "1005";
        var v = ddpVer.TodaysDDP();
        SARSURPICMsg.DDPVersionNum = v.regularVer + ":" + v.inmediateVer;


        if (areaIndex == 0)
        {
          if (!Regex.IsMatch(var1, @"([0-9]{3})"))
            ModelState.AddModelError("var1", "Radio: El formato correcto es 000"); 

            strItem = string.Format(Lat + ":" + Long + ":" + var1);
            SARSURPICMsg.Item = strItem;
            SARSURPICMsg.ItemElementName = DataCenterLogic.DataCenterTypesIDE.ItemChoiceType1.SARCircularArea;
        }
        if (areaIndex == 1)
        {
          if (!Regex.IsMatch(var1, @"(([0-8][0-9]\.[0-5][0-9]\.[nN])|(90\.00\.[nN]))"))
            ModelState.AddModelError("var1", "Offset Norte: El formato correcto es 00.00.N");

          if (!Regex.IsMatch(var2, @"(([0-1][0-7][0-9]\.[0-5][0-9]\.[eE])|(180\.00\.[eE]))"))
            ModelState.AddModelError("var2", "Offset Este: es 000.00.E");

            strItem = string.Format(Lat + ":" + Long + ":" + var1 + ":" + var2);
            SARSURPICMsg.Item = strItem;
            SARSURPICMsg.ItemElementName = DataCenterLogic.DataCenterTypesIDE.ItemChoiceType1.SARRectangularArea;
        }

        if (!ModelState.IsValid)
          return ShowNewSarsurpic(areaIndex);

        SARSURPICMsg.MessageId = MessageIdManager.Generate(SARSURPICMsg.DataUserRequestor);
        SARSURPICMsg.MessageType = (DataCenterLogic.DataCenterTypesIDE.messageTypeType2)Enum.Parse(typeof(DataCenterLogic.DataCenterTypesIDE.messageTypeType2), "Item6");
        //SARSURPICMsg.NumberOfPositions = dc3NumberOfPositions.Text;
        SARSURPICMsg.schemaVersion = decimal.Parse(cfgman.Configuration.SchemaVersion);
        SARSURPICMsg.test = DataCenterLogic.DataCenterTypesIDE.testType.Item1;
        SARSURPICMsg.TimeStamp = DateTime.UtcNow;

        
        
        Message msgout = new Message(SARSURPICMsg);
        msgout.Label = "SARSURPICRequest";

        string outQueue = System.Configuration.ConfigurationManager.AppSettings["CoreOutQueue"];
        QueueManager.Instance().SetOut(outQueue);

        QueueManager.Instance().EnqueueOut(msgout);

        //sprda.Create( TypeHelper.Map2DB(SARSURPICMsg), 1);


        return View("Sent");
      }

      private void MakeComboSARService()
      {
        DDPVersionManager v = new DDPVersionManager();
        var pepe = new List<object>();
        foreach (var sser in context.SARServices.Where(  ss => ss.ContractingGoverment.LRITId == 1005 && ss.ContractingGoverment.DDPVersion.Id == v.GetCurrentDDPVersion().Id ))
        {
          pepe.Add(new { @id = sser.LRITid, @value = sser.Name + " (" + sser.LRITid + ") "});
        }
        ViewData["sar_services"] = pepe;
      }


      public ActionResult GridData(int page, int rows, string[] _search, string sidx, string sord, int msgInOut)
      {

        string[] ReqParams = { "IMONum" , "DataUserRequestor", "DataUserProvider", "AccessType" , "RequestType", "StartTime", "StopTime", "MessageID", "TimeStamp"};
        List<string> columns = new List<string>();
        List<string> querys = new List<string>();


        //Vectores Apareados SearchQuery, Columns
        string tstamp = "-";
        var fromDate = new DateTime(2000,1,1);
        var toDate = new DateTime(2200,1,1);
        for (int i = 0; i < ReqParams.Count(); i++)
        {
          if (ReqParams[i].Contains("TimeStamp"))
          {
            tstamp = Request.Params[ReqParams[i]];
            if (tstamp != null)
            {
              var dates = tstamp.Split('-');
              fromDate = DateTime.Parse(dates[0]);
              if (dates.Length == 1)
                toDate = fromDate.AddDays(1);
              else
                toDate = DateTime.Parse(dates[1]);
            }
            continue;
          }

          var tempValue = Request.Params[ReqParams[i]];
          if (tempValue != null)
          {
            columns.Add(ReqParams[i]);
            querys.Add(tempValue);
          }
        }

        //Solo traer 6 - Sar
        if (ControllerContext.HttpContext.User.IsInRole("SARUser"))
        {
          columns.Add("RequestType");
          querys.Add("6");
        }


        var sprda = new ShipPositionRequestDataAccess(context);
        IQueryable<ShipPositionRequest> requests;

        if (msgInOut == 2)
          requests = sprda.GetActives();
        else 
          requests = sprda.GetAllBetween(msgInOut, fromDate , toDate);

        var model = from entity in requests.OrderBy(sidx + " " + sord)
                    select new
                    {
                      IMONum = entity.IMONum,
                      DataUserRequestor = entity.DataUserRequestor,
                      DataUserProvider = entity.DataUserProvider,
                      AccessType = entity.AccessType,
                      RequestType = entity.RequestType,
                      StartTime = entity.StartTime.ToString(),
                      StopTime = entity.StopTime.ToString(),
                      MessageId = entity.MessageId,
                      TimeStamp = entity.TimeStamp.ToString(),
                    };
        return Json(model.ToJqGridData(page, rows, null, querys.ToArray(), columns.ToArray()), JsonRequestBehavior.AllowGet);
      }

      public ActionResult SurpicGridData(int page, int rows, string[] _search, string sidx, string sord, int msgInOut)
      {

        string[] ReqParams = { "DataUserRequestor", "DDPVersionNum", "Item", "ItemElementName", "MessageId", "NumberOfPositions", "TimeStamp" };
        List<string> columns = new List<string>();
        List<string> querys = new List<string>();


        string tstamp = "-";
        var fromDate = new DateTime(2000, 1, 1);
        var toDate = new DateTime(2200, 1, 1);

        //Vectores Apareados SearchQuery, Columns
        for (int i = 0; i < ReqParams.Count(); i++)
        {

          if (ReqParams[i].Contains("TimeStamp"))
          {
            tstamp = Request.Params[ReqParams[i]];
            if (tstamp != null)
            {
              var dates = tstamp.Split('-');
              fromDate = DateTime.Parse(dates[0]);
              if (dates.Length == 1)
                toDate = fromDate.AddDays(1);
              else
                toDate = DateTime.Parse(dates[1]);
            }
            continue;
          }


          var tempValue = Request.Params[ReqParams[i]];
          if (tempValue != null)
          {
            columns.Add(ReqParams[i]);
            querys.Add(tempValue);
          }
        }

        var sprda = new SARSURPICRequestDataAccess(context);
        IQueryable<SARSURPICRequest> requests;

        requests = sprda.GetAllBetween(msgInOut, fromDate, toDate);

        var model = from entity in requests.OrderBy(sidx + " " + sord)
                    select new
                    {
                      DataUserRequestor = entity.DataUserRequestor,
                      DDPVersionNum = entity.DDPVersionNum,
                      Item = entity.Item,
                      ItemElementName = entity.ItemElementName,
                      MessageId = entity.MessageId,
                      NumberOfPositions = entity.NumberOfPositions,
                      TimeStamp = entity.TimeStamp.ToString(),
                    };
        return Json(model.ToJqGridData(page, rows, null, querys.ToArray(), columns.ToArray()), JsonRequestBehavior.AllowGet);
      }




    }
}
