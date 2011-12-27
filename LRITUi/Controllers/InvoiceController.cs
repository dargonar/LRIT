using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Text;
using DataCenterDataAccess;
using LRITUi.Utils;
using System.Globalization;

namespace LRITUi.Controllers
{
  [Authorize(Roles = "Administrador, Facturador")]
    public class InvoiceController : MyController
    {
      public ActionResult DownloadInvoiceFile(int fileid)
      {
        var invoiceFile = context.InvoiceFiles.Where(ivf => ivf.id == fileid).FirstOrDefault();
        return new FileContentResult(invoiceFile.fileContent.ToArray(), invoiceFile.fileType);
      }

      public ActionResult SelectCountry()
      {
        MakeCombos();
        return View();
      }

      public ActionResult List(int emireci)
      {
        ViewData["title"]   = (emireci == 0) ? "Listado de Facturas Emitidas" : "Listado de Facturas Recibidas";
        ViewData["emireci"] = emireci;

        MakeCombos();
        return View("List");
      }

      public ActionResult ListMessages(int invoice)
      {
        var oinvoice = context.Invoices.Where(i => i.id == invoice).SingleOrDefault();

        ViewData["title"] = "Messages for invoice " + oinvoice.invoiceNumber;
        ViewData["invoice"] = invoice;
        return View();
      }

      public ActionResult ListMessagesJSON(int page, int rows, string sidx, string sord, int invoice)
      {
        //No URL HACKING!!!!!
        if (User.IsInRole("ExternoVerificarFacturas"))
        {
          var inv = context.Invoices.Where(i => i.id == invoice).FirstOrDefault();
          if (inv.Contract.lritid != User.Identity.Name || inv.state == 0 )
          {
            return Content("Not allowed to see");
          }
        }

        var columns = new string[] { "MsgType", "MsgId", "RefId", "TimeStamp", "Price" };

        var oinvoice = context.Invoices.Where(i => i.id == invoice).SingleOrDefault();

        return Json(JQGrid.Helper.Paginate(
                      context.MsgInOuts.Where(m => m.TimeStamp >= oinvoice.dateFrom && m.TimeStamp <= oinvoice.dateTo && m.Price != null ),
                      Request.Params,
                      columns,
                      page, rows, sidx, sord), JsonRequestBehavior.AllowGet);
      }

      public ActionResult Calculate(int contract, DateTime from, DateTime to)
      {
        var ocont = context.Contracts.Where(c => c.id == contract).SingleOrDefault();

        var pu = new PriceUpdaterLib.PriceUpdater(context);
        
        decimal deuda = 0;
        
        deuda = pu.CreditBalanceFor(ocont.lritid, from, to);

        return Content(((decimal)deuda).ToString(CultureInfo.InvariantCulture));
      }

      

      public ActionResult Remove(int id)
      {
        FlashOK("La factura fue eliminada correctamente");

        int emireci = 0;
        try
        {
          var invoice = context.Invoices.Where(i => i.id == id).SingleOrDefault();
          emireci = invoice.emitidarecibida;
          context.Invoices.DeleteOnSubmit(invoice);
          context.SubmitChanges();
        }
        catch (Exception ex)
        {
          FlashError(string.Format("Imposible eliminar la factura: {0}", ex.Message));
        }

        return List(emireci);
      }

      public ActionResult Edit(int id)
      {
        var invoice = context.Invoices.Where(i => i.id == id).SingleOrDefault();

        ViewData["title"] = string.Format("Editando factura {2} {3} {1} nro. {0} ", invoice.invoiceNumber, invoice.Contract.name, invoice.emitidarecibida == 0 ? "emitida" : "recibida", invoice.emitidarecibida == 0 ? "a" : "de");
        MakeCombos();

        return View("New", invoice);
      }

      public ActionResult New(int emireci, int? contract)
      {
        var invoice = new Invoice();
        var country = "";

        //Si es emitida siempre viene con contract
        //Generate invoice number
        string invoiceNumber = string.Empty;
        if (emireci == 0)
        {
          var counter = context.InvoiceCounters.SingleOrDefault();
          invoiceNumber = string.Format("0001-{0:000000000}", counter.nextInvoice++);
          context.SubmitChanges();
        }

        if (contract != null)
        {
          //Ponemos el pais al que le vamos a facturar
          var ocont = context.Contracts.Where(c => c.id == (int)contract).SingleOrDefault();
          country = ocont.name;
          invoice.contract_id = (int)ocont.id;
          invoice.dateFrom = ocont.lastInvoice != null ? (DateTime)ocont.lastInvoice : new DateTime(2011, 1, 1);

          ViewData["show_calc"] = true;
          ViewData["ocont"] = ocont;
        }
        else
        {
          invoice.dateFrom = DateTime.UtcNow;
        }
        
        invoice.dateTo        = DateTime.Today;
        invoice.isueDate      = DateTime.Today;
        invoice.currency      = "USD";
        invoice.invoiceNumber = invoiceNumber;

        ViewData["title"]     = string.Format( emireci == 0 ? "Facturando a {0}" : "Nueva Factura Recibida", country);
        ViewData["emireci"]   = emireci;
        
        MakeCombos();
        return View(invoice);
      }

