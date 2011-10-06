using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterLogic
{
  /// <summary>
  /// Clase de ayuda para los mensajes de tipo ShipPositionRequest ( 4 y 5 )
  /// </summary>
  public partial class ShipPositionRequestHelper
  {
    /// <summary>
    /// Verifica si el pedido es para datos historicos.
    /// </summary>
    /// <param name="requestTypeType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es para datos historicos, falso otra cosa</returns>
    static public bool IsHistoricRequest(DataCenterLogic.DataCenterTypes.requestTypeType requestType)
    {
      if (requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item7 ||     //Archived LRIT information request
          requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item9)       //Most recent position report
      {
        return true;
      }
      return false; 	    
    }

    /// <summary>
    /// Verifica si el pedido es para datos periodicos
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es para datos periodicos, falso otra cosa</returns>
    static public bool IsPeriodicRequest(DataCenterLogic.DataCenterTypes.requestTypeType requestType)
    {
      if( requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item2 ||   //15 minute periodic rate
          requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item3 ||   //30 minute periodic rate
          requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item4 ||   //1 hour periodic rate
          requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item5 ||   //3 hour periodic rate
          requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item6 ||   //6 hour periodic rate
          requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item10 ||  //12 hour periodic rate
          requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item11 )   //24 hour periodic rate
      {
        return true;    
      }
      return false;
    }
    
    /// <summary>
    /// Verifica si el pedido es para datos periodicos
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es para datos periodicos, falso otra cosa</returns>
    static public bool IsPeriodicRequest(DataCenterLogic.DataCenterTypesIDE.requestTypeType requestType)
    {
      if (requestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item2 ||   //15 minute periodic rate
          requestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item3 ||   //30 minute periodic rate
          requestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item4 ||   //1 hour periodic rate
          requestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item5 ||   //3 hour periodic rate
          requestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item6 ||   //6 hour periodic rate
          requestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item10 ||  //12 hour periodic rate
          requestType == DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item11)    //24 hour periodic rate
      {
        return true;
      }
      return false;
    }
    /// <summary>
    /// Verifica si el pedido es para datos periodicos
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es para datos periodicos, falso otra cosa</returns>
    static public bool IsPeriodicRequest(int requestType)
    {
      return IsPeriodicRequest((DataCenterLogic.DataCenterTypes.requestTypeType)requestType);
    }

    /// <summary>
    /// Obtiene la cantidad de minutos de un request type de tipo periodico
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Cantidad de minutos o -1 si el tipo de request no es periodico</returns>
    internal static int GetMinutes(int requestType)
    {
      return GetMinutes((DataCenterLogic.DataCenterTypes.requestTypeType)requestType);
    }

    /// <summary>
    /// Obtiene la cantidad de minutos de un request type de tipo periodico
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Cantidad de minutos o -1 si el tipo de request no es periodico</returns>
    internal static int GetMinutes(DataCenterLogic.DataCenterTypes.requestTypeType requestType)
    {
      if( !IsPeriodicRequest(requestType) )
        return -1;
      
      if ( requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item2 ) return 15;
      if ( requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item3 ) return 30;
      if ( requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item4 ) return 60;
      if ( requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item5 ) return 180;
      if ( requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item6 ) return 360;
      if ( requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item10 ) return 720;
      if ( requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item11) return 1440;
      
      return -1;
    }

    /// <summary>
    /// Verifica si el tipo de request es OneTimePoll
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es OneTimePoll, falso otra cosa</returns>
    public static bool IsOneTimePoll(int requestType)
    {
      return IsOneTimePoll((DataCenterLogic.DataCenterTypes.requestTypeType)requestType);
    }

    /// <summary>
    /// Verifica si el tipo de request es OneTimePoll
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es OneTimePoll, falso otra cosa</returns>
    public static bool IsOneTimePoll(DataCenterLogic.DataCenterTypes.requestTypeType requestType)
    {
      if (requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item1 ) //One time poll of ship
        return true;

      return false;      
    }
    /// <summary>
    /// Verifica si el tipo de request es MostRecentPosition
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es MostRecentPosition, falso otra cosa</returns>
    public static bool IsMostRecentPosition(int requestType)
    {
      return IsMostRecentPosition((DataCenterLogic.DataCenterTypes.requestTypeType)requestType);
    }

    /// <summary>
    /// Verifica si el tipo de request es MostRecentPosition
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es MostRecentPosition, falso otra cosa</returns>
    public static bool IsMostRecentPosition(DataCenterLogic.DataCenterTypes.requestTypeType requestType)
    {
      if (requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item9 ) //Most recent position report
        return true;

      return false;          
    }

    /// <summary>
    /// Verifica si el tipo de request es Reset
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es Reset, falso otra cosa</returns>
    public static bool IsResetRequest(int requestType)
    {
      return IsResetRequest((DataCenterLogic.DataCenterTypes.requestTypeType)requestType);
    }

    /// <summary>
    /// Verifica si el tipo de request es Reset
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es Reset, falso otra cosa</returns>
    public static bool IsResetRequest(DataCenterLogic.DataCenterTypes.requestTypeType requestType)
    {
      if (requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item0)
        return true;

      return false;
    }

    /// <summary>
    /// Verifica si el tipo de request es Stop
    /// </summary>
    /// <param name="requestType">Tipo de request</param>
    /// <returns>Verdadero si el pedido es Stop, falso otra cosa</returns>
    public static bool IsStopRequest(DataCenterLogic.DataCenterTypes.requestTypeType requestType)
    {
      if (requestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item8)
        return true;

      return false;
    }
  }
}
