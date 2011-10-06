using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Globalization;
using DataCenterDataAccess;
using log4net;
using PriceUpdaterLib;

namespace PriceUpdaterLib
{
  public class PriceUpdater
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(PriceUpdater));
    private DBDataContext context = null;
    private string cstring;
    public PriceUpdater(string cs)
    {
      cstring = cs;
    }

    public PriceUpdater(DBDataContext c)
    {
      context = c;
    }

    public void UpdateContracts(DateTime now, string[] lritids)
    {
      logInfo("Entro en UpdateContracts");
      using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromMinutes(10)))
      {
        DBDataContext ctx = null;
        try
        {
          context = ctx = new DBDataContext(cstring);
          logInfo("Contexto creado");

          IQueryable<Contract> contracts = ctx.Contracts;
          if( lritids.Length != 0 )
            contracts = ctx.Contracts.Where(c => lritids.Contains(c.lritid));
          
          foreach (var contract in contracts )
          {
            //Lo que me deben, se calcula desde la ultima fecha que facture
            contract.credit_balance = CreditBalanceFor(contract.lritid, (DateTime)contract.lastInvoice, now);

            //Lo que debeo, se calcula desde la ultima fecha que me facturaron
            contract.debit_balance  = DebitBalanceFor(contract.lritid, (DateTime)contract.lastInvoiceRecv, now);
          }

          logInfo("Intentando enviar cambios..");
          ctx.SubmitChanges();
          logInfo("Cerrando scope de transaccion..");
          scope.Complete();
          logInfo("TERMINADO!");
          
          if( lritids.Length == 0 )
            logInfo(string.Format("PriceUpdater: Actualizado todo a fecha {0:yyyy-MM-dd}",now));
          else
            logInfo(string.Format("PriceUpdater: Actualizado {0} a fecha {1:yyyy-MM-dd}",String.Join(",",lritids),now));
        }
        catch (Exception ex)
        {
          logError(string.Format("PriceUpdater Error: {0}", ex.Message), ex);

          if (ctx != null)
            ctx.Dispose();
        }
        finally
        {
          if (ctx != null)
            ctx.Dispose();
        }
      }
    }

    private void logInfo(string p)
    {
      log.Info(p);
      System.Console.WriteLine(p);
    }

    private void logError(string p, Exception ex)
    {
      log.Error(p);
      System.Console.WriteLine(p);
    }
    
    //Calcula me deben, son los mensajes de salida (1) que son enviados (Destination) 
    //al lritid del contrato (LDU), entre determinadas fechas
    public decimal CreditBalanceFor(string lritid, DateTime t0, DateTime t1)
    {
      return SumMessages( context.MsgInOuts.Where(m => m.Destination == lritid && m.InOut == 1), 
                          lritid, 
                          t0, 
                          t1);
    }

    //Calcula cuanto debo, son los mensajes de entrada (0) que son originados (Source) 
    //desde el lritid del contrato (LDU)
    public decimal DebitBalanceFor(string lritid, DateTime t0, DateTime t1)
    {
      return SumMessages(context.MsgInOuts.Where(m => m.Source == lritid && m.InOut == 0),
                          lritid,
                          t0,
                          t1);
    }

    private decimal SumMessages(IQueryable<MsgInOut> filtered, string lritid, DateTime t0, DateTime t1)
    {
      decimal? total = filtered
                      .Where(m =>
                        m.TimeStamp.Date > t0.Date &&
                        m.TimeStamp.Date <= t1.Date
                       )
                      .Sum(m => (decimal?)m.Price);

      if (total == null)
        total = 0;

      return (decimal)total;      
    }


  }

}
