using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acesso a datos de DDPNotification
  /// </summary>
  public class PendingUpdatesDataAccess : BaseDataAccess
  {
    public PendingUpdatesDataAccess() : base() { }
    public PendingUpdatesDataAccess(DBDataContext context) : base(context) { }
    
    public void Create( List<PendingDDPUpdate> updates )
    {
      context.PendingDDPUpdates.InsertAllOnSubmit(updates);
      context.SubmitChanges();
    }

    public List<PendingDDPUpdate> WithTimeStampLess(DateTime ts)
    {
      DataLoadOptions options = new DataLoadOptions();
      options.LoadWith<PendingDDPUpdate>(p => p.DDPUpdate);
      context.LoadOptions = options;
      return context.PendingDDPUpdates
              .Where(p => p.implementationTime <= ts)
              .OrderBy( p => p.type )
              .OrderBy( p => p.implementationTime )
              .ToList();
    }

    public void Remove(List<PendingDDPUpdate> pendings)
    {
      List<int> ids = new List<int>();
      foreach(PendingDDPUpdate pd in pendings) ids.Add(pd.id);
      IEnumerable<PendingDDPUpdate> upds = context.PendingDDPUpdates.Where(p => ids.Contains(p.id));
      context.PendingDDPUpdates.DeleteAllOnSubmit(upds);
      context.SubmitChanges();
    }
  }
}
