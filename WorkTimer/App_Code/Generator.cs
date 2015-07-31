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
    public Generator(EventLogReader logReader)
    {

    }

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
        DateTime now = DateTime.Now;
        Dictionary<string, TimeEntry> timetable = new Dictionary<string, TimeEntry>();
        for (EventRecord eventdetail = logReader.ReadEvent();
                     eventdetail != null;
                     eventdetail = logReader.ReadEvent())
        {
            TimeEntry log = null;
            EventLogRecord logRecord = (EventLogRecord)eventdetail;
            DateTime eventTime = eventdetail.TimeCreated.Value;
            //Console.WriteLine("Description: {0}\n", eventdetail.FormatDescription());
            // Cast the EventRecord object as an EventLogRecord object to 
            // access the EventLogRecord class properties            
            //Console.WriteLine("Container Event Log: {0}\n", logRecord.ContainerLog);
            String key = eventTime.ToString(AppConfig.DateFormat);
            if (!timetable.TryGetValue(key, out log)) {
                log = new TimeEntry();
                timetable.Add(key, log);
            }

            if (eventdetail.Id == 12) { // login time
                log.TimeIn = eventTime;
                // estimate time-out
                log.TimeOut = log.OutTime;
            }
            else if (eventdetail.Id == 13) { // logout time
                if (log.TimeOut < eventTime) log.TimeOut = eventTime;
            }
        }

        List<KeyValuePair<string, TimeEntry>> myList = timetable.ToList();
        myList.Sort(
            delegate (KeyValuePair<string, TimeEntry> firstPair,
            KeyValuePair<string, TimeEntry> nextPair) {
                return AppConfig.isAscending() ? firstPair.Value.CompareTo(nextPair.Value) :
                    nextPair.Value.CompareTo(firstPair.Value);
            }
        );

        foreach (KeyValuePair<string, TimeEntry> e in myList) {
            TimeEntry time = e.Value;
            // strip invalid entries
            if (time.IsNotValid()) continue;

            // calculate logout time for today
            if (time.IsSameDay(DateTime.Today) && time.OutTime < DateTime.Now) {
                time.TimeOut = DateTime.Now;
            }
            this.Add(time);
        }
        return this;
    }

}