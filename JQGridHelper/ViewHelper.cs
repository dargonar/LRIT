using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace JQGridHelper
{
  public static class ViewHelper
  {

    public static string ComboFilter(this HtmlHelper htmlhelper, string name)
    {
      return ComboFilter(htmlhelper, name, false);
    }

    public static string ComboFilterEN(this HtmlHelper htmlhelper, string name)
    {
      return ComboFilter(htmlhelper, name, false, true);
    }

    public static string ComboFilter(this HtmlHelper htmlhelper, string name, bool debug)
    {
      return ComboFilter(htmlhelper, name, debug, false);
    }

    public static string ComboFilter(this HtmlHelper htmlhelper, string name, bool debug, bool en)
    {
      var sb = new StringBuilder();

      var datos = (Array)htmlhelper.ViewData[name];
      
      //var t = htmlhelper.ViewData[name].GetType();
      //t.GetProperty("id").GetValue(htmlhelper.ViewData[name]);
      sb.AppendLine(string.Format("function {0}(cellvalue, options, rowObject) {{", name));

      if (debug)
        sb.AppendLine("\tconsole.log(cellvalue);"); 

      sb.AppendLine("\tswitch(cellvalue) {");

      Type t = null;
      var dplist = string.Format(":{0};", en ? "ALL" : "TODOS");
      foreach (var cg in datos)
      {

        if (t == null)
          t = cg.GetType();

        var id=t.GetProperty("id").GetValue(cg, null);
        var value = t.GetProperty("value").GetValue(cg, null);
        sb.AppendLine(string.Format("\t\tcase {0} : return \"{1}\";", id , value ));
        dplist += id + ":" + value + ";";
      }
      
      sb.AppendLine("}");
      sb.AppendLine(string.Format("return \"{0}\";", en ? "Unknown" : "Desconocido"));
      sb.AppendLine("}");

      sb.AppendLine(string.Format("\n var {0}_edit = '{1}';",name, dplist.Substring(0, dplist.Length-1)));
      return sb.ToString();
    }


    public static string RestoreJQState(this HtmlHelper htmlhelper, string name)
    {
      return RestoreJQState(htmlhelper, name, "id");
    }

    public static string RestoreJQState(this HtmlHelper htmlhelper, string name, string def_col)
    {
      var sb = new StringBuilder();


      var dict = htmlhelper.ViewContext.HttpContext.Session["grid_state_" + name];

      //TODO: default configurables
      sb.AppendLine(string.Format("page: {0},",         get(dict,"page",     "1")    ));
      sb.AppendLine(string.Format("rowNum: {0},",       get(dict,"rows",     "25")   ));
      sb.AppendLine(string.Format("sortname: '{0}',",   get(dict, "sidx",   def_col) ));
      sb.AppendLine(string.Format("sortorder: '{0}',",  get(dict,"sord",     "asc")  ));
      sb.AppendLine(string.Format("search: {0},",       get(dict,"postdata", "{}") == "{}" ? "false" : "true"));
      sb.AppendLine(string.Format("postData: {0},",     get(dict,"postdata", "{}")    ));
      
      return sb.ToString();
    }

    public static string RestoreJQStateScript(this HtmlHelper htmlhelper, string name)
    {
      return RestoreJQStateScript(htmlhelper, name, false);
    }

    public static string RestoreJQStateScript(this HtmlHelper htmlhelper, string name, bool debug)
    {
      var dict = htmlhelper.ViewContext.HttpContext.Session["grid_state_" + name];

      var sb = new StringBuilder();

      sb.AppendLine(string.Format("var temp_{0} = {1};", name, get(dict, "postdata", "{}")));
      
      if(debug)
        sb.AppendLine("\tconsole.log('Entro!');");

      sb.AppendLine(string.Format("for(k in temp_{0}) {{", name));
      
      if (debug)
        sb.AppendLine(string.Format("\tconsole.log(k + '=>' + temp_{0}[k]);", name));

      sb.AppendLine(string.Format("\t$('#gs_'+k).val(temp_{0}[k]);", name));
      sb.AppendLine("}");
      return sb.ToString();
    }

    private static string get(object dict, string key, string def)
    {
      if( dict == null )
        return def;

      var odict = (Dictionary<string,string>)dict;
      if( !odict.Keys.Contains(key) )
        return def;

      return odict[key];
    }


  }
}
