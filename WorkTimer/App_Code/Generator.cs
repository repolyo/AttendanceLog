using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security;

/// <summary>
/// Summary description for TestClass
/// </summary>
public class Generator : List<TimeEntry>
{
    enum WinEvent
    {
        Startup = 12,
        Shutdown = 13,
        Logon = 7001,
        Logoff = 7002
    };
    private double _totalOvertime;

    public Generator(EventLogReader logReader)
    {
        _totalOvertime = 0.0;
    }

    public double TotalOvertime { get { return _totalOvertime; } }

    public List<TimeEntry> QueryRemoteComputer(string queryString, string machine, string username, string passwd)
    {
        // startup or shutdown
        if (string.IsNullOrEmpty(queryString)) { // defaults
            queryString = "*[System/EventID=13 or System/EventID=12]";
        }
        SecureString pw = new SecureString();
        if (!string.IsNullOrEmpty(passwd)) {
            char[] charArray = passwd.ToCharArray();
            for (int i = 0; i < passwd.Length; i++) { pw.AppendChar(charArray[i]); }
        }
        pw.MakeReadOnly();
        EventLogSession session = new EventLogSession(
            machine,                               // Remote Computer
            "lrdc.lexmark.ds",                                  // Domain
            username,                                // Username
            pw,
            SessionAuthentication.Default);
        pw.Dispose();

        // Query the Application log on the remote computer.
        EventLogQuery query = new EventLogQuery("System", PathType.LogName, queryString);
        query.Session = session;
        
        EventLogReader logReader = new EventLogReader(query);

        // Display event info
        DisplayEventAndLogInformation(logReader);
        return this;
    }

    /// <summary>
    /// Displays the event information and log information on the console for 
    /// all the events returned from a query.
    /// </summary>
    public List<TimeEntry> DisplayEventAndLogInformation(EventLogReader logReader)
    {
        TimeEntry time = null;
        DateTime now = DateTime.Now;
        Dictionary<string, TimeEntry> timetable = new Dictionary<string, TimeEntry>();
        
        for (EventRecord eventdetail = logReader.ReadEvent();
                     eventdetail != null;
                     eventdetail = logReader.ReadEvent())
        {
            EventLogRecord logRecord = (EventLogRecord)eventdetail;
            DateTime eventTime = eventdetail.TimeCreated.Value;
            WinEvent eventId = (WinEvent)eventdetail.Id;
            string key = eventTime.ToString(AppConfig.DateFormat);

            // candidate for logout after midnight.
            if ((eventId == WinEvent.Shutdown || eventId == WinEvent.Logoff)
                && 0.0 <= eventTime.TimeOfDay.TotalHours
                && eventTime.TimeOfDay.TotalHours <= AppConfig.LogoutTolerance) {
                key = eventTime.AddDays(-1).ToString(AppConfig.DateFormat);
            }
            
            if (!timetable.TryGetValue(key, out time)) {
                time = new TimeEntry();
                timetable.Add(key, time);
            }

            switch (eventId) {
                case WinEvent.Startup:
                case WinEvent.Logon:
                    time.TimeIn = eventTime;
                    break;
                case WinEvent.Logoff:
                case WinEvent.Shutdown:
                    if (time.TimeOut < eventTime) time.TimeOut = eventTime;
                    break;
            }
        }

        
        // note: logic below will only works with sorted in ascending order log entries!
        foreach (KeyValuePair<string, TimeEntry> e in timetable.ToList()) {
            time = e.Value;

            // calculate logout time for today
            if (time.IsSameDay(DateTime.Today)) {
                time.TimeOut = time.OutTime;
                if (time.TimeOut < DateTime.Now) {
                    time.TimeOut = DateTime.Now;
                }
                this.Add(time);
                break;
            }

            // strip invalid entries
            if (time.IsNotValid()) {
                continue;
            }
            this.Add(time);
            _totalOvertime += time.OverTime;
        }

        if (!AppConfig.isAscending()) {
            this.Sort(delegate (TimeEntry firstPair, TimeEntry nextPair) {
                return nextPair.CompareTo(firstPair);
            });
        }
        return this;
    }

}