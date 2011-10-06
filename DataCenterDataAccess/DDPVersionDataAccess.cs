using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DataCenterDataAccess
{
    public class DDPVersionDataAccess : BaseDataAccess
    {
      public DDPVersionDataAccess() : base() { }
      public DDPVersionDataAccess(DBDataContext context) : base(context) { }

        public DDPVersion GetInmediateDDPVersion(string strInmediateVersion)
        {
          DDPVersion ddpVersion = context.DDPVersions
                                    .Where(d => d.inmediateVer == strInmediateVersion)
                                    .OrderByDescending(d => d.published_at)
                                    .OrderByDescending(d => d.received_at)
                                    .FirstOrDefault();
          return ddpVersion;
        }

        public DDPVersion GetRegularDDPVersion(string strRegularVersion)
        {
          DDPVersion ddpVersion = context.DDPVersions
                                    .Where(d => d.regularVer == strRegularVersion)
                                    .OrderByDescending(d => d.published_at)
                                    .OrderByDescending(d => d.received_at)
                                    .FirstOrDefault();
          return ddpVersion;
        }

        public DDPVersion GetDDPVersion(string strDDPVersion)
        {
          string[] parts = strDDPVersion.Split(':');
          if (parts.Length != 2) return null;

          DDPVersion ddpVersion = context.DDPVersions
              .Where(d => d.regularVer == parts[0] && d.inmediateVer == parts[1] )
              .FirstOrDefault();

          return ddpVersion;
        }
        
        public void InsertCompleteDDP(DDPVersion ddpVersion)
        {
          context.DDPVersions.InsertOnSubmit(ddpVersion);
          context.SubmitChanges();
        }

        public DDPVersion DDPForDate(DateTime date)
        {
          return context.DDPVersions.Where(d => d.published_at <= date)
                                    .OrderByDescending(p => p.published_at)
                                    .OrderByDescending(d => d.received_at)
                                    .FirstOrDefault();
        }

        public DDPVersion TodaysDDP()
        {
          return DDPForDate(DateTime.UtcNow);
        }

        public List<DDPVersion> GetVersions()
        {
          return context.DDPVersions.OrderByDescending( ddp => ddp.Id )
                                    .Take(10)
                                    .ToList();
        }
      
        public IQueryable<DDPData> GetAllFromVersion(DDPVersion ddp)
        {
          string ddpver = ddp.regularVer + ":" + ddp.inmediateVer;
          return context.DDPDatas.Where(d => d.DDPVersion == ddpver);
        }

        public IQueryable<DDPData> GetAllFromVersion(int ddpid)
        {
          return context.DDPDatas.Where(d => d.Id == ddpid);
        }

        public DDPVersion GetVersionById(int id)
        {
          return context.DDPVersions.Where(ddpv => ddpv.Id == id).SingleOrDefault();
        }

        public void update(DDPVersion ddpVersion)
        {
          var xx = context.DDPVersions.Where(d => d.Id == ddpVersion.Id).SingleOrDefault();
          xx.published_at = ddpVersion.published_at;
          context.SubmitChanges();
        }
    }
}
