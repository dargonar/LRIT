using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using OpenFlashChart;
using System.Collections;
using DataCenterDataAccess;
using System.Data.Linq;
using System.Globalization;

namespace LRITUi.Controllers
{
  [Authorize(Roles = "Administrador, Auditor")]
  public class AuditController : MyController
  {
    public ActionResult Day()
    {
      ViewData["periodo"] = "Diario";
      ViewData["period"] = "DayStat";
      return View("Audit");
    }

    public ActionResult Week()
    {
      ViewData["periodo"] = "Semanal";
      ViewData["period"] = "WeekStat";
      return View("Audit");
    }

    public ActionResult Month()
    {
      ViewData["periodo"] = "Mensual";
      ViewData["period"] = "MonthStat";
      return View("Audit");
    }

    public void DayStat(string time)
    {
      OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();

      switch (time)
      {
        case "ShipAsp":
          {
            chart.Title = new Title("Barco / PSA");
            break;
          }
        case "Asp":
          {
            chart.Title = new Title("Entrada PSA / Salida PSA");
            break;
          }
        case "AspDc":
          {
            chart.Title = new Title("Salida PSA / Entrada DC");
            break;
          }
        case "Dc":
          {
            chart.Title = new Title("Entrada DC / Salida DC");
            break;
          }
      }

      chart.Title.Style = "{font-size: 15px; color:#0000ff; font-family: Verdana; text-align: center;}";

      var tda = new TrafficDataAccess();
      var TDayStats = tda.GetDayStat();


      //Mapea los DayStat a cada dia
      var map = new Dictionary<int, DayStat>();
      foreach (var row in TDayStats)
      {
        map.Add((int)row.dia,row);
      }

      int day_of_year = DateTime.Today.DayOfYear;

      var lineas = new OpenFlashChart.LineDot[3];

      for (int i = 0; i < lineas.Length; i++)
      {
        lineas[i] = new OpenFlashChart.LineDot();
      }

      lineas[0].Colour = "#ff0000";
      lineas[1].Colour = "#00ff00";
      lineas[2].Colour = "#0000ff";

      for (int i = 0; i < 7; i++ )
      {
        int day = day_of_year - 6 + i;
        if (map.Keys.Contains(day))
        {
          var row = map[day];

          switch (time)
          {
            case "ShipAsp":
              {
                lineas[0].Add(row.MaxShipAsp / 60);
                lineas[1].Add(row.MinShipAsp / 60);
                lineas[2].Add(row.AvgShipAsp / 60);
                break;
              }
            case "Asp":
              {
                lineas[0].Add(row.MaxAsp / 60);
                lineas[1].Add(row.MinAsp / 60);
                lineas[2].Add(row.AvgAsp / 60);
                break;
              }
            case "AspDc":
              {
                lineas[0].Add(row.MaxAspDc / 60);
                lineas[1].Add(row.MinAspDc / 60);
                lineas[2].Add(row.AvgAspDc / 60);
                break;
              }
            case "Dc":
              {
                lineas[0].Add(row.MaxDc / 60);
                lineas[1].Add(row.MinDc / 60);
                lineas[2].Add(row.AvgDc / 60);
                break;
              }
          }
        }
        else
        {
          for (int j = 0; j < 2; j++)
            lineas[j].Add(new LineDotValue(0));
        }
      }

      foreach (var linea in lineas)
      {
        chart.AddElement(linea);
      }

      
      
      XAxis xaxis = new XAxis();
      xaxis.Labels.SetLabels(new string[] { /*(DateTime.Today.AddDays(-7).DayOfWeek).ToString(),*/ (DateTime.Today.AddDays(-6).DayOfWeek).ToString(), 
                                            (DateTime.Today.AddDays(-5).DayOfWeek).ToString(), (DateTime.Today.AddDays(-4).DayOfWeek).ToString(),
                                            (DateTime.Today.AddDays(-3).DayOfWeek).ToString(), (DateTime.Today.AddDays(-2).DayOfWeek).ToString(),
                                            (DateTime.Today.AddDays(-1).DayOfWeek).ToString(), (DateTime.Today.AddDays(-0).DayOfWeek).ToString()
                                              });
      chart.X_Axis = xaxis;

      chart.Y_Axis.Max = 60;
      chart.Y_Axis.Steps = 10;

      string chartString = chart.ToPrettyString();

      Response.Clear();
      Response.CacheControl = "no-cache";
      Response.Write(chartString);
      Response.End();
    }

