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
      if (args.Length != 4)
      {
        usage();
        return;
      }

      var b = new Builder();
      b.run(args[0], args[1], args[2], args[3]);
    }

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

    private void run(string date0, string date1, string folder, string outfile)
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

      writer.Write("</LritMessageLog>");
      writer.Close();
      
      System.Console.Out.WriteLine("--end--");
      foreach (var s in types)
      {
        System.Console.Out.WriteLine("Procesado: " + s);
      }
    }

    private void processFolder(string folder, StreamWriter writer)
    {
      System.Console.Out.WriteLine("Processing: " + folder);
      foreach (var f in Directory.EnumerateFiles(folder))
      {
        var fi = new FileInfo(f);

        var name = fi.Name.ToLower();

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

        if (ename == "ShipPositionReport")
        {
          writer.Write("<LritMessage>\n<PositionReport positionSent=\"true\">\n  " + pretmp + tmp + "\n</PositionReport>\n</LritMessage>\n");
          
        }
        else
        {
          writer.Write("<LritMessage>\n  " + pretmp + tmp + "\n</LritMessage>\n");
        }

        //break;
      }
    }

    private static void usage()
    {
      System.Console.Out.WriteLine("builder date1 date2 rootfolder outfile.xml");
      System.Console.Out.WriteLine("date1 date2 format YYYY-MM-DD");
    }
  }
}
