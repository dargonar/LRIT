using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace LRITDataCenter
{
  [AttributeUsage(AttributeTargets.Method,
                  AllowMultiple = false)]
  public class ValidationAttribute : SoapExtensionAttribute
  {
    int priority = 0;

    public override System.Type ExtensionType
    {
      get { return typeof(MessageValidator); }
    }

    public override int Priority
    {
      get { return priority; }
      set { priority = value; }
    }
  }

  public class MessageValidator : SoapExtension
  {
    private bool xsdLoaded = false;
    public override object GetInitializer(
      System.Type serviceType)
    {
      return GetInitializerHelper();
    }
    public override object GetInitializer(
      LogicalMethodInfo methodInfo,
      SoapExtensionAttribute attribute)
    {
      return null;
    }

    public object GetInitializerHelper()
    {
      XmlSchemaSet sc = new XmlSchemaSet();
      HttpContext ctx = HttpContext.Current;
      if (Directory.Exists(ctx.Server.MapPath("xsd")))
      {
        string[] schemaFiles = Directory.GetFiles(
          ctx.Server.MapPath("xsd"), "*.xsd");
        foreach (string schemaFile in schemaFiles)
        {
          if (schemaFile.EndsWith("Types.xsd"))
            continue;

          XmlTextReader r = new XmlTextReader(schemaFile);
          XmlSchema schema = XmlSchema.Read(r, null);
          sc.Add(schema);
        }
      }
      return sc;
    }

    private XmlSchemaSet _context = null;
    public override void Initialize(object initializer)
    {
      if (initializer == null)
      {
        initializer = GetInitializerHelper();
      }
      _context = (XmlSchemaSet)initializer;
    }

    public override void ProcessMessage(SoapMessage message)
    {
      if (SoapMessageStage.BeforeDeserialize == message.Stage)
      {
        try 
        {
          XmlReaderSettings settings = new XmlReaderSettings();
          settings.ValidationType = ValidationType.Schema;
          settings.Schemas = _context;
          XmlReader tr = XmlReader.Create(message.Stream, settings);
          while (tr.Read()) ; // read through stream
        }
        finally
        {
          message.Stream.Position = 0;
        }
      }

    }
  }
}
