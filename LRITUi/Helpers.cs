using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace LRITUi
{
  public static partial class Helpers
  {
    public static string Label(this HtmlHelper helper, string target, string text)
    {
      return String.Format("<label for='{0}'>{1}</label>", target, text);
    }

    /// <summary>
    /// Genera un list Element con un link dentro
    /// </summary>
    /// <param name="link">Url del link</param>
    /// <param name="text">Texto del Link</param>
    /// <param name="cssclass">clase del list element</param>
    /// <returns></returns>
    public static string ListElement(string link, string text, string cssclass)
    {
      return String.Format(@"<li class='{2}'><a href='{0}'>{1}</a></li>", link, text, cssclass);
    }

    public static string Opened(this ViewContext viewcontext, string str)
    {
      foreach (var s in str.Split(';'))
      {
        if (viewcontext.Controller.GetType().Name.StartsWith(s))
          return "class=\"opened\"";
      }
      return "";
    }

    public static string ErrorBR<T>(this HtmlHelper<T> helper, string label, string error, bool usebr) where T: class
    {
      if ( helper.ViewData.ModelState[label] == null || helper.ViewData.ModelState[label].Errors.Count == 0)
        return "";

      var s = "";
      if (usebr)
        s = "<br/>";
      return s+"<span class=\"field-validation-error\">" + error + "</span>";
    }

    public static string StatusTR(this HtmlHelper helper, string label, int status, TimeSpan since)
    {
      return StatusTR(helper, label, status, since, false);
    }

    public static string StatusTR(this HtmlHelper helper, string label, int status, TimeSpan since, bool border)
    {
      string statusText = status == 0 ? "OK" : "FAIL";
      string cssClass   = status == 0 ? "bgok" : "bgfail";
      
      if(border)
      {
        return String.Format("<tr class=\"bg {0}\"><td style=\"border-left: 1px solid #959595;\" >{1}</td><td>{2}</td><td>Hace {3} Minutos</td></tr>", 
            cssClass, label, statusText, since.TotalMinutes.ToString("f0"));
      }

      return String.Format("<tr class=\"bg {0}\"><td>{1}</td><td>{2}</td><td>Hace {3} Minutos</td></tr>", 
          cssClass, label, statusText, since.TotalMinutes.ToString("f0"));
    }
  }

}