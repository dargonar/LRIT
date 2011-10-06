using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de PricingUpdate
  /// </summary>
  public class PricingUpdateDataAccess : BaseDataAccess
  {
    public PricingUpdateDataAccess() : base() {}
    public PricingUpdateDataAccess(DBDataContext context) : base(context) {}
    
    /// <summary>
    /// Crea un nuevo PricingUpdate en base de datos
    /// </summary>
    /// <param name="pricingUpdate">PricingUpdate</param>
    public void Create(PricingUpdate pricingUpdate, int inOut )
    {

        pricingUpdate.MsgInOut = new MsgInOut();
        pricingUpdate.MsgInOut.DDPVersion = pricingUpdate.DDPVersionNum;
        pricingUpdate.MsgInOut.Destination = "";
        pricingUpdate.MsgInOut.InOut = inOut;
        pricingUpdate.MsgInOut.MsgId = pricingUpdate.MessageId;
        pricingUpdate.MsgInOut.MsgType = pricingUpdate.MessageType;
        pricingUpdate.MsgInOut.RefId = "";
        pricingUpdate.MsgInOut.Source = "";
        pricingUpdate.MsgInOut.TimeStamp = pricingUpdate.TimeStamp;

        context.PricingUpdates.InsertOnSubmit(pricingUpdate);
        context.SubmitChanges();

    }

    public IQueryable<PricingUpdate> GetAll()
    {
        return context.PricingUpdates;
    }

    public IQueryable<PricingUpdate> GetAllBetween(DateTime fromDate, DateTime toDate)
    {
        return context.PricingUpdates.Where(r => r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }

  }
}
