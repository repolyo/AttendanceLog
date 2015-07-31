using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Diagnostics.Eventing.Reader;
using SlimeeLibrary;
using System.Security;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EventLogReader logReader = (EventLogReader)Session["EventLogReader"];
        EmployeeProfile emp = (EmployeeProfile)Session["EmployeeProfile"];
        if (null == emp 
            || !this.Page.User.Identity.IsAuthenticated
            || string.IsNullOrEmpty(emp.Name)) {
            FormsAuthentication.RedirectToLoginPage();
        }
        else if (logReader != null) try {
            Generator testClass = new Generator(logReader);
            List<TimeEntry> table = testClass.DisplayEventAndLogInformation(logReader);
            Attendance1.Text = String.Format("Query: {0}", Session["EventLogQuery"]);
            GridView1.DataSource = table;
            GridView1.DataBind();
            Session["EventLogReader"] = null;

            this.StartDate.Culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            this.StartDate.SelectedDate = (DateTime)Session["DateTime"];
                //this.StartDate.SelectedDateChanged += new EventHandler(DatePicker1_DateChanged);
        }
        catch (EventLogException err) {
            Console.WriteLine("Could not query the remote computer! " + err.Message);
            FormsAuthentication.RedirectToLoginPage();
        }
    }

    protected void DatePicker1_DateChanged(object sender, EventArgs e)
    {
        EmployeeProfile emp = (EmployeeProfile)Session["EmployeeProfile"];
        SecureString pw = (SecureString)Session["SecureString"];
        DatePicker dp = (DatePicker)sender;
        DateTime now = DateTime.Now;
        DateTime endDate = now;
        DateTime startDate = dp.SelectedDate;

        //MessageBox.Show(this, dp.SelectedDate.ToString());
        
        string passwd = emp.Password;
        string queryString = String.Format(AppConfig.EventQuery,
            startDate.ToUniversalTime().ToString("o"),
            endDate.ToUniversalTime().ToString("o"));
        
        EventLogSession session = new EventLogSession(
            emp.Machine, // Remote Computer
            emp.Domain, // Domain
            emp.Name,   // Username
            pw,
            SessionAuthentication.Default);

        // Query the Application log on the remote computer.
        EventLogQuery query = new EventLogQuery("System", PathType.LogName, queryString);
        query.Session = session;
        EventLogReader logReader = new EventLogReader(query);
        Session["EventLogQuery"] = queryString;

        Generator testClass = new Generator(logReader);
        List<TimeEntry> table = testClass.DisplayEventAndLogInformation(logReader);
        Attendance1.Text = String.Format("Query: {0}", Session["EventLogQuery"]);
        GridView1.DataSource = table;
        GridView1.DataBind();
    }

    protected void Import_Click(object sender, EventArgs e)
    {
        MessageBox.Show(this,
            "Not yet implemented: Intended for loading tyco exported time data. Coming soon...");
    }
}