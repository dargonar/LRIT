using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;
using System.Configuration;

using log4net;
using log4net.Config;


//Use web.config and check for modifications
[assembly: log4net.Config.XmlConfigurator(Watch=false)]

namespace DDPSimulator
{
  /// <summary>
  /// Summary description for DDPSimulator
  /// </summary>
  [WebServiceAttribute(Namespace = "http://gisis.imo.org/XML/LRIT/2008")]
  [WebServiceBindingAttribute(Name = "ddpServiceBinding", Namespace = "http://gisis.imo.org/XML/LRIT/2008")]
  [SoapRpcService(RoutingStyle=SoapServiceRoutingStyle.RequestElement)]

  public class DDPSimulator : IDdpServiceBinding
  {
    private log4net.ILog m_log = log4net.LogManager.GetLogger(typeof(DDPSimulator));

    DDPSimulator()
    {
      XmlConfigurator.Configure(new FileInfo(ConfigurationManager.AppSettings["Log4NetConfigFile"]));
    }

    #region IDdpServiceBinding Members

    public Response DDPRequest(DDPRequestType request)
    {
      string msg = string.Format(@"ArchivedDDPTimeStamp:{0},ArchivedDDPTimeStampSpecified:{1},ArchivedDDPVersionNum:{2},
                    DDPVersionNum:{3},MessageId:{4},Originator:{5}, schemaVersion:{6}",
                    request.ArchivedDDPTimeStamp ,
                    request.ArchivedDDPTimeStampSpecified ,
                    request.ArchivedDDPVersionNum ,
                    request.DDPVersionNum,
                    request.MessageId,
                    request.Originator  ,
                    request.schemaVersion );

      m_log.Info( msg );

      Response response = new Response();
      response.response = responseType.Success;
      return response;  
    }

    public Response Receipt(ReceiptType Receipt1)
    {
      m_log.Info( "Receitp called" );

      Response response = new Response();
      response.response = responseType.Success;
      return response;     
    }

    public Response SystemStatus(SystemStatusType status)
    {
      m_log.Info( "System status called" );

      Response response = new Response();
      response.response = responseType.Success;
      return response;     
    }

    #endregion
  }
}
