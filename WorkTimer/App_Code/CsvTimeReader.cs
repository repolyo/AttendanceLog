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
public class CsvTimeReader
{
    const short START_LINE = 3;
    const short DATE_COL = 0;
    const short TIME_COL = 3;

    public CsvTimeReader()
    {

    }

    public List<TimeEntry> LoadTimes(string fileName)
    {
        List<TimeEntry> sheetData = new List<TimeEntry>();

        const int startLine = 3;
        string[] Lines = File.ReadAllLines(fileName);
        string[] Fields;
        Fields = Lines[startLine].Split(new char[] { ',' });
        int Cols = Fields.GetLength(0);
        //1st row must be column names; force lower case to ensure matching later on.
        //for (int i = 0; i < Cols - 1; i++)
        //    dt.Columns.Add(Fields[i].ToLower(), typeof(string));
        //DataRow Row;

        for (int i = startLine + 1; i < Lines.GetLength(0) - 1; i++) {
            Fields = Lines[i].Split(new char[] { ',' });
            //Debug.WriteLine("start = " + start);
            //Debug.WriteLine("times = " + times);
            TimeEntry t = TimeEntry.parse(Fields[DATE_COL], Fields[TIME_COL]);
            if (t.TotalWorkHours() < 1) continue;
            sheetData.Add(t);
        }
        return sheetData;
    }
}