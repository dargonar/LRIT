using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de PricingRequestSent
  /// </summary>
  public class PricingRequestSentDataAccess : BaseDataAccess
  {
    public PricingRequestSentDataAccess() : base() { }
    public PricingRequestSentDataAccess(DBDataContext context) : base(context) { }
    
    /// <summary>
    /// Crea un nuevo PricingRequestSent en base de datos
    /// </summary>
    /// <param name="pricingRequestSent">PricingRequestSent</param>
    public void Create(PricingRequestSent pricingRequestSent, int inOut )
    {
        pricingRequestSent.MsgInOut = new MsgInOut();
        pricingRequestSent.MsgInOut.DDPVersion = pricingRequestSent.DDPVersionNum;
        pricingRequestSent.MsgInOut.Destination = "";
        pricingRequestSent.MsgInOut.InOut = inOut;
        pricingRequestSent.MsgInOut.MsgId = pricingRequestSent.MessageId;
        pricingRequestSent.MsgInOut.MsgType = pricingRequestSent.MessageType;
        pricingRequestSent.MsgInOut.RefId = "";
        pricingRequestSent.MsgInOut.Source = pricingRequestSent.Originator;
        pricingRequestSent.MsgInOut.TimeStamp = pricingRequestSent.TimeStamp;

        context.PricingRequestSents.InsertOnSubmit(pricingRequestSent);
        context.SubmitChanges();
    }


    public IQueryable<PricingRequestSent> GetAll()
    {
        return context.PricingRequestSents;
    }

    public IQueryable<PricingRequestSent> GetAllBetween(DateTime fromDate, DateTime toDate)
    {
        return context.PricingRequestSents.Where(r => r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }





  }
}
