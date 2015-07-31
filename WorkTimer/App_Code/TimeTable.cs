using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for Class1
/// </summary>
public class TimeTable : DataTable
{
    public TimeTable()
    {
        this.Columns.Add( new DataColumn("ID", typeof(int)) );
        this.Columns.Add("Time In", typeof(string));
        this.Columns.Add("Time Out", typeof(string));
    }

    //This is the AddRow method to add a new row in Table dt 
    public void AddRow(int id, TimeEntry t)
    {
        this.Rows.Add(new object[] { id, t.TimeIn.ToString(), t.TimeOut.ToString() });
    }
}