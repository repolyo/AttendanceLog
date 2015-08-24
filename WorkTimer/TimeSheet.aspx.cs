using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Diagnostics.Eventing.Reader;
using SlimeeLibrary;
using System.Security;
using System.IO;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EventLogReader logReader = (EventLogReader)Session["EventLogReader"];
        EmployeeProfile emp = (EmployeeProfile)Session["EmployeeProfile"];
        LoginStatus1.Visible = (null == Session["autologin"]);
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
            TotalOT.Text = "Total Overtime: " + testClass.TotalOvertime;

            this.StartDate.Culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            this.StartDate.SelectedDate = (DateTime)Session["DateTime"];
                //this.StartDate.SelectedDateChanged += new EventHandler(DatePicker1_DateChanged);
        }
        catch (EventLogException err) {
            Console.WriteLine("Could not query the remote computer! " + err.Message);
            FormsAuthentication.RedirectToLoginPage();
        }
    }

    protected void LoginStatus1_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        //Session["EmployeeProfile"] = null;
        //Session["EventLogReader"] = null;
    }

    protected void DatePicker1_DateChanged(object sender, EventArgs e)
    {
        EmployeeProfile emp = (EmployeeProfile)Session["EmployeeProfile"];
        SecureString pw = (SecureString)Session["SecureString"];
        DatePicker dp = (DatePicker)sender;
        DateTime now = DateTime.Now;
        DateTime endDate = now;
        DateTime startDate = dp.SelectedDate;

        string queryString = String.Format(AppConfig.EventQuery,
            startDate.ToUniversalTime().ToString("o"),
            endDate.ToUniversalTime().ToString("o"));
        
        // Query the Application log on the remote computer.
        EventLogQuery query = new EventLogQuery("System", PathType.LogName, queryString);
        if (AppConfig.RemoteQuery) {
            string passwd = emp.Password;
            query.Session = new EventLogSession(
                emp.Machine, // Remote Computer
                emp.Domain, // Domain
                emp.Name,   // Username
                pw,
                SessionAuthentication.Default);
        }
        EventLogReader logReader = new EventLogReader(query);
        Session["EventLogQuery"] = queryString;
        
        Generator testClass = new Generator(logReader);
        List<TimeEntry> table = testClass.DisplayEventAndLogInformation(logReader);
        Attendance1.Text = String.Format("Query: {0}", Session["EventLogQuery"]);
        GridView1.DataSource = table;
        GridView1.DataBind();

        TotalOT.Text = "Total Overtime: " + testClass.TotalOvertime;
    }

    protected void Import_Click(object sender, EventArgs e)
    {
        //MessageBox.Show(this,
        //    "Not yet implemented: Intended for loading tyco exported time data. Coming soon...");
        if (FileUploadControl.HasFile) {
            try {
                string filename = Server.MapPath("~/") + Path.GetFileName(FileUploadControl.FileName);
                FileUploadControl.SaveAs(filename);
                CsvTimeReader reader = new CsvTimeReader();
                GridView1.DataSource = reader.LoadTimes(filename);
                GridView1.DataBind();
                Attendance1.Text = "Upload status: File uploaded!";
                TotalOT.Text = "Total Overtime: " + reader.TotalOvertime;
            }
            catch (Exception ex) {
                Attendance1.Text = 
                    "Upload status: The file could not be uploaded. The following error occured: " 
                    + ex.Message;
            }
        }
    }
}