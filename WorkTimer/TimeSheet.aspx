<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TimeSheet.aspx.cs" Inherits="_Default" %>
<%@ Register Assembly="SlimeeLibrary" Namespace="SlimeeLibrary" TagPrefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Employee Attendance Generator v1.0</title>
    <style type="text/css">

    /*CSS to main Container TABLE or DIV or any PlaceHolder TAG of the web page*/
    #container{
        width:500px;
        margin-left:auto;
        margin-right:auto;
        text-align:left;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="PopupBox" runat="server"></asp:Literal>
    <asp:LoginName ID="LoginName1" Runat="server"  FormatString ="Welcome, {0}"/>
    <br />
    <asp:LoginStatus ID="LoginStatus1" runat="server" />
    <div>
    <table style="width:50%;">
        <tr>
            <td >
                <asp:Label ID="StartDateLabel" runat="server" AssociatedControlID="StartDate">Start Date:</asp:Label>
            </td>
            <td>
                <cc1:DatePicker ID="StartDate" runat="server" OnSelectedDateChanged="DatePicker1_DateChanged"
                    AutoPostBack="true" Width="100px" PaneWidth="150px" EnableViewState="true">
                    <PaneTableStyle BorderColor="#707070" BorderWidth="1px" BorderStyle="Solid" />
                    <PaneHeaderStyle BackColor="#0099FF" />
                    <TitleStyle ForeColor="White" Font-Bold="true" />
                    <NextPrevMonthStyle ForeColor="White" Font-Bold="true" />
                    <NextPrevYearStyle ForeColor="#E0E0E0" Font-Bold="true" />
                    <DayHeaderStyle BackColor="#E8E8E8" />
                    <TodayStyle BackColor="#FFFFCC" ForeColor="#000000" Font-Underline="false" BorderColor="#FFCC99"/>
                    <AlternateMonthStyle BackColor="#F0F0F0" ForeColor="#707070" Font-Underline="false"/>
                    <MonthStyle BackColor="" ForeColor="#000000" Font-Underline="false"/>
                </cc1:DatePicker>
            </td>
        </tr>
        <tr>
            <asp:GridView ID='GridView1' runat='server' AutoGenerateColumns='false'
                    EmptyDataText ="There are no data here yet!"
                    HeaderStyle-BackColor="PapayaWhip"
                    AlternatingRowStyle-BackColor="LightCyan">
                  <Columns>
                    <asp:TemplateField 
                        HeaderStyle-Width="30"
                        ItemStyle-HorizontalAlign="Center" >
                        <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText='Date' DataField='DateStr' 
                        HeaderStyle-Width="200" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText='Time In' DataField='TimeInStr' 
                        HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText='Time Out' DataField='TimeOutStr' 
                        HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText='OT Start' DataField='OutTimeStr' 
                        HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText='OT (hours)' DataField='OverTimeStr' 
                        HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                 </Columns>
            </asp:GridView>
        </tr>
    </table>
    </div>
    <div>
        <asp:Button ID="Import" runat="server" Text="Import" OnClick="Import_Click" />
        <br />
        <asp:Label ID="Attendance1" runat="server" Text=" - " Font-Size="Small"></asp:Label>
    </div>
    </form>
</body>
</html>
