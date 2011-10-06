using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LRITSIGOTestClient
{
  class Program
  {
    static void Main(string[] args)
    {
      var s = new LRITSIGO.LRITSIGOSoapClient();

      var d = s.DatasetLritHistorico(
                              new DateTime(2010, 5, 1, 10, 0, 0),
                              new DateTime(2010, 6, 1, 10, 0, 0),
                              new string[] { "111", "222" }
              );

      int i = 0;

    }
  }
}
