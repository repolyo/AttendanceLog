<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="_Default" %>

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
    <h2>Employee Attendance Generator v1.0</h2>
    <asp:Login ID="Login1" OnAuthenticate= "ValidateUser"
        
        runat="server"  BackColor="#F7F6F3" BorderColor="#E6E2D8" BorderPadding="4" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" ForeColor="#333333">
        <TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="0.9em" ForeColor="White" />
        <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
        <TextBoxStyle Font-Size="0.8em" />
        <LayoutTemplate>
            <table cellpadding="4" cellspacing="0" style="border-collapse:collapse;">
                <tr>
                    <td>
                        <table cellpadding="0">
                            <tr>
                                <td align="center" colspan="2" style="color:White;background-color:#5D7B9D;font-size:0.9em;font-weight:bold;">Log In</td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="MachineLabel" runat="server" AssociatedControlID="Machine">Workstation:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="Machine" runat="server" Font-Size="0.8em" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="MachineRequired" runat="server"
                                        ControlToValidate="Machine" ErrorMessage="Must be a windows machine." 
                                        ToolTip="Target windows machine." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="UserName" runat="server" Font-Size="0.8em"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="Password" runat="server" Font-Size="0.8em" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:CheckBox ID="RememberMe" runat="server" Text="Remember me next time." />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" style="color:Red;">
                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="2">
                                    <asp:Button ID="LoginButton" runat="server" BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" CommandName="Login" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284775" Text="Log In" ValidationGroup="Login1" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </LayoutTemplate>
        <LoginButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px"
            Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284775" />
    </asp:Login>
</form>
</body>
</html>
