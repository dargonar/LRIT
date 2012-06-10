using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Text.RegularExpressions;

namespace JournalFileBuilder
{
  class Builder
  {
    static void Main(string[] args)
    {
      if (args.Length != 5)
      {
        usage();
        return;
      }

      var b = new Builder();
      b.run(args[0], args[1], args[2], args[3], args[4]);
    }
    private string shipreport_t = @"<PositionReport positionSent=""{0}"">
  <ns1:ShipPositionReport test=""0"" schemaVersion=""1.3"">
    <ns1:Latitude>{1}</ns1:Latitude>
    <ns1:Longitude>{2}</ns1:Longitude>
    <ns1:TimeStamp1>{3}</ns1:TimeStamp1>
    <ns1:ShipborneEquipmentId>{4}</ns1:ShipborneEquipmentId>
    <ns1:ASPId>{5}</ns1:ASPId>
    <ns1:MessageType>{6}</ns1:MessageType>
    <ns1:MessageId>{7}</ns1:MessageId>
    <ns1:ReferenceId>{8}</ns1:ReferenceId>
    <ns1:IMONum>{9}</ns1:IMONum>
    <ns1:MMSINum>{10}</ns1:MMSINum>
    <ns1:TimeStamp2>{11}</ns1:TimeStamp2>
    <ns1:TimeStamp3>{12}</ns1:TimeStamp3>
    <ns1:DCId>{13}</ns1:DCId>
    <ns1:TimeStamp4>{14}</ns1:TimeStamp4>
    <ns1:TimeStamp5>{15}</ns1:TimeStamp5>
    <ns1:ResponseType>{16}</ns1:ResponseType>
    <ns1:DataUserRequestor>{17}</ns1:DataUserRequestor>
    <ns1:ShipName>{18}</ns1:ShipName>
    <ns1:DataUserProvider>{19}</ns1:DataUserProvider>
    <ns1:DDPVersionNum>{20}</ns1:DDPVersionNum>
  </ns1:ShipPositionReport>
</PositionReport>";

    private static string MSSQLQUERY = "\n\n\n\nquery-----\n"+
@"SELECT   (select top 1 regularVer+':'+inmediateVer from DDPVersion
			where published_at <= sp.TimeStampInDC
			order by published_at desc, received_at desc
          ) ddpver,
          ISNULL(spr.Id,0) enviado,  geography::STGeomFromWKB(sp.Position, 4326).STAsText() as postext,  *


from ShipPosition sp
LEFT JOIN Ship S ON sp.ShipId=S.ID
LEFT join ShipPositionReport spr on s.IMONum=spr.IMONum and sp.TimeStamp=spr.TimeStamp1
  
 where 
  sp.TimeStamp >= CAST('2012-01-01 00:00' AS DATETIME)
  and sp.TimeStamp <= CAST('2012-30-01 23:59' AS DATETIME)
  order by sp.TimeStamp

for xml path('report'),root('reports')";

    private string header = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<LritMessageLog xmlns:ns1=""http://gisis.imo.org/XML/LRIT/positionReport/2008""
  xmlns:ns2=""http://gisis.imo.org/XML/LRIT/ddpNotification/2008""
  xmlns:ns3=""http://gisis.imo.org/XML/LRIT/ddpUpdate/2008""
  xmlns:ns4=""http://gisis.imo.org/XML/LRIT/ddpRequest/2008""
  xmlns:ns5=""http://gisis.imo.org/XML/LRIT/pricingNotification/2008""
  xmlns:ns6=""http://gisis.imo.org/XML/LRIT/journalReport/2008""
  xmlns:ns7=""http://gisis.imo.org/XML/LRIT/types/2008""
  xmlns:ns8=""http://gisis.imo.org/XML/LRIT/pricingUpdate/2008""
  xmlns=""http://gisis.imo.org/XML/LRIT/2008""
  xmlns:ns10=""http://gisis.imo.org/XML/LRIT/systemStatus/2008""
  xmlns:ns11=""http://gisis.imo.org/XML/LRIT/pricingRequest/2008""
  xmlns:ns12=""http://gisis.imo.org/XML/LRIT/sarSurpicRequest/2008""
  xmlns:ns13=""http://gisis.imo.org/XML/LRIT/positionRequest/2008""
  xmlns:ns14=""http://gisis.imo.org/XML/LRIT/receipt/2008""
  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" + "\n";

    Regex ex0;
    Dictionary<string, string> dicto;
    HashSet<string> types = new HashSet<string>();

