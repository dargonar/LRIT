using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Transactions;
using System.Globalization;

using DataCenterDataAccess;

using log4net;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace DataCenterLogic
{
    class PricingImportHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PricingImportHelper));

        internal void Import(MemoryStream s)
        {
          var zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(s);
          Stream stream = zipFile.GetInputStream(0);

          DBDataContext context = null;
          try
          {
            XmlDocument ddpxml = new XmlDocument();
            ddpxml.Load(stream);

            context = new DBDataContext(Config.ConnectionString);

            //---------------------------------------------------
            XmlNamespaceManager nsmanager = new XmlNamespaceManager(ddpxml.NameTable);
            nsmanager.AddNamespace("pr", "http://gisis.imo.org/XML/LRIT/pricingFile/2008");
            //---------------------------------------------------

            var prices = new List<Price>();
            foreach (XmlNode price in ddpxml.SelectNodes("/pr:PricingFile/pr:PriceList", nsmanager))
            {
              var p = new Price();
              
              //Effective date
              p.effectiveDate         = DateTime.Parse(price.Attributes["effectiveDate"].Value, CultureInfo.InvariantCulture);

              //Issue date
              p.issueDate = null;
              if (price.Attributes["issueDate"] != null)
                p.issueDate = DateTime.Parse(price.Attributes["issueDate"].Value, CultureInfo.InvariantCulture);
              
              //Datacenter ID
              p.dataCentreID = price.Attributes["dataCentreID"].Value;
              
              //Data Providers (list)
              if (price["dataProviderID"] != null)
              {
                foreach (string dataProvider in price["dataProviderID"].InnerText.Split(' '))
                {
                  var pup = new PriceUserProvider();
                  pup.dataProviderID = dataProvider;
                  p.PriceUserProviders.Add(pup);
                }
              }

              var bd = price["BreakDown"];
              p.currency = bd.Attributes["currency"].Value;
              p.ArchivePositionReport = decimal.Parse(bd["ArchivePositionReport"].InnerText , CultureInfo.InvariantCulture);
              p.PeriodicRateChange = decimal.Parse(bd["PeriodicRateChange"].InnerText, CultureInfo.InvariantCulture);
              p.Poll = decimal.Parse(bd["Poll"].InnerText, CultureInfo.InvariantCulture);
              p.PositionReport = decimal.Parse(bd["PositionReport"].InnerText, CultureInfo.InvariantCulture);

              prices.Add(p);
            }

            var pdao = new PricesDataAccess(context);
            pdao.Create(prices.ToArray());
          
          }
          catch (Exception ex)
          {
            if (context != null)
              context.Dispose();

            throw new Exception("Unable to Import Pricing File", ex);
          }
          finally
          {
            if (context != null)
              context.Dispose();
          }

        }
    }
}
