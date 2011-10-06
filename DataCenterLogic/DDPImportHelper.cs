using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using DataCenterDataAccess;
using System.Transactions;
using Microsoft.SqlServer.Types;
using log4net;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace DataCenterLogic
{
  /// <summary>
  /// Clase de ayuda para importar el DDP
  /// </summary>
  public class DDPImportHelper
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(DDPImportHelper));

    /// <summary>
    /// Contructor, genera una lista vacia de Places, StandingOrders, Exclusions y SARServices
    /// </summary>
    class ContractingConfig
    {
      public List<Place> places = new List<Place>();
      public List<StandingOrder> standingOrders = new List<StandingOrder>();
      public List<Exclusion> exclusions = new List<Exclusion>();
      public List<SARService> sarservice = new List<SARService>();
    }

    private XmlDocument MixDocs(XmlNode unode, XmlDocument olddoc, XmlNamespaceManager nsmanager)
    {
      foreach (XmlNode node in unode.ChildNodes)
      {
        string xpath = node.Attributes["xPath"].Value.Replace("/", "/lr:");
        XmlNode thenode = node.FirstChild;

        XmlNode targetNode = olddoc.SelectSingleNode(xpath, nsmanager);
        if (targetNode == null)
        {
          log.Warn(string.Format("MixDocs: unable to find {0}, skiping node", xpath));
          continue;
        }

        if (node.Name == "Delete")
          targetNode.ParentNode.RemoveChild(targetNode);

        if (node.Name == "Update")
          targetNode.ParentNode.ReplaceChild(olddoc.ImportNode(thenode, true), targetNode);

        if (node.Name == "Insert")
          targetNode.AppendChild(olddoc.ImportNode(thenode, true));
      }

      return olddoc;
    }

    /// <summary>
    /// Importa un archivo DDP en formato XML a la base de datos
    /// </summary>
    /// <param name="fileName">Nombre del archivo en disco</param>
    public void Import( string fileName, DDPVersion ddpVersion )
    {
      if (File.Exists(fileName) == false)
        throw new FileNotFoundException(string.Format("File {0} not exists", fileName));
        
      Import( File.Open( fileName, FileMode.Open ) , ddpVersion );
    }

    public DateTime Import(Stream stream, DDPVersion ddpversion)
    {
        XmlDocument ddpxml = new XmlDocument();
        ddpxml.Load(stream);

        return Import(ddpxml, ddpversion);
    }


    /// <summary>
    /// Importa un archivo DDP en formato XML a la base de datos
    /// </summary>
    /// <param name="stream">Stream de memoria del archivo</param>
    public DateTime Import(XmlDocument ddpxml, DDPVersion ddpVersion)
    {
      DateTime pubDate = DateTime.UtcNow;
      DBDataContext context = null;
      try
      {
        context = new DBDataContext(Config.ConnectionString);

        //---------------------------------------------------
        XmlNamespaceManager nsmanager = new XmlNamespaceManager(ddpxml.NameTable);
        nsmanager.AddNamespace("lr", "http://gisis.imo.org/XML/LRIT/ddp/2008");
        //---------------------------------------------------

        //Hack-o-mati: Asociation of contracting goverment
        Dictionary<ContractingGoverment, ContractingConfig> goverments = new Dictionary<ContractingGoverment, ContractingConfig>();
        
        //Just for return, do not use here.
        pubDate = DateTime.Parse(ddpxml.SelectSingleNode("/lr:DataDistributionPlan", nsmanager).Attributes["regularVersionImplementationAt"].Value);

        readCGS("/lr:DataDistributionPlan/lr:ContractingGovernment", ref goverments, ref ddpxml, ref nsmanager);
        readCGS("/lr:DataDistributionPlan/lr:Territory", ref goverments, ref ddpxml, ref nsmanager);

        var cgda = new ContractingGovermentDataAccess(context);
        var pda = new PlaceDataAccess(context);
        var soda = new StandingOrderDataAccess(context);
        var ssda = new SARServiceDataAccess(context);
        //var exda = new ExclusionDataAccess(context);

        //cgda.DropAll();

        foreach (KeyValuePair<ContractingGoverment, ContractingConfig> kv in goverments)
        {
          kv.Key.DDPVersionId = ddpVersion.Id;
          int gId = cgda.Create(kv.Key);

          foreach (Place p in kv.Value.places)
            p.ContractingGovermentId = gId;

          foreach (SARService ss in kv.Value.sarservice)
            ss.ContractingGovermentId = gId;

          //foreach (Exclusion excl in kv.Value.exclusions)
          //  excl.ContractingGoverment = gId;

          log.Debug(string.Format("{2}: Key:{0}, Value{1}", kv.Key.Name, kv.Value.places.Count, kv.Key.Id) );

          pda.Create(kv.Value.places.ToArray());
          //exda.Create(kv.Value.exclusions.ToArray());

          ssda.Create(kv.Value.sarservice.ToArray());

          //Places with ids
          List<Place> places = pda.GetAll(ddpVersion.regularVer + ":" + ddpVersion.inmediateVer);

          //Standing orders
          string path = string.Format("/lr:DataDistributionPlan/lr:CoastalStateStandingOrders/lr:StandingOrder[@contractingGovernmentID='{0}']", kv.Key.LRITId);
          XmlNode standingOrder = ddpxml.SelectSingleNode(path, nsmanager);

          if (standingOrder != null && !String.IsNullOrEmpty(standingOrder.InnerText))
          {
            foreach (string area in standingOrder.InnerText.Split(' '))
            {
              int id = getPlaceId(places, area);
              if (id == -1)
                continue;

              StandingOrder so = new StandingOrder();
              so.PlaceId = id;
              kv.Value.standingOrders.Add(so);
            }
          }

          //ES ACA
          soda.Create(kv.Value.standingOrders.ToArray());

        }

      }
      catch (Exception ex)
      {
        if (context != null)
          context.Dispose();

        throw new Exception("Unable to Import DDP File", ex);
      }
      finally
      {
        if(context != null )
          context.Dispose();
      }

      return pubDate;
    }

    private void readCGS(string mpath, ref Dictionary<ContractingGoverment, ContractingConfig> goverments, ref XmlDocument ddpxml, ref XmlNamespaceManager nsmanager)
    {
      foreach (XmlNode goverment in ddpxml.SelectNodes(mpath, nsmanager))
      {
        ContractingGoverment cg = ReadContractingGoverment(goverment);
        goverments[cg] = new ContractingConfig();

        var cc = goverments[cg];
        var dd = goverment;

        //Nested in CG
        ExtractInfo("", ref cc, ref dd, ref nsmanager, false);

        //Nested in Territory
        ExtractInfo("/lr:DataDistributionPlan/lr:Territory[@contractingGovernmentID='" + cg.LRITId.ToString() + "']/", ref cc, ref dd, ref nsmanager, true);

        //Exclusions
        foreach (XmlNode exclusion in ddpxml.SelectNodes("/lr:DataDistributionPlan/lr:Exclusions/lr:Exclusion[@contractingGovernmentID='" + cg.LRITId.ToString() + "']", nsmanager))
        {
          var ex = new Exclusion();
          ex.exclusionID = exclusion.Attributes["exclusionID"].Value;
          ex.ExcludedContractingGovernmentID = exclusion["ExcludedContractingGovernmentID"].InnerText;

          if (exclusion["From"] != null)
            ex.FromDate = DateTime.Parse(exclusion["From"].InnerText).ToUniversalTime();
          else
            ex.FromDate = new DateTime(2000, 1, 1);

          if (exclusion["Until"] != null)
            ex.ToDate = DateTime.Parse(exclusion["Until"].InnerText).ToUniversalTime();
          else
            ex.ToDate = new DateTime(2050, 1, 1);

          //goverments[cg].exclusions.Add(ex);
          cg.Exclusions.Add(ex);
        }
      }
    }
    private void ExtractInfo(string pre, ref ContractingConfig cg, ref XmlNode node, ref XmlNamespaceManager nsmanager, bool IsTerritory)
    {
      cg.places.AddRange(
          ReadPorts(node.SelectNodes(pre + "lr:Ports/lr:Port", nsmanager), IsTerritory));

      //Port Facilities
      cg.places.AddRange(
          ReadPortFacilities(node.SelectNodes(pre + "lr:PortFacilities/lr:PortFacility", nsmanager),IsTerritory));

      //Internal waters
      cg.places.AddRange(ReadPolygons("internalwaters",
                      node.SelectNodes(pre + "lr:InternalWaters/lr:Polygon", nsmanager), IsTerritory));

      //Territorial Sea
      cg.places.AddRange(ReadPolygons("territorialsea",
                      node.SelectNodes(pre + "lr:TerritorialSea/lr:Polygon", nsmanager), IsTerritory));

      //SeawardAreaOf1000NM
      cg.places.AddRange(ReadPolygons("seawardareaof1000nm",
                      node.SelectNodes(pre + "lr:SeawardAreaOf1000NM/lr:Polygon", nsmanager), IsTerritory));

      //CustomCoastalAreas
      cg.places.AddRange(ReadPolygons("customcoastalareas",
                      node.SelectNodes(pre + "lr:CustomCoastalAreas/lr:Polygon", nsmanager), IsTerritory));

      cg.sarservice.AddRange(ReadSARServices(node.SelectNodes(pre + "lr:SARServices/lr:SARService", nsmanager)));
    }

    private SARService[] ReadSARServices(XmlNodeList xmlNodeList)
    {
      List<SARService> sarServices = new List<SARService>();

      foreach (XmlNode xmlsar in xmlNodeList)
      {
        SARService sarserv = new SARService();
        sarserv.LRITid = xmlsar.Attributes["lritID"].Value;
        sarserv.Name = xmlsar["Name"].InnerText;

        sarServices.Add(sarserv);
      }

      return sarServices.ToArray();
    }

    /// <summary>
    /// Obtiene el ID de base de datos de un determinado Place
    /// </summary>
    /// <param name="list">Lista de places</param>
    /// <param name="area">Area a buscar</param>
    /// <returns>-1 si no se encontro o el ID de base de datos si existe.</returns>
    private int getPlaceId(List<Place> list, string area)
    {
      int id = -1;
      foreach(Place place in list)
      {
        if( place.PlaceStringId == area )
        {
          id = place.Id;
          break;
        }
      }
      
      return id;
    }

    /// <summary>
    /// Lee informacion de poligonos de un archivo XML en formato DDP
    /// </summary>
    /// <param name="type">Tipo de area a leer</param>
    /// <param name="xmlNodeList">Nodos XML con informacion de poligonos</param>
    /// <returns>Array de Places</returns>
    private Place[] ReadPolygons(string type, XmlNodeList xmlNodeList, bool IsTerritory)
    {
      List<Place> internalWaters = new List<Place>();

      foreach (XmlNode xmlpolygon in xmlNodeList)
      {
        Place port = new Place();
        
        port.IsTerritory = 0;
        if (IsTerritory) port.IsTerritory = 1;

        port.PlaceStringId = xmlpolygon.Attributes["areaID"].Value;
        port.Name          = "Wata";
        
        byte[] binGeom  = BuildGeomAsBinary(xmlpolygon["lrit:PosList"].InnerText);
        if (binGeom != null)
          port.Area = binGeom;
        else System.Diagnostics.Debug.WriteLine("Error al cargar place. PlaceStringID: "+ port.PlaceStringId);
                
        port.AreaType          = type;
        
        internalWaters.Add(port);
      }

      return internalWaters.ToArray();      
    }

    /// <summary>
    /// Construye un dato del tipo Geography a partir de un string con las coordenadas del poligono, verificando que el sentido de
    /// construcción sea el correcto
    /// </summary>
    /// <param name="rawdata">datos del polígono</param>
    /// <returns>el poligo en formato WKB (well known binary)</returns>
    private byte[] BuildGeomAsBinary(string rawdata)
    {

        try
        {
            var strGeom = new System.Data.SqlTypes.SqlChars(("POLYGON((" + ToPol(rawdata, false) + "))").ToCharArray());
            var binGeom = SqlGeography.STGeomFromText(strGeom, 4326).STAsBinary();
            return binGeom.Buffer;
        }
        catch
        {
            try
            {
                var strGeom = new System.Data.SqlTypes.SqlChars(("POLYGON((" + ToPol(rawdata, true) + "))").ToCharArray());
                var binGeom = SqlGeography.STGeomFromText(strGeom, 4326).STAsBinary();
                return binGeom.Buffer;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Ni al derecho, ni al reves");
            }
        }
        return null;
    }

    

    /// <summary>
    /// Arma un string de formato POLYGON('xx yy ...') para ser insertado en base de datos
    /// Puede recorrer el string de derecha a izquierda o de izquierda a derecha.
    /// </summary>
    /// <param name="s">Vertices del poligono separados por espacios</param>
    /// <param name="reverse">Indica si la informacion del poligono esta clockwise o counter-clockwise</param>
    /// <returns>string en formato POLYGON(xxxx)</returns>
    private string ToPol(string s, bool reversed)
    {
      string[] atom = s.Split(' ');
      string[] PuntosDelPoligono = new string[atom.Length / 2];
      string formateado = string.Empty;
      int ptr = -1;
      for (int i = 0; i < atom.Length; i += 2)
      {
        ptr++;
        if (i == atom.Length - 2)
          PuntosDelPoligono[ptr] += string.Format("{0} {1}", atom[0], atom[1]);
        else
          PuntosDelPoligono[ptr] = string.Format("{0} {1}", atom[i], atom[i + 1]);
      }
       
      if (reversed == true)
      {
        Array.Reverse(PuntosDelPoligono);
      }
    
      for (int i = 0; i < PuntosDelPoligono.Length; i++)
      {
        if (i != PuntosDelPoligono.Length - 1)
          formateado += PuntosDelPoligono[i] + ", ";
        else
          formateado += PuntosDelPoligono[i];
      }
      return formateado;
    }
    
    /// <summary>
    /// Lee informacion de puertos de un XML Nodelist del DDP
    /// </summary>
    /// <param name="xmlports">Nodos XML con informacion de puertos</param>
    /// <returns>Arreglo de Places</returns>
    private Place[] ReadPorts(XmlNodeList xmlports, bool IsTerritory)
    {
      List<Place> ports = new List<Place>();

      foreach (XmlNode xmlport in xmlports)
      {
        Place port = new Place();
        port.IsTerritory = 0;
        if (IsTerritory) port.IsTerritory = 1;

        byte[] buffer = null;

        try
        {
         buffer = SqlGeography.STGeomFromText(new System.Data.SqlTypes.SqlChars(
                    "POINT(" + xmlport["Position"].InnerText + ")"), 4326).STAsBinary().Buffer;
        }
        catch
        {
        }

        port.PlaceStringId    = xmlport.Attributes["locode"].Value;
        port.Name             = xmlport["Name"].InnerText;
        port.Area             = buffer;
        port.AreaType         = "port";
        ports.Add(port);
      }

      return ports.ToArray();
    }

    /// <summary>
    /// Lee informacion de port facilities de un XML Nodelist del DDP
    /// </summary>
    /// <param name="xmlportFacilities">Nodos XML con informacion de port facilities</param>
    /// <returns>Arreglo de Places</returns>
    private Place[] ReadPortFacilities(XmlNodeList xmlportFacilities, bool IsTerritory)
    {
      List<Place> portFacilities = new List<Place>();

      foreach (XmlNode xmlportFacility in xmlportFacilities)
      {
        Place portFacility = new Place();
        portFacility.IsTerritory = 0;
        if (IsTerritory) portFacility.IsTerritory = 1;

        byte[] buffer = null;

        try
        {
          buffer = SqlGeography.STGeomFromText(new System.Data.SqlTypes.SqlChars(
                     "POINT(" + xmlportFacility["Position"].InnerText + ")"), 4326).STAsBinary().Buffer;
        }
        catch
        {
        }

        portFacility.PlaceStringId          = xmlportFacility.Attributes["imoPortFacilityNumber"].Value;
        portFacility.Name                   = xmlportFacility["Name"].InnerText;
        portFacility.Area                   = buffer;
        portFacility.AreaType               = "portfacility";
        portFacilities.Add(portFacility);
      }

      return portFacilities.ToArray();
    }

    /// <summary>
    /// Lee informacion de contracting goverments de un XML Node del DDP
    /// </summary>
    /// <param name="goverment">XML Node del goverment</param>
    /// <returns>Instancia de Contracting goverment para base de datos</returns>
    private ContractingGoverment ReadContractingGoverment(XmlNode goverment)
    {
      ContractingGoverment cg = new ContractingGoverment();
      cg.DataCenterId = int.Parse(goverment["DataCentreInfo"]["DataCentreID"].InnerText);
      cg.isoCode      = goverment["Name"].Attributes["isoCode"].Value;
      cg.LRITId       = int.Parse(goverment.Attributes["lritID"].Value);
      cg.Name         = goverment["Name"].InnerText;
      return cg;
    }

    public void SavePendingUpdates(Stream stream, int ddpUpdateId)
    {
      XmlDocument newdoc = new XmlDocument();
      newdoc.Load(stream);

      //---------------------------------------------------
      XmlNamespaceManager nsmanager = new XmlNamespaceManager(newdoc.NameTable);
      nsmanager.AddNamespace("lr", "http://gisis.imo.org/XML/LRIT/ddp/2008");
      //---------------------------------------------------

      XmlNode root = newdoc.SelectSingleNode("/lr:DataDistributionPlan-IncrementalUpdate", nsmanager);

      var pendingUpdates = new List<PendingDDPUpdate>();

      //Recorrer inmediata si hubiese
      foreach (XmlNode inmediate in newdoc.SelectNodes("/lr:DataDistributionPlan-IncrementalUpdate/lr:Immediate", nsmanager))
      {
        var update = new PendingDDPUpdate();
        update.baseVersion = inmediate.Attributes["baseImmediateVersionNum"].Value;
        update.ddpUpdateId = ddpUpdateId;
        update.implementationTime = DateTime.Parse(inmediate.Attributes["targetImplementationAt"].Value);
        update.targetVersion = inmediate.Attributes["targetImmediateVersionNum"].Value;
        update.type = 0;

        pendingUpdates.Add(update);
      }

      //Recorrer regulares si hubiese
      foreach (XmlNode inmediate in newdoc.SelectNodes("/lr:DataDistributionPlan-IncrementalUpdate/lr:Regular", nsmanager))
      {
        var update = new PendingDDPUpdate();
        update.baseVersion = inmediate.Attributes["baseRegularVersionNum"].Value;
        update.ddpUpdateId = ddpUpdateId;
        update.implementationTime = DateTime.Parse(inmediate.Attributes["targetImplementationAt"].Value);
        update.targetVersion = inmediate.Attributes["targetRegularVersionNum"].Value;
        update.type = 1;

        pendingUpdates.Add(update);
      }

      PendingUpdateManager.Insert(pendingUpdates);
    }

    public void UpdateIncrementalOrRegular(PendingDDPUpdate pending)
    {
      XmlDocument newdoc = new XmlDocument();
      ICSharpCode.SharpZipLib.Zip.ZipFile zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(new MemoryStream(pending.DDPUpdate.DDPFile.ToArray()));
      Stream stream = zipFile.GetInputStream(0);
      newdoc.Load(stream);

      //---------------------------------------------------
      XmlNamespaceManager nsmanager = new XmlNamespaceManager(newdoc.NameTable);
      nsmanager.AddNamespace("lr", "http://gisis.imo.org/XML/LRIT/ddp/2008");
      //---------------------------------------------------

      XmlNode root = newdoc.SelectSingleNode("/lr:DataDistributionPlan-IncrementalUpdate", nsmanager);

      //Get ddp to modify
      var verman = new DDPVersionManager();
      
      //Just get what is ready from the XML
      string qstr = string.Format("/lr:DataDistributionPlan-IncrementalUpdate/lr:{0}[@base{0}VersionNum='{1}' and @target{0}VersionNum='{2}']",
        pending.type==0?"Immediate":"Regular", pending.baseVersion, pending.targetVersion);
      
      //Ontener el nodo "inmediate" o "regular"
      XmlNode inmediate = newdoc.SelectSingleNode(qstr, nsmanager);

      string baseVer = inmediate.Attributes[pending.type == 0 ? "baseImmediateVersionNum" : "baseRegularVersionNum"].Value;
      string targetVer = inmediate.Attributes[pending.type == 0 ? "targetImmediateVersionNum" : "targetRegularVersionNum"].Value;
      DateTime targetImplementationAt = DateTime.Parse(inmediate.Attributes["targetImplementationAt"].Value);

      var ddpver = pending.type == 0 ? verman.GetInmediateDDPVersion(baseVer) : verman.GetRegularDDPVersion(baseVer);
      if (ddpver == null)
      {
        log.Error(string.Format("UpdateIncrementalOrRegular: Unable to get old DDP file with version {1} type:{0}--> aborting", baseVer, pending.type));
        return;
      }

      byte[] rawbuffer = ddpver.DDPFile.ToArray();
      MemoryStream xms = new MemoryStream(rawbuffer);
      XmlDocument olddoc = new XmlDocument();
      if (rawbuffer[0] == 0x50 && rawbuffer[1] == 0x4b)
      {
        ICSharpCode.SharpZipLib.Zip.ZipFile zipFile0 = new ICSharpCode.SharpZipLib.Zip.ZipFile(xms);
        Stream s = zipFile0.GetInputStream(0);
        olddoc.Load(s);
      }
      else
      {
        olddoc.Load(xms);
      }

      //Generar nueva version del xml mezclando los dos docuemtnos;
      var theNewXml = MixDocs(inmediate, olddoc, nsmanager);

      string newver = ddpver.regularVer + ":" + targetVer;
      if (pending.type != 0) newver = targetVer + ":" + ddpver.inmediateVer;

      var ms = new MemoryStream(300000);
      theNewXml.Save(ms);
      var raw = ms.ToArray();

      File.WriteAllBytes(@"c:\"+newver.Split(':')[0]+"x"+newver.Split(':')[1]+".textos", raw);

      Import(theNewXml, DDPManager.InsertCompleteDDP(newver, targetImplementationAt, raw));
      log.Info("UpdateIncrementalOrRegular: New version ready => " + newver);
      
      stream.Close();
    }
  }

}