    private void run(string date0, string date1, string folder, string infile, string outfile)
    {
      var d0 = DateTime.ParseExact(date0, "yyyy-MM-dd", CultureInfo.InvariantCulture);
      var d1 = DateTime.ParseExact(date1, "yyyy-MM-dd", CultureInfo.InvariantCulture);
      //POSTA: "\<([\/]*)([^\s>]*)[^\>]*([\/]*)\>"
      //ex0 = new Regex(@"<([/]*)([^\s/>]*)[^>]*>", RegexOptions.Compiled);
      ex0 = new Regex(@"(<[/]*)([^\s>]*).*?([/]*>)", RegexOptions.Compiled);

      dicto = new Dictionary<string,string>() {
        {"priceRequest", "ns11"},  
        {"systemStatus", "ns10"},
        {"receipt", "ns14"},
        {"pricingUpdate", "ns8"},
        {"journalReport", "ns6"},
        {"shipPositionReport", "ns1"},
        {"ddpRequest", "ns4"},
        {"shipPositionRequest", "ns13"},
        {"SARSURPICRequest", "ns12"},
      };

      //var xxx = new XmlTextWriter("test2.xml", Encoding.UTF8);
      //xxx.WriteString(header);
      //xxx.Close();

      var writer = File.CreateText(outfile);
      writer.Write(header);

      while(d1 >= d0)
      {
        var subfolder = folder + @"\" + string.Format("{0:yyyyMMdd}", d0);
        if (Directory.Exists(subfolder))
        {
          //var infolder = subfolder + @"\in";
          var outfolder = subfolder + @"\out";

          //if (Directory.Exists(infolder))
          //processFolder(infolder, writer);

          if (Directory.Exists(outfolder))
            processFolder(outfolder, writer);
        }
        
        d0 = d0.AddDays(1);
        //break;
      }


      LoadReportsFromXml(infile, writer);

      writer.Write("</LritMessageLog>");
      writer.Close();
      
      System.Console.Out.WriteLine("--end--");
      foreach (var s in types)
      {
        System.Console.Out.WriteLine("Procesado: " + s);
      }
    }

    private void LoadReportsFromXml(string infile, StreamWriter writer)
    {
      var fi = new FileInfo(infile);
      var s = fi.OpenText();

      var doc = new XmlDocument();
      doc.Load(s);
      s.Close();

      foreach (XmlNode report in doc.SelectNodes("/reports/report"))
      {
        string tmp = string.Empty;
        if (report["enviado"].InnerText == "0")
        {
       
          tmp = string.Format(shipreport_t,
            "false", //positionSent
            parsepoint(report["postext"].InnerText, 0), //Latitude
            parsepoint(report["postext"].InnerText, 1),  //Longitude
            report["TimeStamp"].InnerText,  //TimeStamp1
            "0", //ShipborneEquipmentId
            "4024", //ASPId
            "1", //MessageType
            "30052012010101010199999",  //MessageId
            "", //ReferenceId
            report["IMONum"].InnerText, //IMONum
            report["MMSINum"].InnerText, //MMSINum
            report["TimeStampInASP"].InnerText,  //TimeStamp2
            report["TimeStampOutASP"].InnerText,  //TimeStamp3
            "3005", //DCId
            report["TimeStampInDC"].InnerText,  //TimeStamp4
            "1000-01-01T00:00:00Z", //TimeStamp5
            "2", //ResponseType
            "0003", //DataUserRequestor
            report["Name"].InnerText, //ShipName
            "1005", //DataUserProvider
            report["ddpver"].InnerText //DDPVersionNum
            );
          
        }
        else
        {
          tmp = string.Format(shipreport_t,
            "true", //positionSent
            report["Latitude"].InnerText, //Latitude
            report["Longitude"].InnerText,  //Longitude
            report["TimeStamp1"].InnerText,  //TimeStamp1
            report["ShipborneEquipmentId"].InnerText, //ShipborneEquipmentId
            "4024", //ASPId
            report["MessageType"].InnerText, //MessageType
            report["MessageId"].InnerText,  //MessageId
            report["ReferenceId"].InnerText, //ReferenceId
            report["IMONum"].InnerText, //IMONum
            report["MMSINum"].InnerText, //MMSINum
            report["TimeStamp2"].InnerText,  //TimeStamp2
            report["TimeStamp3"].InnerText,  //TimeStamp3
            report["DCId"].InnerText, //DCId
            report["TimeStamp4"].InnerText,  //TimeStamp4
            report["TimeStamp5"].InnerText, //TimeStamp5
            report["ResponseType"].InnerText, //ResponseType
            report["DataUserRequestor"].InnerText, //DataUserRequestor
            report["ShipName"].InnerText, //ShipName
            report["DataUserProvider"].InnerText, //DataUserProvider
            report["DDPVersionNum"].InnerText //DDPVersionNum
            );
        }

        writer.Write("<LritMessage>\n" + tmp + "\n</LritMessage>\n");

        
      
      }
      
      


    }

