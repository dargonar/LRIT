using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de ShipPositionReport
  /// </summary>
  public class ShipPositionReportDataAccess : BaseDataAccess
  {
    public ShipPositionReportDataAccess() : base() { }
    public ShipPositionReportDataAccess(DBDataContext context) : base(context) { }


    /// <summary>
    /// Crea un nuevo ShipPositionReport en base de datos
    /// </summary>
    /// <param name="shipPositionReport">ShipPositionReport</param>
    public void Create(ShipPositionReport shipPositionReport, int inOut, decimal? price)
    {
        shipPositionReport.MsgInOut = new MsgInOut();
        shipPositionReport.MsgInOut.Price       = price;
        shipPositionReport.MsgInOut.DDPVersion  = shipPositionReport.DDPVersionNum;
        shipPositionReport.MsgInOut.Destination = shipPositionReport.DataUserProvider;
        shipPositionReport.MsgInOut.InOut       = inOut;
        shipPositionReport.MsgInOut.MsgId       = shipPositionReport.MessageId;
        shipPositionReport.MsgInOut.MsgType     = shipPositionReport.MessageType;
        shipPositionReport.MsgInOut.RefId       = shipPositionReport.ReferenceId;
        shipPositionReport.MsgInOut.Source      = shipPositionReport.DataUserRequestor;
        shipPositionReport.MsgInOut.TimeStamp   = shipPositionReport.TimeStamp5;

        context.ShipPositionReports.InsertOnSubmit(shipPositionReport);
        context.SubmitChanges();
    }

    public IQueryable<ShipPositionReport> GetAll(int inout)
    {
      return context.ShipPositionReports.Where(s => s.MsgInOut.InOut == inout);
    }


    public IQueryable<ShipPositionReport> GetAllBetween(int msgInOut, DateTime fromDate, DateTime toDate)
    {
      return context.ShipPositionReports.Where(s => s.MsgInOut.InOut == msgInOut && s.TimeStamp1 > fromDate && s.TimeStamp1 < toDate);
    }
  }
}
