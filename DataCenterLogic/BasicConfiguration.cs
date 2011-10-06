using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DataCenterLogic
{
    /// <summary>
    /// Agrupa la configuracion necesaria para cada componente del LRIT
    /// </summary>
    public class BasicConfiguration
    {
        /// <summary>
        /// Indica si el DC esta en modo debug
        /// </summary>
        protected string mDCDebug = string.Empty;
        public string DCDebug
        {
          get { return mDCDebug; }
          set { mDCDebug = value; }
        }

        /// <summary>
        /// String de conexion a la base de datos
        /// </summary>
        protected string mConnectionString  = string.Empty;
        public string ConnectionString
        {
          get { return mConnectionString; }
          set { mConnectionString = value; }
        }

        /// <summary>
        /// Nombre de la cola de salida para CORE
        /// </summary>
        protected string mCoreOutQueue = string.Empty;
        public string CoreOutQueue
        {
            get { return mCoreOutQueue; }
            set { mCoreOutQueue = value; }
        }

        /// <summary>
        /// Nombre de la cola de entrada para CORE
        /// </summary>
        protected string mCoreInQueue = string.Empty;
        public string CoreInQueue
        {
            get { return mCoreInQueue; }
            set { mCoreInQueue = value; }
        }

        /// <summary>
        /// URL del DataCenter
        /// </summary>
        protected string mDCUrl = string.Empty;
        public string DCUrl
        {
          get { return mDCUrl; }
          set { mDCUrl = value; }
        }

        /// <summary>
        /// URL del IDE Server
        /// </summary>
        protected string mIDEUrl = string.Empty;
        public string IDEUrl
        {
          get { return mIDEUrl; }
          set { mIDEUrl = value; }
        }

        /// <summary>
        /// URL del DDP Server
        /// </summary>
        protected string mDDPURL = string.Empty;
        public string DDPURL
        {
          get { return mDDPURL; }
          set { mDDPURL = value; }
        }


        public static BasicConfiguration FromNameValueCollection( NameValueCollection nvc )
        {
          BasicConfiguration basicConfiguration = new BasicConfiguration();

          basicConfiguration.ConnectionString = nvc["ConnectionString"];
          basicConfiguration.CoreInQueue      = nvc["CoreInQueue"];
          basicConfiguration.CoreOutQueue     = nvc["CoreOutQueue"];
          basicConfiguration.DCUrl            = nvc["DCUrl"];
          basicConfiguration.DDPURL           = nvc["DDPURL"];
          basicConfiguration.IDEUrl           = nvc["IDEUrl"];
          basicConfiguration.DCDebug          = nvc["DCDebug"];
          
          return basicConfiguration;
        }
    }
}