      public ActionResult Create(Invoice invoice)
      {
        if (!ModelState.IsValid)
        {
          MakeCombos();
          FlashError("Error intentando guardar, verifique los campos");
          return View("New", invoice);
        }

        try
        {
          if (invoice.id == 0)
          {
            FlashOK("La factura fue creada correctamente");

            context.Invoices.InsertOnSubmit(invoice);
            context.SubmitChanges();

            //Update contracts last facturado
            DateTime? maxDate = context.Invoices.Where(i => i.contract_id == invoice.contract_id && i.emitidarecibida == invoice.emitidarecibida).Max(i => (DateTime?)i.dateTo);
            if (maxDate != null)
            {
              //Save last date
              var contract = context.Contracts.Where(c => c.id == invoice.contract_id).SingleOrDefault();
              if( invoice.emitidarecibida == 0 )
                contract.lastInvoice = (DateTime)maxDate;
              else
                contract.lastInvoiceRecv = (DateTime)maxDate;

              context.SubmitChanges();

              //Recalculate contract account
              var pu = new PriceUpdaterLib.PriceUpdater(Config.ConnectionString);
              pu.UpdateContracts((DateTime)maxDate, new string[] { invoice.Contract.lritid });
            }

          }
          else
          {
            FlashOK("La factura se actualizo con exito");

            var updatedInvoice = context.Invoices.Where(c => c.id == invoice.id).SingleOrDefault();
            updatedInvoice.SimpleCopyFrom(invoice, new string[] { "state", "contract_id", "invoiceNumber", "isueDate", "dateFrom", "dateTo", "amount", "currency", "transfercost", "interests", "bankreference", "notes" });

            //Process file
            if (Request.Files.Count != 0)
            {
              var file = Request.Files[0];
              if (file.ContentLength > 0)
              {
                byte[] tmp = new byte[file.ContentLength];
                file.InputStream.Read(tmp, 0, file.ContentLength);
                if( updatedInvoice.invoiceFile_id == null )
                  updatedInvoice.InvoiceFile = new InvoiceFile();

                updatedInvoice.InvoiceFile.fileContent = tmp;
                updatedInvoice.InvoiceFile.fileName = Path.GetFileName(file.FileName);
                updatedInvoice.InvoiceFile.fileType = file.ContentType;
              }
            }


            context.SubmitChanges();
          }

          return Edit(invoice.id);
        }
        catch (Exception ex)
        {
          FlashError(string.Format("Error inesperado: {0}", ex.Message));
        }
        
        MakeCombos();
        return View("New", invoice);
      }

      public ActionResult print(int id)
      {
        var invoice = context.Invoices.Where(i => i.id == id).SingleOrDefault();
        return View(invoice);
      }
        
      public ActionResult ListJSON(int page, int rows, string sidx, string sord, int emireci)
      {
        var columns = new string[] { "id", "contract_id", "invoiceNumber", "isueDate", "currency", "amount", "state", "invoiceFile_id" };
        JQGrid.Helper.SaveJQState("invoice"+emireci, columns, new Dictionary<string, string> { { "page", page.ToString() }, { "rows", rows.ToString() }, { "sidx", sidx }, { "sord", sord } }, Request, Session);

        return Json(JQGrid.Helper.Paginate(
                      context.Invoices.Where(i => i.emitidarecibida==emireci),
                      Request.Params,
                      columns,
                      page, rows, sidx, sord), JsonRequestBehavior.AllowGet);
      }

      private void MakeCombos()
      {
        var pairs = new List<object>();
        foreach (var contract in context.Contracts)
        {
          pairs.Add(new { @id = contract.id.ToString(), @value = contract.name });
        }
        ViewData["lrit_countries"] = pairs.ToArray();

        ComboEstadoFor("invoice_states_emi"   , new string[] { "Emitida", "Enviada", "Cobrada", "Rechazada", "Vencida" });
        ComboEstadoFor("invoice_states_reci"  , new string[] { "Pendiente", "Pagada", "Rechazada", "Vencida" });
      }

      void ComboEstadoFor(string comboname, string[] states)
      {
        ComboEstadoFor(ViewData, comboname, states, 0);
      }

      public static void ComboEstadoFor(ViewDataDictionary vd, string comboname, string[] states, int start)
      {
        int cnt = start;
        var pairs2 = new List<object>();
        foreach (var state in states)
        {
          pairs2.Add(new { @id = cnt.ToString(), @value = state });
          cnt++;
        }
        
        vd[comboname] = pairs2.ToArray();
      }

    }
}
