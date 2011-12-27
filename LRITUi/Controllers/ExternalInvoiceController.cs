using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace LRITUi.Controllers
{

  [Authorize(Roles = "ExternoVerificarFacturas")]
    public class ExternalInvoiceController : MyController
    {
        public IMembershipService MembershipService
        {
          get;
          private set;
        }

        public ActionResult List()
        {
          ViewData["title"] = "Invoice List for user " + User.Identity.Name;
          InvoiceController.ComboEstadoFor(ViewData, "invoice_states_emien" , new string[] { "Invoiced", "Paid", "Rejected" }, 1);
          return View();
        }

        public ActionResult ListJSON(int page, int rows, string sidx, string sord)
        {
          var columns = new string[] { "id", "invoiceNumber", "isueDate", "currency", "amount", "state" };

          return Json(JQGrid.Helper.Paginate(
                        context.Invoices.Where(i => i.Contract.lritid == User.Identity.Name && new int[] {1,2,3}.Contains(i.state) ),
                        Request.Params,
                        columns,
                        page, rows, sidx, sord), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Password()
        {
          ViewData["title"] = "Change password for user " + User.Identity.Name;
          return View();
        }

        public ActionResult Change(string pass1, string pass2)
        {
          MembershipService = new AccountMembershipService();

          if( ValidatePasswords(pass1, pass2) == false)
            return View("Password");

          if (MembershipService.ChangePassword(User.Identity.Name, "", pass1) == false)
          {
            FlashError("Error trying to change password");
            return View("Password");
          }
          
          FlashOK("Password has been changed");
          return View("Password");
        }

        private bool ValidatePasswords(string pass1, string pass2)
        {
          if (pass1 == null || pass1.Length < MembershipService.MinPasswordLength)
          {
            FlashError(string.Format("Password must be at least {0} characters long", MembershipService.MinPasswordLength));
            return false;
          }

          if (!String.Equals(pass1, pass2, StringComparison.Ordinal))
          {
            FlashError("Password mismatch");
            return false;
          }

          return true;
        }

    }

}
