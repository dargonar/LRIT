using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de PricingNotification
  /// </summary>
  public class PricingNotificationDataAccess : BaseDataAccess
  {
    public PricingNotificationDataAccess() : base() { }
    public PricingNotificationDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Crea un nuevo PricingNotification en base de datos
    /// </summary>
    /// <param name="pricingNotification">PricingNotification</param>
    public void Create(PricingNotification pricingNotification, int inOut)
    {
        pricingNotification.MsgInOut = new MsgInOut();
        pricingNotification.MsgInOut.DDPVersion = pricingNotification.DDPVersion;
        pricingNotification.MsgInOut.Destination = "";
        pricingNotification.MsgInOut.InOut = inOut;
        pricingNotification.MsgInOut.MsgId = pricingNotification.MessageId;
        pricingNotification.MsgInOut.MsgType = pricingNotification.MessageType;
        pricingNotification.MsgInOut.RefId = "";
        pricingNotification.MsgInOut.Source = "";
        pricingNotification.MsgInOut.TimeStamp = pricingNotification.TimeStamp;


        context.PricingNotifications.InsertOnSubmit(pricingNotification);
        context.SubmitChanges();

    }

    public IQueryable<PricingNotification> GetAll(int msgInOut)
    {
      return context.PricingNotifications.Where(r => r.MsgInOut.InOut == msgInOut);
    }


    public IQueryable<PricingNotification> GetAllBetween(int msgInOut, DateTime fromDate, DateTime toDate)
    {
      return context.PricingNotifications.Where(r => r.MsgInOut.InOut == msgInOut && r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }
  }
}
