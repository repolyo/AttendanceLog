using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Collections.Specialized;

/// <summary>
//< add key = "TDomain" value = "lrdc.lexmark.ds" />
//< add key = "TWorkHours" value = "9.6" />
//< add key = "TMinLoginTime" value = "6.0" />
//< add key = "TMaxLoginTime" value = "10.0" />
//< add key = "TDateFormat" value = "yyyy/MM/dd" />
//< add key = "TTimeFormat" value = "HH:mm" />
/// </summary>
public static class AppConfig
{
    public static string ASC = "asc";
    public static string DESC = "desc";

    /// <summary>
    /// Global variable that is constant.
    /// </summary>
    private static string _appName = "Employee Attendance Generator";
    private static string _domain = "lrdc.lexmark.ds";
    private static double _workHours = 9.6; // working time
    private static double _minLogin = 6.0; // 6am
    private static double _maxLogin = 10.0; // 10am
    private static string _dateFormat = "yyyy/MM/dd";
    private static string _timeFormat = "HH:mm";
    private static string _valueFormat = "{0:0.00}";
    private static string _nullValue = "-";
    private static double _version = 1.0;

    // startup or shutdown
    private static string _eventQuery = "*[System/EventID=13 or System/EventID=12]";
    private static int _displayMonths = 1;
    private static string _sortOder = ASC;
    private static bool init = false;

    public static void initialize()
    {
        if (!init) {
            NameValueCollection settings = WebConfigurationManager.AppSettings;
            _appName = settings["TAppName"]; 
            _version = double.Parse(settings["TVersion"]);
            _dateFormat = settings["TDateFormat"];
            _timeFormat = settings["TTimeFormat"];
            _valueFormat = settings["TValueFormat"];
            _nullValue = settings["TNullValue"];
            _eventQuery = settings["TEventQuery"];
            _domain = settings["TDomain"];
            _displayMonths = int.Parse(settings["TDisplayMonths"]);
            _workHours = double.Parse(settings["TWorkHours"]);
            _minLogin = double.Parse(settings["TMinLoginTime"]);
            _maxLogin = double.Parse(settings["TMaxLoginTime"]);
            _sortOder = settings["TSortOrder"];
        }
    }
    public static string AppName { get { return _appName; } }
    public static bool isAscending() { return _sortOder == ASC; }
    public static double Version { get { return _version; } }
    public static string Domain { get { return _domain; } }

    public static double TotalWorkHours
    {
        get { return _workHours; }
    }

    public static double MinLoginTime
    {
        get { return _minLogin; }
    }

    public static double MaxLoginTime
    {
        get { return _maxLogin; }
    }

    public static string DateFormat
    {
        get { return _dateFormat; }
    }

    public static string TimeFormat
    {
        get { return _timeFormat; }
    }

    public static string ValueFormat
    {
        get { return _valueFormat; }
    }

    public static string NullValue
    {
        get { return _nullValue; }
    }

    public static string EventQuery
    {
        get { return _eventQuery; }
    }

    public static int DisplayMonths
    {
        get { return _displayMonths; }
    }
}