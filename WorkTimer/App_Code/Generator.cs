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
        TimeEntry time = null;
        DateTime now = DateTime.Now;
        TimeEntry prev = null;
        Dictionary<string, TimeEntry> timetable = new Dictionary<string, TimeEntry>();
        for (EventRecord eventdetail = logReader.ReadEvent();
                     eventdetail != null;
                     eventdetail = logReader.ReadEvent())
        {
            EventLogRecord logRecord = (EventLogRecord)eventdetail;
            DateTime eventTime = eventdetail.TimeCreated.Value;
            //Console.WriteLine("Description: {0}\n", eventdetail.FormatDescription());
            // Cast the EventRecord object as an EventLogRecord object to 
            // access the EventLogRecord class properties            
            //Console.WriteLine("Container Event Log: {0}\n", logRecord.ContainerLog);
            String key = eventTime.ToString(AppConfig.DateFormat);
            if (!timetable.TryGetValue(key, out time))
            {
                time = new TimeEntry();
                timetable.Add(key, time);
            }

            if (eventdetail.Id == 12) { // login time
                time.TimeIn = eventTime;

                //if (eventTime.Day == 1) {
                //    eventTime = eventTime.AddDays(1);
                //    key = eventTime.ToString(AppConfig.DateFormat);
                //    if (!timetable.ContainsKey(key))
                //    {
                //        time = new TimeEntry(); 
                //        time.TimeOut = eventTime.AddHours(eventTime.Hour * -1 + 1);
                //        timetable.Add(key, time);
                //    }
                //}
            }
            else if (eventdetail.Id == 13) { // logout time
                if (time.TimeOut < eventTime) time.TimeOut = eventTime;
            }
        }

        List<KeyValuePair<string, TimeEntry>> myList = timetable.ToList();
        myList.Sort(
            delegate (KeyValuePair<string, TimeEntry> firstPair,
            KeyValuePair<string, TimeEntry> nextPair) {
                return firstPair.Value.CompareTo(nextPair.Value);
            }
        );

        // note: logic below will only works with sorted in ascending order log entries!
        foreach (KeyValuePair<string, TimeEntry> e in myList) {
            time = e.Value;
            // strip invalid entries
            if (time.NoTimeOut()) {
                prev = time; // entry without matching logout
                continue;
            }
            if (time.NoTimeOut()) {
            }

            if (time.NoTimeIn() // entry without matching login
                && null != prev && !prev.NoTimeIn()) // previous entry with no matching logout
            { 
                // candidate for logout after midnight.
                if (time.TimeOut.TimeOfDay.TotalHours > AppConfig.LogoutTolerance) {
                   prev = null; // outside our tolerable time after midnight logout.
                   continue;
                }
                prev.TimeOut = time.TimeOut;
                time = prev;
            }
            // calculate logout time for today
            if (time.IsSameDay(DateTime.Today) && time.OutTime < DateTime.Now) {
                if (time.TimeOut < DateTime.Now) time.TimeOut = DateTime.Now;
            }
            else { // estimate time-out
                time.TimeOut = time.OutTime;
            }
            
            this.Add(time);
            prev = null;
        }

        if (!AppConfig.isAscending()) {
            myList.Sort( delegate(KeyValuePair<string, TimeEntry> firstPair,
                KeyValuePair<string, TimeEntry> nextPair) {
                    return nextPair.Value.CompareTo(firstPair.Value);
                }
            );
        }
        return this;
    }

}