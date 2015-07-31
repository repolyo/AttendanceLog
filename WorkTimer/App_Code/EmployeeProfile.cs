using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Class1
/// </summary>
public class EmployeeProfile
{
    /// <summary>
    /// Global variable that is constant.
    /// </summary>
    private string _domain = "lrdc.lexmark.ds";

    /// <summary>
    /// Static value protected by access routine.
    /// </summary>
    private string _name;
    /// <summary>
    /// Global static field.
    /// </summary>
    private string _passwd;

    private string _machine;

    /// <summary>
    /// Access routine for global variable.
    /// </summary>
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string Password
    {
        get { return _passwd; }
        set { _passwd = value; }
    }

    public string Machine
    {
        get { return _machine; }
        set { _machine = value; }
    }

    public string Domain
    {
        get { return _domain; }
        set { _domain = value; }
    }
}