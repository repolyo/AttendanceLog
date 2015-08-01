using System;
using System.Security;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Diagnostics.Eventing.Reader;
using System.Web.Configuration;
using System.Collections.Specialized;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppConfig.initialize();
        EmployeeProfile emp = (EmployeeProfile)Session["EmployeeProfile"];
        if (null == emp) {
            emp = new EmployeeProfile();
            emp.Domain = AppConfig.Domain;
            emp.Machine = GetIPAddress();
            Session["EmployeeProfile"] = emp;
        }
        Login1.TitleText = "Employee Attendance Generator v1.0";
        TextBox TextBox1 = (TextBox)Login1.FindControl("Machine");
        if (null != TextBox1 && string.IsNullOrEmpty(TextBox1.Text)) TextBox1.Text = GetIPAddress();
        if (string.IsNullOrEmpty(Login1.UserName)) Login1.UserName = emp.Name;
        if (!string.IsNullOrEmpty(AppConfig.UserName)
            && !string.IsNullOrEmpty(AppConfig.Password))
        {
            Session["autologin"] = true;
            emp.Name = AppConfig.UserName;
            emp.Password = AppConfig.Password;
            Session["EventLogReader"] = createEventQuery(emp);
            FormsAuthentication.RedirectFromLoginPage(
                String.Format("{0}@{1}", emp.Name, emp.Machine), true);
        }
    }

    protected void ValidateUser(object sender, EventArgs e)
    {
        EmployeeProfile emp = (EmployeeProfile)Session["EmployeeProfile"];
        if (!string.Equals(emp.Name, Login1.UserName)) emp.Name = Login1.UserName;
        if (!string.Equals(emp.Password, Login1.Password)) emp.Password = Login1.Password;
        TextBox TextBox1 = (TextBox)Login1.FindControl("Machine");
        if (null != TextBox1) emp.Machine = TextBox1.Text;

        //Login1.FailureText = "Account has not been activated.";
        if (string.IsNullOrEmpty(emp.Name)) {
            Login1.FailureText = "Username and/or password is incorrect.";
        }
        else try {
            Session["EventLogReader"] = createEventQuery(emp);            
            FormsAuthentication.RedirectFromLoginPage(
                String.Format("{0}@{1}", Login1.UserName, emp.Machine), 
                Login1.RememberMeSet);
        }
        catch (UnauthorizedAccessException err) {
            Login1.FailureText = String.Format("{0} on {1}", 
                err.Message, emp.Machine);
        }
        catch (EventLogException err) {
            Login1.FailureText = String.Format("{0} on {1}", 
                err.Message, emp.Machine);
        }
    }

    private EventLogReader createEventQuery(EmployeeProfile emp)
    {
        DateTime now = DateTime.Now;
        DateTime endDate = now;
        DateTime startDate = DateTime.Today.AddDays(AppConfig.DisplayDays * -1);
        Session["DateTime"] = startDate;

        string queryString = String.Format(AppConfig.EventQuery,
                startDate.ToUniversalTime().ToString("o"),
                endDate.ToUniversalTime().ToString("o"));
        EventLogQuery query = new EventLogQuery("System", PathType.LogName, queryString);
        if (AppConfig.RemoteQuery) {
            // Query the Application log on the remote computer.
            SecureString pw = new SecureString();
            string passwd = emp.Password;
            if (!string.IsNullOrEmpty(passwd)) {
                char[] charArray = passwd.ToCharArray();
                for (int i = 0; i < passwd.Length; i++) { pw.AppendChar(charArray[i]); }
            }
            pw.MakeReadOnly();
            query.Session = new EventLogSession(
                emp.Machine, // Remote Computer
                emp.Domain, // Domain
                emp.Name,   // Username
                pw,
                SessionAuthentication.Default);
            //pw.Dispose();
            Session["SecureString"] = pw;
        }
        Session["EventLogQuery"] = queryString;
        return new EventLogReader(query);
    }

    /// <summary>
    /// Read a password from the console into a SecureString
    /// </summary>
    /// <returns>Password stored in a secure string</returns>
    public static SecureString GetPassword()
    {
        SecureString password = new SecureString();
        Console.WriteLine("Enter password: ");

        // get the first character of the password
        ConsoleKeyInfo nextKey = Console.ReadKey(true);
        while (nextKey.Key != ConsoleKey.Enter)
        {
            if (nextKey.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.RemoveAt(password.Length - 1);

                    // erase the last * as well
                    Console.Write(nextKey.KeyChar);
                    Console.Write(" ");
                    Console.Write(nextKey.KeyChar);
                }
            }
            else
            {
                password.AppendChar(nextKey.KeyChar);
                Console.Write("*");
            }
            nextKey = Console.ReadKey(true);
        }

        Console.WriteLine();

        // lock the password down
        password.MakeReadOnly();
        return password;
    }


    protected string GetIPAddress()
    {
        string ipAddress = "";
        do
        {
            //if (!string.IsNullOrEmpty(Machine1.Text)) break;

            ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            if (!string.IsNullOrEmpty(ipAddress)) break;

            System.Web.HttpContext context = System.Web.HttpContext.Current;
            ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    ipAddress = addresses[0];
                }
            }
            if (!string.IsNullOrEmpty(ipAddress)) break;

            ipAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            if (!string.IsNullOrEmpty(ipAddress)) break;

            ipAddress = "10.194.15.187"; // my machine: debug testing
        } while (false);
        return ipAddress;
    }


}