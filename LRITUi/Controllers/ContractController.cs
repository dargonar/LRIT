using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using DataCenterDataAccess;
using JQGrid;
using LRITUi.Utils;

namespace LRITUi.Controllers
{
  [Authorize(Roles = "Administrador, Facturador")]
  public class ContractController : MyController
  {
    public ActionResult List()
    {
      return View();
    }

    public ActionResult Remove(int id)
    {
      FlashOK("El contrato fue eliminado correctamente");

      try
      {
        var contract = context.Contracts.Where(c => c.id == id).SingleOrDefault();
        context.Contracts.DeleteOnSubmit(contract);
        context.SubmitChanges();
        return View("List");
      }
      catch(Exception ex)
      {
        FlashError(string.Format("Imposible eliminar el contrato: {0}",ex.Message));
      }
      
      return View("List");
    }

    public ActionResult Edit(int id)
    {
      ViewData["title"] = "Edición de contrato";
      var contract = context.Contracts.Where(c => c.id == id).SingleOrDefault();
      return View("New", contract);
    }

    public ActionResult New()
    {
      ViewData["title"] = "Nuevo contrato";
      var contract = new Contract();
      return View(contract);
    }

    public ActionResult Create(Contract contract)
    {
      try
      {
        if (contract.id == 0)
        {
          context.Contracts.InsertOnSubmit(contract);
          FlashOK("El contrato fue creado con exito");
        }
        else
        {
          var updatedContract = context.Contracts.Where(c => c.id == contract.id).SingleOrDefault();
          updatedContract.SimpleCopyFrom(contract, new string[] {"lritid", "name", "minimun", "period", "lastInvoice"});

          FlashOK("El contrato fue modificado con exito");
        }

        context.SubmitChanges();
        return Edit(contract.id);
      }
      catch(Exception ex)
      {
        FlashError(string.Format("Error inesperado: {0}", ex.Message));
      }

      return View("New", contract);
    }

    public ActionResult ListJSON(int page, int rows, string sidx, string sord)
    {
      var columns = new string[] { "id", "lritid", "name", "lastInvoice", "credit_balance", "lastInvoiceRecv", "debit_balance", "period" };
      JQGrid.Helper.SaveJQState("contract", columns, new Dictionary<string, string> { { "page", page.ToString() }, { "rows", rows.ToString() }, { "sidx", sidx }, { "sord", sord } }, Request, Session);
      
      return Json(JQGrid.Helper.Paginate(
                    context.Contracts, 
                    Request.Params,
                    columns,
                    page, rows, sidx, sord), JsonRequestBehavior.AllowGet);
    }

  }
}
