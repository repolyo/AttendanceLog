using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CsvTimeReader
/// </summary>
public class CsvTimeReader : List<TimeEntry>
{
    const short START_LINE = 3;
    const short DATE_COL = 0;
    const short TIME_COL = 3;
    private double _totalOvertime;

    public CsvTimeReader() { }
    public double TotalOvertime { get { return _totalOvertime; } }
    public List<TimeEntry> LoadTimes(string fileName)
    {
        string[] Lines = File.ReadAllLines(fileName);
        string[] Fields;
        Fields = Lines[START_LINE].Split(new char[] { ',' });
        int Cols = Fields.GetLength(0);
        //1st row must be column names; force lower case to ensure matching later on.
        //for (int i = 0; i < Cols - 1; i++)
        //    dt.Columns.Add(Fields[i].ToLower(), typeof(string));
        //DataRow Row;

        for (int i = START_LINE + 1; i < Lines.GetLength(0) - 1; i++) {
            Fields = Lines[i].Split(new char[] { ',' });
            //Debug.WriteLine("start = " + start);
            //Debug.WriteLine("times = " + times);
            TimeEntry t = TimeEntry.parse(Fields[DATE_COL], Fields[TIME_COL]);
            if (t.TotalWorkHours() < 1) continue;
            Add(t);
            _totalOvertime += t.OverTime;
        }

        File.Delete(fileName);
        return this;
    }
}