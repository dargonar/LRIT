using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  public class PricesDataAccess : BaseDataAccess
  {
    public PricesDataAccess() : base() { }
    public PricesDataAccess(DBDataContext context) : base(context) { }
    
    public enum PriceType
    {
      Poll = 0,
      PeriodicRateChange = 1,
      ArchivedPosition = 2,
      PositionReport = 3,
    }

    public void Create(Price[] prices)
    {
      context.Prices.InsertAllOnSubmit(prices);
      context.SubmitChanges();
    }

    public decimal? GetPrice(string provider, PriceType type)
    {
      Price price = context.Prices
         .Join(context.PriceUserProviders,
                  p => p.id,
                  dp => dp.priceId,
                  (p, dp) => new { p = p, dp = dp })
         .Where(temp => (temp.dp.dataProviderID == provider))
         .Select(temp => temp.p)
         .FirstOrDefault();

      if (price == null)
        return null;

      if (type == PriceType.ArchivedPosition)
        return price.ArchivePositionReport;
      
      if (type == PriceType.PeriodicRateChange)
        return price.PeriodicRateChange;
      
      if (type == PriceType.Poll)
        return price.Poll;
      
      if (type == PriceType.PositionReport)
        return price.PositionReport;

      //Never reach
      return null;
    }


    public Price GetPrice(string provider)
    {
      return context.Prices
         .Join(context.PriceUserProviders,
                  p => p.id,
                  dp => dp.priceId,
                  (p, dp) => new { p = p, dp = dp })
         .Where(temp => (temp.dp.dataProviderID == provider))
         .Select(temp => temp.p)
         .FirstOrDefault();
    }
  }
}
