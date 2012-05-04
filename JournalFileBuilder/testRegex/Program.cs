using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace testRegex
{
  class Program
  {
    static void Main(string[] args)
    {

      Regex ex = new Regex(@"(<[/]*)([^\s>]*).*?([/]*>)", RegexOptions.Compiled);
      
      string tmp = "<ReferenceId xmlns=\"http://gisis.imo.org/XML/LRIT/positionReport/2008\" />";

      System.Diagnostics.Debug.WriteLine("-----");
      foreach (Group g in ex.Match(tmp).Groups)
      {
        System.Diagnostics.Debug.WriteLine("-->" + g.Value);
      }
 
      tmp = ex.Replace(tmp, string.Format("$1{0}:$2$3", "ns1"));


      tmp = tmp;

    }
  }
}
