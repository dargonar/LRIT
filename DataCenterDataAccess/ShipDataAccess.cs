using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de Ship
  /// </summary>
  public enum ShipStatus { Error = -1, Ok = 0, Polling = 1 }

  public class ShipDataAccess : BaseDataAccess
  {
    public ShipDataAccess() : base() { }
    public ShipDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Obtiene un Ship basado en ID de base de datos
    /// </summary>
    /// <param name="id">ID de base de datos del Ship</param>
    /// <returns>Ship</returns>
    public Ship getById(int id)
    {
        Ship ship = context.Ships.Where ( s => s.Id == id ).ToList()[0];
        if( ship != null )
        {
          string s = ship.ASP.ToString();
        }
        return ship;
    }
    /// <summary>
    /// Obtiene un Ship basado en el numero IMO del Ship
    /// </summary>
    /// <param name="IMONum">Numero IMO</param>
    /// <returns>Ship</returns>
    public Ship getByIMONum(string IMONum)
    {
        DataLoadOptions options = new DataLoadOptions();
        options.LoadWith<Ship>(c => c.ASP);
        context.LoadOptions = options;

        Ship ship = context.Ships.SingleOrDefault(s => s.IMONum == IMONum);
        return ship;
    }

    public Ship getByMemberNum(int member)
    {
      Ship ship = context.Ships.SingleOrDefault(s => s.Member == member);
      return ship;
    }

    public Ship getByMobileNum(string mobile)
    {
      Ship ship = context.Ships.SingleOrDefault(s => s.Mobile == mobile);
      return ship;
    }

    public List<Ship> GetAll()
    {
      DataLoadOptions options = new DataLoadOptions();
      options.LoadWith<Ship>(c => c.ASP);
      context.LoadOptions = options;
      return context.Ships.OrderBy(s => s.Name).ToList();
    }

    public List<ShipPositionRequest> GetAllRequestsForShip(Ship ship)
    {
        return context.ShipPositionRequests.Where(s => s.IMONum == ship.IMONum).ToList();
    }

    public int HasActiveRequest(Ship ship)
    {
        return context.ActiveShipPositionRequests.Count(a => a.ShipPositionRequest.IMONum == ship.IMONum);//   .Where(s => s.IMONum == ship.IMONum).ToList();
    }


    public void Insert(Ship ship)
    {
      context.Ships.InsertOnSubmit(ship);
      context.SubmitChanges();
    }

    public void Update(Ship ship, int id, string manga, string eslora, string calado)
    {

      var oldship = context.Ships.SingleOrDefault(s => s.Id == id);

      if (oldship != null)
      {
        oldship.Name = ship.Name;
        oldship.IMONum = ship.IMONum;
        oldship.EquipID = ship.EquipID;
        oldship.MMSINum = ship.MMSINum;

        eslora = eslora.Replace(',', '.');
        if (eslora != "")
          oldship.Eslora = Math.Round(float.Parse(eslora), 2);

        manga = manga.Replace(',', '.');
        if (manga != "")
          oldship.Manga = Math.Round(float.Parse(manga), 2);

        calado = calado.ToString().Replace(',', '.');
        if (calado != "")
          oldship.Calado = Math.Round(decimal.Parse(calado), 2);
        
        oldship.Tipo_Navegacion = ship.Tipo_Navegacion;
        oldship.Tipo_Navegacion_Ingles = ship.Tipo_Navegacion_Ingles;
        oldship.Tipo_Buque = ship.Tipo_Buque;
        oldship.Tipo_Buque_Ingles = ship.Tipo_Buque;

        context.SubmitChanges();
      }

      return;

    }
    
    public void Delete(Ship ship)
    {
        ship = context.Ships.SingleOrDefault(s => s.Id == ship.Id);
        context.Ships.DeleteOnSubmit(ship);
        context.SubmitChanges();
    }

    public void ChangeShipStatus(ShipStatus status, Ship ship)
    {
      ship = context.Ships.SingleOrDefault(s => s.Id == ship.Id);
      ship.EstadoAsp = status.ToString();
      ship.EstadoAspFecha = DateTime.UtcNow;
      context.SubmitChanges();
    }

    public string GetShipStatus(Ship ship)
    {
      return context.Ships.SingleOrDefault(s => s.Id == ship.Id).EstadoAsp;
    }

    //TODO: cambiar nombre
    public List<Ship> GetAllBle()
    {
      //DataLoadOptions options = new DataLoadOptions();
      //options.LoadWith<Ship>(c => c.ASP);
      //context.LoadOptions = options;
      return context.Ships.Where(s => s.EstadoAsp != "Ok" && DateTime.UtcNow.AddMinutes(-60) > s.EstadoAspFecha).ToList();
    }

    public Ship getByISN(string p)
    {
        Ship ship = context.Ships.SingleOrDefault(s => s.Mobile == p);
        return ship;
    }
  }
}
