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
    private static double _minOverTime = 1;
    private static double _overTimeInterval = 0.5; // half-hour interval
    private static double _logoutTolerance = 5.0; // 5 hours after midnight
    private static double _minLogin = 6.0; // 6am
    private static double _maxLogin = 10.0; // 10am
    private static string _dateFormat = "yyyy/MM/dd";
    private static string _timeFormat = "HH:mm";
    private static string _valueFormat = "{0:0.00}";
    private static string _overtimeFormat = "HH:mm / {0:0.00}";
    private static string _nullValue = "-";
    private static double _version = 1.0;
    private static string _passwd;
    private static string _username;

    // startup or shutdown
    private static string _eventQuery = "*[System/EventID=13 or System/EventID=12]";
    private static int _displayDays = 7;
    private static string _sortOder = ASC;
    private static bool init = false;
    private static bool _remoteQuery = false;

    public static void initialize()
    {
        if (!init) {
            NameValueCollection settings = WebConfigurationManager.AppSettings;
            _username = settings["TUserName"];
            _passwd = settings["TPassWord"]; 
            _appName = settings["TAppName"]; 
            _version = double.Parse(settings["TVersion"]);
            _dateFormat = settings["TDateFormat"];
            _timeFormat = settings["TTimeFormat"];
            _valueFormat = settings["TValueFormat"];
            _overtimeFormat = settings["TOverTimeFormat"];
            _nullValue = settings["TNullValue"];
            _eventQuery = settings["TEventQuery"];
            _domain = settings["TDomain"];
            _displayDays = int.Parse(settings["TDisplayDays"]);
            _workHours = double.Parse(settings["TWorkHours"]);
            _minOverTime = double.Parse(settings["TMinOvertime"]);
            _overTimeInterval = double.Parse(settings["TOvertimeInterval"]);
            _minLogin = double.Parse(settings["TMinLoginTime"]);
            _maxLogin = double.Parse(settings["TMaxLoginTime"]);
            _logoutTolerance = double.Parse(settings["TLogoutTolerance"]);
            _sortOder = settings["TSortOrder"];
            _remoteQuery = bool.Parse(settings["TRemoteQuery"]);
        }
    }
    public static string AppName { get { return _appName; } }
    public static bool isAscending() { return _sortOder == ASC; }
    public static double Version { get { return _version; } }
    public static string Domain { get { return _domain; } }
    public static bool RemoteQuery { get { return _remoteQuery; } }
    public static double TotalWorkHours { get { return _workHours; } }
    public static double MinOverTime { get { return _minOverTime; } }
    public static double OverTimeInterval { get { return _overTimeInterval; } }
    public static double MinLoginTime { get { return _minLogin; } }
    public static double MaxLoginTime { get { return _maxLogin; } }
    public static double LogoutTolerance { get { return _logoutTolerance; } }
    public static string DateFormat { get { return _dateFormat; } }
    public static string TimeFormat { get { return _timeFormat; } }
    public static string ValueFormat { get { return _valueFormat; } }
    public static string OverTimeFormat { get { return _overtimeFormat; } }
    public static string NullValue { get { return _nullValue; }  }
    public static string EventQuery { get { return _eventQuery; } }
    public static int DisplayDays { get { return _displayDays; } }
    public static string UserName { get { return _username; } }
    public static string Password { get { return _passwd; } }
}