    public void MonthStat(string time)
    {
      OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();

      switch (time)
      {
        case "ShipAsp":
          {
            chart.Title = new Title("Barco / PSA");
            break;
          }
        case "Asp":
          {
            chart.Title = new Title("Entrada PSA / Salida PSA");
            break;
          }
        case "AspDc":
          {
            chart.Title = new Title("Salida PSA / Entrada DC");
            break;
          }
        case "Dc":
          {
            chart.Title = new Title("Entrada DC / Salida DC");
            break;
          }
      }

      chart.Title.Style = "{font-size: 15px; color:#0000ff; font-family: Verdana; text-align: center;}";

      var tda = new TrafficDataAccess();
      var row = tda.GetMonthStat();

      var lineas = new OpenFlashChart.LineDot[3];

      for (int i = 0; i < lineas.Length; i++)
      {
        lineas[i] = new OpenFlashChart.LineDot();
      }

      lineas[0].Colour = "#ff0000";
      lineas[1].Colour = "#00ff00";
      lineas[2].Colour = "#0000ff";

      for (int i = 0; i < row.Count; i++)
      {
          switch (time)
          {
            case "ShipAsp":
              {
                lineas[0].Add(row[i].MaxShipAsp / 60);
                lineas[1].Add(row[i].MinShipAsp / 60);
                lineas[2].Add(row[i].AvgShipAsp / 60);
                break;
              }
            case "Asp":
              {
                lineas[0].Add(row[i].MaxAsp / 60);
                lineas[1].Add(row[i].MinAsp / 60);
                lineas[2].Add(row[i].AvgAsp / 60);
                break;
              }
            case "AspDc":
              {
                lineas[0].Add(row[i].MaxAspDc / 60);
                lineas[1].Add(row[i].MinAspDc / 60);
                lineas[2].Add(row[i].AvgAspDc / 60);
                break;
              }
            case "Dc":
              {
                lineas[0].Add(row[i].MaxDc / 60);
                lineas[1].Add(row[i].MinDc / 60);
                lineas[2].Add(row[i].AvgDc / 60);
                break;
              }
          }
      }

      foreach (var linea in lineas)
      {
        chart.AddElement(linea);
      }

      var dtfi = new DateTimeFormatInfo();
      
      List<String> labels = new List<String>();
      for (int i = row.Count; i > 0; i--)
			{
        labels.Add(dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-i+1).Month).ToString() )));
      }

      XAxis xaxis = new XAxis();
      xaxis.Labels.SetLabels(labels);         /*(new string[] {   
			                                        dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-5).Month ).ToString() )), 
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-4).Month ).ToString() )),
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-3).Month ).ToString() )), 
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-2).Month ).ToString() )),
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-1).Month ).ToString() )), 
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-0).Month ).ToString() ))
                                              });*/

      chart.X_Axis = xaxis;

      chart.Y_Axis.Max = 60;
      chart.Y_Axis.Steps = 10;

      string chartString = chart.ToPrettyString();

      Response.Clear();
      Response.CacheControl = "no-cache";
      Response.Write(chartString);
      Response.End();
    }


    public void WeekStat(string time)
    {
      OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();

      switch (time)
      {
        case "ShipAsp":
          {
            chart.Title = new Title("Barco / PSA");
            break;
          }
        case "Asp":
          {
            chart.Title = new Title("Entrada PSA / Salida PSA");
            break;
          }
        case "AspDc":
          {
            chart.Title = new Title("Salida PSA / Entrada DC");
            break;
          }
        case "Dc":
          {
            chart.Title = new Title("Entrada DC / Salida DC");
            break;
          }
      }

      chart.Title.Style = "{font-size: 15px; color:#0000ff; font-family: Verdana; text-align: center;}";

      int woy;

      var tda = new TrafficDataAccess();
      var row = tda.GetWeekStat(out woy);

      

      var lineas = new OpenFlashChart.LineDot[3];

      for (int i = 0; i < lineas.Length; i++)
      {
        lineas[i] = new OpenFlashChart.LineDot();
      }

      lineas[0].Colour = "#ff0000";
      lineas[1].Colour = "#00ff00";
      lineas[2].Colour = "#0000ff";

      for (int i = 0; i < row.Count; i++)
      {
        switch (time)
        {
          case "ShipAsp":
            {
              lineas[0].Add(row[i].MaxShipAsp / 60);
              lineas[1].Add(row[i].MinShipAsp / 60);
              lineas[2].Add(row[i].AvgShipAsp / 60);
              break;
            }
          case "Asp":
            {
              lineas[0].Add(row[i].MaxAsp / 60);
              lineas[1].Add(row[i].MinAsp / 60);
              lineas[2].Add(row[i].AvgAsp / 60);
              break;
            }
          case "AspDc":
            {
              lineas[0].Add(row[i].MaxAspDc / 60);
              lineas[1].Add(row[i].MinAspDc / 60);
              lineas[2].Add(row[i].AvgAspDc / 60);
              break;
            }
          case "Dc":
            {
              lineas[0].Add(row[i].MaxDc / 60);
              lineas[1].Add(row[i].MinDc / 60);
              lineas[2].Add(row[i].AvgDc / 60);
              break;
            }
        }
      }

      foreach (var linea in lineas)
      {
        chart.AddElement(linea);
      }

      var dtfi = new DateTimeFormatInfo();

      List<String> labels = new List<String>();
      for (int i = row.Count; i > 0; i--)
      {
        labels.Add((woy -i + 1).ToString());
      }

      XAxis xaxis = new XAxis();
      xaxis.Labels.SetLabels(labels);         /*(new string[] {   
			                                        dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-5).Month ).ToString() )), 
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-4).Month ).ToString() )),
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-3).Month ).ToString() )), 
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-2).Month ).ToString() )),
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-1).Month ).ToString() )), 
                                              dtfi.GetMonthName( int.Parse( (DateTime.Today.AddMonths(-0).Month ).ToString() ))
                                              });*/

      chart.X_Axis = xaxis;

      chart.Y_Axis.Max = 60;
      chart.Y_Axis.Steps = 10;

      string chartString = chart.ToPrettyString();

      Response.Clear();
      Response.CacheControl = "no-cache";
      Response.Write(chartString);
      Response.End();
    }
  }
}
