using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

/// <summary>
/// Summary description for MessageBox
/// </summary>
/// 
public static class MessageBox
{
    public static void Show(this Page page, String Message) {
        page.ClientScript.RegisterStartupScript(
           page.GetType(),
           "MessageBox",
           "<script language='javascript'>alert('" + Message + "');</script>"
        );
    }
}

