using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de StandingOrder
  /// </summary>
  public class StandingOrderDataAccess : BaseDataAccess
  {
    public StandingOrderDataAccess() : base() { }
    public StandingOrderDataAccess(DBDataContext context) : base(context) {}

    /// <summary>
    /// Crea un nuevo StandingOrder en base de datos
    /// </summary>
    /// <param name="standingOrders">Lista de StandingOrders</param>
    public void Create(StandingOrder[] standingOrders)
    {
        context.StandingOrders.InsertAllOnSubmit(standingOrders);
        context.SubmitChanges();
    }

    public List<StandingOrder> GetAll()
    {
        return context.StandingOrders.ToList();
    }

    public ContractingGoverment GetContractingGovermentItBelongs(StandingOrder order)
    {
        DataLoadOptions options = new DataLoadOptions();
        options.LoadWith<DDPVersion>(s => s.ContractingGoverments);
        options.LoadWith<Place>(p => p.ContractingGoverment);
        context.LoadOptions = options;

        return context.ContractingGoverments.Where(c => c.Id == order.Place.ContractingGoverment.Id).SingleOrDefault();
    }

    public List<StandingOrder> GetOrdersForPosition(ShipPosition pos, DDPVersion ddpVersion)
    {
        DataLoadOptions options = new DataLoadOptions();
        options.LoadWith<StandingOrder>(s => s.Place);
        options.LoadWith<Place>(p => p.ContractingGoverment);
        context.LoadOptions = options;

        return context.ExecuteQuery<StandingOrder>(
                        "SELECT s.Id, s.PlaceId FROM StandingOrder as s INNER JOIN Place on Place.Id = s.PlaceId" + Environment.NewLine +
					              "INNER JOIN ContractingGoverment on Place.ContractingGovermentId = ContractingGoverment.Id" +Environment.NewLine+
					              "INNER JOIN DDPVersion on (DDPVersion.Id = ContractingGoverment.DDPVersionId"               +Environment.NewLine+
						            "AND DDPVersion.Id = {0})"                                                                  +Environment.NewLine+
                        "WHERE (geography::STGeomFromWKB(Place.Area, 4326).STIntersects("                           +Environment.NewLine+
                        "geography::STGeomFromWKB((SELECT Position FROM ShipPosition WHERE ShipPosition.Id = {1}),4326))) != 0", ddpVersion.Id, pos.Id).ToList();
    }
  }
}
