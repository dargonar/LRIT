using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de JournalReport
  /// </summary>
  public class JournalReportSentDataAccess : BaseDataAccess
  {
    public JournalReportSentDataAccess() : base() { }
    public JournalReportSentDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Crea un nuevo JournalReport en base de datos
    /// </summary>
    /// <param name="journalReport">JournalReport</param>
    public void Create( JournalReportSent journalReport, int inOut )
    {
        journalReport.MsgInOut = new MsgInOut();
        journalReport.MsgInOut.DDPVersion = journalReport.DDPVersionNum;
        journalReport.MsgInOut.Destination = "";
        journalReport.MsgInOut.InOut = inOut;
        journalReport.MsgInOut.MsgId = journalReport.MessageId;
        journalReport.MsgInOut.MsgType = journalReport.MessageType;
        journalReport.MsgInOut.RefId = "";
        journalReport.MsgInOut.Source = journalReport.Originator;
        journalReport.MsgInOut.TimeStamp = journalReport.TimeStamp;

        context.JournalReportSents.InsertOnSubmit(journalReport);
        context.SubmitChanges();
    }
  }
}