    private string parsepoint(string point, int p)
    {
      var parts = point.Split(' ');
      if (p == 0)
        return WGS84LatFormat(double.Parse(parts[1].Substring(1)));

      return WGS84LongFormat(double.Parse(parts[2].Substring(0, parts[2].Length - 1)));
    }

    private string WGS84LongFormat(double val)
    {
      //<xs:pattern value="([0-1][0-7][0-9]\.[0-5][0-9]\.[0-9][0-9]\.[eEwW])|([0][8-9][0-9]\.[0-5][0-9]\.[0-9][0-9]\.[eEwW])|(180\.00\.00\.[eEwW])"/>
      string h = "w";
      if (val >= 0) h = "e";

      val = Math.Abs(val);
      double ival = (int)val;
      double min = (val - ival) * 60;
      double sec = 100 * (min - (int)min);

      return string.Format("{0:000}.{1:00}.{2:00}.{3}", (int)ival, (int)min, (int)sec, h);
    }

    private string WGS84LatFormat(double val)
    {
      //<xs:pattern value="([0-8][0-9]\.[0-5][0-9]\.[0-9][0-9]\.[nNsS])|(90\.00\.00\.[nNsS])"/>
      string h = "s";
      if (val >= 0) h = "n";

      val = Math.Abs(val);
      double ival = (int)val;
      double min = (val - ival) * 60;
      double sec = 100 * (min - (int)min);

      return string.Format("{0:00}.{1:00}.{2:00}.{3}", (int)ival, (int)(min), (int)sec, h);
    }


    private void processFolder(string folder, StreamWriter writer)
    {
      System.Console.Out.WriteLine("Processing: " + folder);
      foreach (var f in Directory.EnumerateFiles(folder))
      {
        var fi = new FileInfo(f);

        var name = fi.Name.ToLower();

        if (name.ToLower().StartsWith("shipPositionReport".ToLower()))
        {
          System.Console.Out.WriteLine(string.Format("processFolder: skiping shipPositionReport: {0}", fi.FullName));
          continue;
        }


        string ns = string.Empty;
        foreach (var kv in dicto)
        {
          if (name.StartsWith(kv.Key.ToLower()))
          {
            ns = kv.Value;
            types.Add(kv.Key);
            break;
          }
        }

        if (String.IsNullOrEmpty(ns))
        {
          System.Console.Out.WriteLine(string.Format("processFolder: skiping {0}", fi.FullName));
          return;
        }

        var s = fi.OpenText();

        var doc = new XmlDocument();
        doc.Load(s);
        s.Close();

        var ename = doc.LastChild.LocalName;
        if (ename.ToLower().EndsWith("type"))
          ename = ename.Substring(0, ename.Length - 4);

        string pretmp = string.Format("<{0}:{1} test=\"0\" schemaVersion=\"1.3\">\n    ", ns, ename);
        string tmp =  doc.LastChild.InnerXml;

        tmp = ex0.Replace(tmp, string.Format("$1{0}:$2$3", ns));
        //tmp = ex0.Replace(tmp, string.Format("<$1{0}:$2$3>",ns));//
        tmp = tmp.Replace("><", ">\n    <");
        tmp += string.Format("\n  </{0}:{1}>", ns, ename);

        //HACKS:
        if (ename == "DDPRequest" && !tmp.Contains("ReferenceId"))
        {
          tmp = tmp.Replace("<ns4:UpdateType>","<ns4:ReferenceId/>\n    <ns4:UpdateType>");
        }

        if (ename == "Receipt" && tmp.Contains("<ns14:ReferenceId>0</ns14:ReferenceId>"))
        {
          tmp = tmp.Replace("<ns14:ReferenceId>0</ns14:ReferenceId>", "<ns14:ReferenceId/>");
        }

        writer.Write("<LritMessage>\n  " + pretmp + tmp + "\n</LritMessage>\n");
      }
    }

    private static void usage()
    {
      System.Console.Out.WriteLine("builder date1 date2 rootfolder infile.xml outfile.xml");
      System.Console.Out.WriteLine("date1 date2 format YYYY-MM-DD");
      System.Console.Out.WriteLine("infile.xml: xml reports from database");
      System.Console.Out.WriteLine(MSSQLQUERY);
    }
  }
}
