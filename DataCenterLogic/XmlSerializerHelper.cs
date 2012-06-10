using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace DataCenterLogic
{
  public class XmlSerializerHelper<T>
  {
    public Type _type;

    public XmlSerializerHelper()
    {
      _type = typeof(T);
    }


    public string ToStr(object obj)
    {
      XmlSerializer serializer = new XmlSerializer(_type);
      var stringWriter = new System.IO.StringWriter();
      serializer.Serialize(stringWriter, obj);
      return stringWriter.ToString();
    }

    public T FromStr(string buff)
    {
      T result;
      XmlSerializer deserializer = new XmlSerializer(_type);
      var stringReader = new System.IO.StringReader(buff);
      result = (T)deserializer.Deserialize(stringReader);
      return result;
    }
  }

}
