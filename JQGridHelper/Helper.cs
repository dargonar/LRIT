using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Linq.Dynamic;
using System.Data.Objects;

namespace JQGrid
{
  public static class Helper
  {
    public static object Paginate<T>(IQueryable<T> table, NameValueCollection req, string[] columns, int page, int rows, string sidx, string sord)
    {
      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = table.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      var where = buildWhere<T>(req, columns);

      string cols =   (string)where[0];
      object[] vals = (object[])where[1];

      var str = "new(" + String.Join(",", columns) + ")";
      
      var query = table.OrderBy(sidx + " " + sord)
                    .Where(cols, vals)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);

      var items = query.ToArray();

      //HACKO
      //System.Diagnostics.Debug.WriteLine("V1=>" + ((System.Data.Objects.ObjectQuery<T>)query).ToTraceString());
      //HACKO

      //var items = new string[]{};

      var i = 0;

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
          from item in items
          select new
          {
            id = "row"+ i++.ToString(),
            cell = from col in columns
                   select myFormat(typeof(T).GetProperty(col).GetValue(item, null))
          }).ToArray()
      };
      
      return jsonData;
    }

    public static object Paginate2<T>(ObjectContext context, int totalRecords, HttpRequestBase req, string[] columns, int page, int rows, string sidx, string sord)
    {
      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      int offset = pageIndex * pageSize;


      var tmp = buildWhere2<T>(req.Params, columns);

      string where = (string)tmp[0];
      ObjectParameter[] vals = (ObjectParameter[])tmp[1];

      //string sql_stmt = String.Format(@"SELECT VALUE b FROM (SELECT VALUE a FROM {0} AS a WHERE {1} ORDER BY a.{2} {3} ) as b ORDER BY b.{2} skip {4} limit {5}",
      string sql_stmt = String.Format(@"SELECT VALUE a FROM {0} AS a WHERE {1} ORDER BY a.{2} {3}",
         typeof(T).Name, where, sidx, sord, offset, rows);

      var queryx = context.CreateQuery<T>(sql_stmt, vals);

      
      var query = queryx.OrderBy("")
                  .Take(10)
                  .Skip(10);

      //System.Diagnostics.Debug.WriteLine((ObjectQuery<T>)query.ToTraceString());


      var items = query.ToArray();

      //var items = query.Execute(MergeOption.NoTracking).ToArray();
        
      var i = 0;

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
          from item in items
          select new
          {
            id = "row" + i++.ToString(),
            cell = from col in columns
                   select myFormat(typeof(T).GetProperty(col).GetValue(item, null))
          }).ToArray()
      };

      return jsonData;
    }

    private static object myFormat(object o)
    {
      if ( o != null && (o.GetType() == typeof(DateTime) || o.GetType() == typeof(Nullable<DateTime>)))
      {
        return string.Format("{0:yyyy-MM-dd}", o);
      }
      
      return o;
    }

    public static object[] buildWhere<T>(NameValueCollection req, string[] columns)
    {
      var values = new List<object>();
      var predicate = new StringBuilder();

      foreach(string key in req.AllKeys)
      {
        if (!columns.Contains(key))
          continue;

        if (values.Count != 0) predicate.Append(" and ");
        
        //string
        if (typeof(T).GetProperty(key).PropertyType == typeof(string))
        {
          predicate.Append(string.Format("{0}.Contains(@{1})", key, values.Count));
          values.Add(Convert.ChangeType(req[key], typeof(T).GetProperty(key).PropertyType));
        }
        //datetime
        else if (typeof(T).GetProperty(key).PropertyType == typeof(Nullable<DateTime>)
              || typeof(T).GetProperty(key).PropertyType == typeof(DateTime))
        {
          DateTime[] dates = getDatesFromString(req[key]);
          if (dates.Length == 0)
          {
            //remove "and"
            predicate.Remove(predicate.Length - 5, 5);
            continue;
          }

          if (dates.Length == 1)
          {
            predicate.Append(string.Format("{0} == @{1}", key, values.Count));
            values.Add(dates[0]);
          }

          if (dates.Length == 2)
          {
            predicate.Append(string.Format("{0} >= @{1} and {0} <= @{2} ", key, values.Count, values.Count + 1));
            values.Add(dates[0]);
            values.Add(dates[1]);
          }
        }
        //cualquier otro
        else
        {
          predicate.Append(string.Format("{0} == @{1}", key, values.Count));
          
          var t = typeof(T).GetProperty(key).PropertyType;
          if( t.IsGenericType )
            t = Nullable.GetUnderlyingType(typeof(T).GetProperty(key).PropertyType);
          
          values.Add(Convert.ChangeType(req[key], t));
        }
      }

      //Hack-o-matic
      if (values.Count == 0)
        predicate.Append("1 == 1");

      return new object[] { predicate.ToString(), values.ToArray() };
    }

    public static object[] buildWhere2<T>(NameValueCollection req, string[] columns)
    {

      var values = new List<ObjectParameter>();
      var predicate = new StringBuilder();

      foreach (string key in req.AllKeys)
      {
        if (!columns.Contains(key))
          continue;

        if (values.Count != 0) predicate.Append(" and ");

        //string
        if (typeof(T).GetProperty(key).PropertyType == typeof(string))
        {
          //marca 1
          predicate.Append(string.Format("a.{0} like @p{1}", key, values.Count));
          values.Add( new ObjectParameter(string.Format("p{0}",values.Count), Convert.ChangeType(req[key], typeof(T).GetProperty(key).PropertyType)));
        }
        //datetime
        else if (typeof(T).GetProperty(key).PropertyType == typeof(Nullable<DateTime>)
              || typeof(T).GetProperty(key).PropertyType == typeof(DateTime))
        {
          DateTime[] dates = getDatesFromString(req[key]);
          if (dates.Length == 0)
          {
            //remove "and"
            predicate.Remove(predicate.Length - 5, 5);
            continue;
          }

          //TODO: hace GETDATE(datetime) para que compare solo fecha y no fecha+hora
          if (dates.Length == 1)
          {
            //marca 2
            predicate.Append(string.Format("a.{0} == @p{1}", key, values.Count));
            values.Add( new ObjectParameter(string.Format("p{0}",values.Count),dates[0]));
          }

          if (dates.Length == 2)
          {
            //marca 3
            var tmp = values.Count;
            predicate.Append(string.Format("a.{0} >= @p{1} and a.{0} <= @p{2} ", key, tmp, tmp+1));
            values.Add( new ObjectParameter(string.Format("p{0}",tmp),dates[0]));
            values.Add( new ObjectParameter(string.Format("p{0}",tmp),dates[1]));
          }
        }
        //cualquier otro
        else
        {
          //marca 4
          predicate.Append(string.Format("a.{0} == @p{1}", key, values.Count));

          var t = typeof(T).GetProperty(key).PropertyType;
          if (t.IsGenericType)
            t = Nullable.GetUnderlyingType(typeof(T).GetProperty(key).PropertyType);

          values.Add( new ObjectParameter(string.Format("p{0}",values.Count), Convert.ChangeType(req[key], t)));
        }
      }

      //Hack-o-matic
      if (values.Count == 0)
        predicate.Append("1 == 1");

      return new object[] { predicate.ToString(), values.ToArray() };
    }

    private static DateTime[] getDatesFromString(string str)
    {
      var dates = new List<DateTime>();
      foreach(var s in str.Split(';'))
      {
        DateTime d;
        if (DateTime.TryParse(s, out d) == true)
          dates.Add(d);
      }

      return dates.ToArray();
    }

    public static void SaveJQState(string grid_name, string[] columns, Dictionary<string,string> gridvars, HttpRequestBase req, HttpSessionStateBase session)
    {
      var postdata = new StringBuilder();

      postdata.Append("{");
      foreach (var col in columns)
      {
        if (col.ToLower() == "id" || !req.Params.AllKeys.Contains(col))
          continue;

        if (postdata.Length > 1)
          postdata.Append(",");

        postdata.Append(string.Format("{0}:'{1}'", col, req.Params[col]));
      }
      postdata.Append("}");
      
      var dict = new Dictionary<string, string>() { { "postdata", postdata.ToString() } };
      foreach( var kv in gridvars )
      {
        dict[kv.Key] = gridvars[kv.Key];
      }
      
      session["grid_state_" + grid_name] = dict;
    }
  }
}
