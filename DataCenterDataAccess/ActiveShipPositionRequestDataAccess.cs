using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DataCenterDataAccess
{
  public class ActiveShipPositionRequestDataAccess : BaseDataAccess
  {
    public ActiveShipPositionRequestDataAccess() : base(){}

    public ActiveShipPositionRequestDataAccess(DBDataContext context) : base(context) { }


    /// <summary>
    /// Retorna el periodo minimo de poleo para un barco determinado.
    /// Todos los request activos de el barco son examinado.
    /// </summary>
    /// <param name="IMONum">El numero IMO del barco</param>
    /// <param name="sprId">El request actual que debe ser omitido en la comparacion</param>
    /// <returns>El requestType o -1 si no hay request activos para ese barco.</returns>
    public int GetMinPeriodicRequestTypeForShip(string IMONum, int sprId)
    {
      //DataLoadOptions options = new DataLoadOptions();
      //options.LoadWith<ActiveShipPositionRequest>(c => c.ShipPositionRequest);
      //context.LoadOptions = options;

      var minReq = context.ActiveShipPositionRequests.Where(aspr =>
                (
                  ( aspr.ShipPositionRequest.Id != sprId) && 
                  ( aspr.ShipPositionRequest.IMONum == IMONum) &&
                  ( new int[]{2,3,4,5,6,10,11}.Contains(aspr.ShipPositionRequest.RequestType) )
                )
              ).OrderBy(res => res.ShipPositionRequest.RequestType).Take(1).SingleOrDefault();

      if (minReq == null)
        return -1;

      return minReq.ShipPositionRequest.RequestType;
    }


    public void AddOrReplace(ShipPositionRequest spr)
    {
      var aborrar = context.ActiveShipPositionRequests.Where<ActiveShipPositionRequest>
        (c => c.ShipPositionRequest.AccessType == spr.AccessType
          && c.ShipPositionRequest.IMONum == spr.IMONum
          && spr.DataUserRequestor == c.ShipPositionRequest.DataUserRequestor
        );
      
      context.ActiveShipPositionRequests.DeleteAllOnSubmit(aborrar);

      //Esto va aca??
      ActiveShipPositionRequest newActiveRequest = new ActiveShipPositionRequest();
      newActiveRequest.LastMessage = "Added";
      newActiveRequest.RequestId = spr.Id;
      newActiveRequest.Status = 0;
      newActiveRequest.LastTime = DateTime.UtcNow;

      context.ActiveShipPositionRequests.InsertOnSubmit(newActiveRequest);
      context.SubmitChanges();

    }






    /// <summary>
    /// Crea un nuevo request activo en la base de datos.
    /// </summary>
    /// <param name="activeShipPositionRequest">El request activo a crear.</param>
    public void Create(ActiveShipPositionRequest activeShipPositionRequest)
    {

        context.ActiveShipPositionRequests.InsertOnSubmit(activeShipPositionRequest);
        context.SubmitChanges();
    }
    
    /// <summary>
    /// Obtiene todos los request activos de la base de datos.
    /// </summary>
    /// <returns>Lista con los request activos</returns>
    public List<ActiveShipPositionRequest> GetAll()
    {
 
        DataLoadOptions options = new DataLoadOptions();
        options.LoadWith<ActiveShipPositionRequest>(c => c.ShipPositionRequest);
        context.LoadOptions = options;
        return context.ActiveShipPositionRequests.ToList();
 
    }

    public void Remove(ActiveShipPositionRequest aspr)
    {
 
        context.ActiveShipPositionRequests.DeleteOnSubmit(
          context.ActiveShipPositionRequests.SingleOrDefault(
           c => c.Id == aspr.Id
          )
        );
        context.SubmitChanges();
 
    }
    /// <summary>
    /// Borra un requerimiento activo tomando en cuenta el AccessType, IMONum y DataUserRequestor
    /// </summary>
    /// <param name="spr">ShipPositionRequest</param>
    public void Remove(ShipPositionRequest spr)
    {
        context.ActiveShipPositionRequests.DeleteAllOnSubmit(
          context.ActiveShipPositionRequests.Where<ActiveShipPositionRequest>
          ( c => 
            c.Status == 1
            && c.ShipPositionRequest.AccessType == spr.AccessType
            && c.ShipPositionRequest.IMONum == spr.IMONum 
            && spr.DataUserRequestor == c.ShipPositionRequest.DataUserRequestor
          )
        );
        context.SubmitChanges();
 
    }

    /// <summary>
    /// Borra todas los requerimientos activos para un mismo UserRequestor
    /// </summary>
    /// <param name="spr">ShipPositionRequest</param>
    public void RemoveAllForRequestor(ShipPositionRequest spr)
    {
        context.ActiveShipPositionRequests.DeleteAllOnSubmit(
          context.ActiveShipPositionRequests.Where<ActiveShipPositionRequest>
          (c => spr.DataUserRequestor == c.ShipPositionRequest.DataUserRequestor));
        
        context.SubmitChanges();
     }



    /// <summary>
    /// Cambia el estado de un request activo.
    /// </summary>
    /// <param name="asprId">Id del request activo</param>
    /// <param name="state">Nuevo estado</param>
    /// <param name="dateTime">Fecha de ultima actualizacion</param>
    public void Update(int asprId, int state, DateTime dateTime)
    {
        ActiveShipPositionRequest aspr = context.ActiveShipPositionRequests.SingleOrDefault(c => c.Id == asprId);
        aspr.Status = state;
        aspr.LastTime = dateTime;
        context.SubmitChanges();
    }

    public bool IsStopRequired(Ship Ship, string LRITId)
    {
        DataLoadOptions options = new DataLoadOptions();
        options.LoadWith<ActiveShipPositionRequest>(c => c.ShipPositionRequest);
        ActiveShipPositionRequest aspr = context.ActiveShipPositionRequests.FirstOrDefault(c => c.ShipPositionRequest.IMONum == Ship.IMONum && c.ShipPositionRequest.RequestType == 8 && c.ShipPositionRequest.AccessType == 1 && c.ShipPositionRequest.DataUserRequestor == LRITId);
        if (aspr == null)
          return false;
        else return true;
     }



    public int Count()
    {
        return context.ActiveShipPositionRequests.Count();
    }



  }
}
