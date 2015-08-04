using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Class1
/// </summary>
public class TimeEntry : IComparable
{
    public TimeEntry()
    {
        //
        // TODO: Add constructor logic here
        //
        _ot = 0;
        _out = DateTime.MinValue;
        _in = DateTime.MaxValue;
        _to = DateTime.MinValue;
    }

    public bool NoTimeIn() { return _in == DateTime.MaxValue;  }
    public bool NoTimeOut() { return _out == DateTime.MinValue; }

    public bool IsNotValid()
    {
        return (_in == DateTime.MaxValue || _out == DateTime.MinValue) ? true : false;
    }

    public int CompareTo(object obj)
    {
        int ret = -1;
        TimeEntry other = obj as TimeEntry;
        if (other == null || IsNotValid() || other.IsNotValid()) {
            return ret;
        }

        if (NoTimeIn()) {
            ret = this.TimeOut.CompareTo(other.NoTimeOut() ? other.TimeIn : other.TimeOut);
        }
        else if (NoTimeOut()) {
            ret = this.TimeIn.CompareTo(other.NoTimeIn() ? other.TimeOut : other.TimeIn);
        }
        return ret;
    }

    public string DateStr
    {
        get { return (_in == DateTime.MaxValue) ? AppConfig.NullValue : _in.ToString(AppConfig.DateFormat); }
    }

    public DateTime TimeIn
    {
        get { return _in; }
        set { if (DateTime.MaxValue == _in || _in > value) _in = value; }
    }

    public DateTime TimeOut
    {
        get { return _out; }
        set { _out = value; }
    }

    // actual time out
    public string TimeOutStr
    {
        get { return (_out == DateTime.MinValue) ? AppConfig.NullValue : _out.ToString(AppConfig.TimeFormat); }
    }

    // actual time in
    public string TimeInStr
    {
        get { return (_in == DateTime.MaxValue) ? AppConfig.NullValue : _in.ToString(AppConfig.TimeFormat); }
    }

    // estimated time-out
    public DateTime OutTime
    {
        get {
            if (_to == DateTime.MinValue && _in != DateTime.MaxValue) {
                //double login_hour = _in.TimeOfDay.TotalHours;
                //double logout_hour = login_hour + working_hours;
                //if (login_hour < minimum_login_hour) {
                //    logout_hour = minimum_login_hour + working_hours;
                //}
                //else if (login_hour > maximum_login_hour) {
                //    logout_hour = maximum_login_hour + working_hours;
                //}
                //else {
                //    logout_hour = login_hour + working_hours;
                //}
                //TimeSpan span = TimeSpan.FromHours(logout_hour);
                _to = _in.AddHours(AppConfig.TotalWorkHours);
            }
            return _to;
        }
    }

    // estimated out time (9.6 hours) based.
    public string OutTimeStr
    {
        get { return (OverTime > AppConfig.MinOverTime) ? OutTime.ToString(AppConfig.TimeFormat) : AppConfig.NullValue; }
    }

    public string OverTimeStr
    {
        get {
            string overtime = AppConfig.NullValue;
            if (OverTime > AppConfig.MinOverTime) {
                overtime = OutTime.ToString(AppConfig.TimeFormat);
                overtime += String.Format(" / " + AppConfig.ValueFormat, OverTime);
            }
            return overtime;
        }
    }

    public double OverTime
    {
        get {
            if (0 == _ot && TimeOut > OutTime) {
                _ot = (TimeOut - OutTime).TotalHours;
            }
            return _ot;
        }
        set { _ot = value; }
    }

    public bool IsSameDay(DateTime date)
    {
        bool same = false;
        if (DateTime.Today.Year == TimeIn.Year
            && DateTime.Today.Month == TimeIn.Month
            && DateTime.Today.Day == TimeIn.Day)
        {
            same = true;
        }
        return same;
    }

    private DateTime _in; // actual timein
    private DateTime _to; // estimated logotu
    private DateTime _out; // actual logout
    private double   _ot; // overtime
}