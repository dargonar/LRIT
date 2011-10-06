using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Linq;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de Receipt
  /// </summary>
  public class ReceiptDataAccess : BaseDataAccess
  {

    public ReceiptDataAccess() : base() { }
    public ReceiptDataAccess(DBDataContext context) : base(context) { } 

    /// <summary>
    /// Crea un nuevo Receipt en base de datos
    /// </summary>
    /// <param name="receipt">Receipt</param>
    public void Create(Receipt receipt, int inOut, decimal? price)
    {
        receipt.MsgInOut = new MsgInOut();
        receipt.MsgInOut.Price = price;
        receipt.MsgInOut.DDPVersion = receipt.DDPVersion;
        receipt.MsgInOut.Destination = receipt.Destination;
        receipt.MsgInOut.InOut = inOut;
        receipt.MsgInOut.MsgId = receipt.MessageId;
        receipt.MsgInOut.MsgType = receipt.MessageType;
        receipt.MsgInOut.RefId = receipt.ReferenceId;
        receipt.MsgInOut.Source = receipt.Originator;
        receipt.MsgInOut.TimeStamp = receipt.TimeStamp;
        receipt.MsgInOut.Price = price;

        context.Receipts.InsertOnSubmit(receipt);
        context.SubmitChanges();
    }

    public IQueryable<Receipt> GetAll(int msgInOut)
    {
      return context.Receipts.Where(r => r.MsgInOut.InOut == msgInOut);
    }

    public IQueryable<Receipt> GetAllBetween(int msgInOut, DateTime fromDate, DateTime toDate)
    {
      return context.Receipts.Where(r => r.MsgInOut.InOut == msgInOut && r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }
  }
}
