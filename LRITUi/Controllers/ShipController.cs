using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.Mvc;
using System.Data.Linq;
using DataCenterLogic;
using DataCenterDataAccess;
using LRITUi;

namespace LRITUi.Controllers
{
  [Authorize]
    public class ShipController : MyController
    {
        //
        // GET: /Ship/
        public ActionResult List()
        {
          var sda = new ShipDataAccess(context);
          ViewData["barcos"] = sda.GetAll();
          return View();
        }

        public ActionResult New()
        {
          return View(new Ship());
        }

        public ActionResult Create([Bind(Exclude = "Id, Manga, Eslora, Calado")] Ship ship, int? shipid, string Manga, string Eslora, string Calado)
        {
          ship.ASPId = 1;
          var sda = new ShipDataAccess(context);

          

          if( ModelState.IsValid == false )
          {
            foreach (var val in ModelState.Values.ToList())
              val.Errors.Clear();

            //ModelState.AddModelError("", "Decimales deben ir separados por " + Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
          }

          if (ship.Name.Trim().Length == 0)
            ModelState.AddModelError("Name", "Nombre requerido");

          if (ship.IMONum.Trim().Length == 0)
            ModelState.AddModelError("IMONum", "IMOnum requerido");
                   
          if (ship.EquipID.Trim().Length == 0)
            ModelState.AddModelError("EquipID", "ID del equipo requerido");

          if (ship.MMSINum.Trim().Length == 0)
            ModelState.AddModelError("MMSINum", "Numero MMSI requerido");

          if (ship.DNID.Trim().Length == 0)
            ModelState.AddModelError("DNID", "Numero DNID requerido");

          if (ship.Member == 0)
            ModelState.AddModelError("Member", "Numero Miembro requerido");

          //if (sda.getByMemberNum(ship.Member) != null)
          //  ModelState.AddModelError("Member", "Ya hay otro barco con el mismo Member Number");
                    
          if (ship.Mobile.Trim().Length == 0)
            ModelState.AddModelError("Mobile", "Numero Mobile requerido");

          //if (sda.getByMobileNum(ship.Mobile) != null)
          //  ModelState.AddModelError("Member", "Ya hay otro barco con el mismo Mobile Number");


          if (!ModelState.IsValid)
            return View("New", ship);

          try
          {
            if (shipid != null)
            {
              
              sda.Update(ship, (int)shipid, Manga, Eslora, Calado );
              return RedirectToAction("List");
            }

            Eslora = Eslora.Replace(',', '.');
            if (Eslora != "")
              ship.Eslora = Math.Round(float.Parse(Eslora), 2);

            Manga = Manga.Replace(',', '.');
            if (Manga != "")
              ship.Manga = Math.Round(float.Parse(Manga), 2);

            Calado = Calado.ToString().Replace(',', '.');
            if (Calado != "")
              ship.Calado = Math.Round(decimal.Parse(Calado), 2);


            sda.Insert(ship);
            return RedirectToAction("List");
          }
          catch(Exception ex)
          {
            ModelState.AddModelError("", ex.Message);
            return View("New", ship);
          }

        }

        public ActionResult Edit(int id)
        {
          var sda = new ShipDataAccess(context);
          ViewData["shipid"] = id;
          return View("New", sda.getById(id));
        }

        
        public ActionResult Delete(int id)
        {
          var sda = new ShipDataAccess(context);
          sda.Delete(sda.getById(id));
          return RedirectToAction("List");
        }

    }
}
