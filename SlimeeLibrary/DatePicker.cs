/*
 * ============================================================================
 * Slimee Control Library - Copyright © 2009
 * [DatePicker Control Class]
 *
 * Author: Daniel Oliver
 * Website: http://www.slimee.com
 * 
 * NOTICE:
 * This file is part of Slimee Control Library.
 * 
 * Slimee Control Library is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Slimee Control Library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * HISTORY:
 * 1.0.2 Minor Update [2009 October 13]
 *   - Fixed issue with SelectedDate property on load.
 * 1.0.1 Minor Update [2009 October 12]
 *   - Fixed issue with control rendered IDs when within content place holders. 
 * 1.0.0 First Release
 * 
 * ============================================================================
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Permissions;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;

[assembly: WebResource("SlimeeLibrary.Images.DatePicker.gif","image/gif")]
[assembly: WebResource("SlimeeLibrary.Javascript.DatePicker.js", "text/javascript")]
[assembly: WebResource("SlimeeLibrary.Javascript.DateTime.js", "text/javascript")]

namespace SlimeeLibrary
{
    [ToolboxData("<{0}:DatePicker runat=\"server\"> </{0}:DatePicker>"),
    PersistChildren(true)]
    public class DatePicker: WebControl
    {
        #region Component Initialization

        /*** Declare inner controls. ***/

        // Base control layout table for managing control width and layout.
        private Table containerTable;
        private TableRow containerRow;
        private TableCell textBoxCell;
        private TextBox textBox;
        private TableCell imageButtonCell;
        private ImageButton imageButton;

        // Client floating container for navigation and calendar.
        private Panel floatingPanel;

        // Floating pane layout table. Includes navigation and container for calendar table.
        private Table paneTable;
        private TableRow paneHeadRow;
        private TableCell prevYearCell;
        private HyperLink prevYearLink;
        private TableCell prevMonthCell;
        private HyperLink prevMonthLink;
        private TableCell titleCell;
        private TableCell nextMonthCell;
        private HyperLink nextMonthLink;
        private TableCell nextYearCell;
        private HyperLink nextYearLink;
        private TableRow paneBodyRow;
        private TableCell paneBodyCell;
        
        // Calendar table base. Contents updated by client javascript.
        private Table calendarTable;
        private TableRow calendarTableHeadRow;

        // Hidden values. Storing information to be interpreted by client javascript.
        private HiddenField todayHidden;
        private HiddenField yearHidden;
        private HiddenField monthHidden;
        private HiddenField alternateMonthStyleHidden;
        private HiddenField alternateMonthLinkStyleHidden;
        private HiddenField monthStyleHidden;
        private HiddenField monthLinkStyleHidden;
        private HiddenField todayStyleHidden;
        private HiddenField todayLinkStyleHidden;
        private HiddenField selectedStyleHidden;
        private HiddenField selectedLinkStyleHidden;
        private HiddenField dateFormatHidden; // Used to indicate to client javascript date format.
        private HiddenField firstDayOfWeekHidden;

        // Property values.
        private bool _autoPostBack;
        private Unit _paneWidth;
        private TableStyle _paneTableStyle;
        private TableItemStyle _paneHeaderStyle;
        private TableItemStyle _titleStyle;
        private TableItemStyle _nextPrevMonthStyle;
        private TableItemStyle _nextPrevYearStyle;
        private TableStyle _calenderTableStyle;
        private TableItemStyle _dayHeaderStyle;
        private TableItemStyle _alternateMonthStyle;
        private TableItemStyle _monthStyle;
        private TableItemStyle _todayStyle;
        private TableItemStyle _selectedStyle;

        private void InitializeComponent()
        {
            /*** Register Javascript ***/
            // Register Control Client Javascript (Populates and Controls Client Calendar.) ***/
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), "DatePicker"))
            {
                Page.ClientScript.RegisterClientScriptInclude("DatePicker",
                    Page.ClientScript.GetWebResourceUrl(this.GetType(), "SlimeeLibrary.Javascript.DatePicker.js"));
            }

            // Generate PostBack Script (If AutoPostBack is TRUE)
            string postBackScript = "<script type=\"text/javascript\"> function DatePicker_PostBack(clientID){ ";
            if (this.AutoPostBack == true)
            {
                this.Page.ClientScript.GetPostBackEventReference(this, null);
                postBackScript += "__doPostBack(clientID,'');";
            }
            postBackScript += "}</script>";
            
            // Register PostBack Script
            if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "DatePicker_PostBack"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DatePicker_PostBack", postBackScript);
            }

            // Register DateTime Client Javascript
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), "DatePicker_DateTime"))
            {
                Page.ClientScript.RegisterClientScriptInclude("DatePicker_DateTime",
                    Page.ClientScript.GetWebResourceUrl(this.GetType(), "SlimeeLibrary.Javascript.DateTime.js"));
            }

            /*** Initialize Inner Controls And Set Properties ***/
            // Base control layout table
            textBox = new TextBox();
            textBox.ID = this.ID + "_TextBox";
            textBox.ForeColor = this.ForeColor;
            textBox.TextChanged += new EventHandler(OnSelectedDateChanged);
            textBox.Width = this.Width; // Due to resizing issues in IE7. Using TextBox to determine control dimensions.
            this.Width = Unit.Empty; // Reset base control width.
            textBox.Height = this.Height; // Using textbox to determine control dimensions.
            this.Height = Unit.Empty; // Reset base control height.
            textBox.Style["padding"] = "0px";
            textBox.Style["margin"] = "0px";

            textBoxCell = new TableCell();
            textBoxCell.HorizontalAlign = HorizontalAlign.Left;
            textBoxCell.VerticalAlign = VerticalAlign.Middle;
            textBoxCell.Controls.Add(textBox);

            imageButton = new ImageButton();
            imageButton.ID = this.ID + "_ImageButton";
            imageButton.ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "SlimeeLibrary.Images.DatePicker.gif");// "Images/cal.gif"; //Default address
            imageButton.AlternateText = "Pick";
            imageButton.Style["margin-left"] = "5px";
            imageButton.OnClientClick = "return DatePicker_ShowCalendar('" + this.ClientID + "');";

            imageButtonCell = new TableCell();
            imageButtonCell.HorizontalAlign = HorizontalAlign.Center;
            imageButtonCell.VerticalAlign = VerticalAlign.Middle;
            imageButtonCell.Controls.Add(imageButton);

            containerRow = new TableRow();
            containerRow.Cells.Add(textBoxCell);
            containerRow.Cells.Add(imageButtonCell);

            containerTable = new Table();
            containerTable.CellPadding = 0;
            containerTable.CellSpacing = 0;
            containerTable.Rows.Add(containerRow);

            /*** Generate Client Floating Panel (Calendar table, navigation table, & panel) ***/
            // Calendar Header Row
            calendarTableHeadRow = new TableRow();
            calendarTableHeadRow.TableSection = TableRowSection.TableHeader;

            // Calendar Table
            calendarTable = new Table();
            calendarTable.ID = this.ID + "_CalendarTable";
            calendarTable.CellPadding = 0;
            calendarTable.CellSpacing = 0;
            calendarTable.Width = new Unit("100%");
            calendarTable.HorizontalAlign = HorizontalAlign.Center;
            calendarTable.Font.Name = "arial";
            calendarTable.Font.Size = new FontUnit("9pt");
            calendarTable.Rows.Add(calendarTableHeadRow);

            // Navigation - Previous Year Link
            prevYearLink = new HyperLink();
            prevYearLink.NavigateUrl = "javascript: DatePicker_PrevYear(\"" + this.ClientID + "\")";
            prevYearLink.TabIndex = -1;
            prevYearLink.Font.Underline = false;
            prevYearLink.Text = "&lt;&lt;";

            prevYearCell = new TableCell();
            prevYearCell.HorizontalAlign = HorizontalAlign.Center;
            prevYearCell.VerticalAlign = VerticalAlign.Middle;
            prevYearCell.Controls.Add(prevYearLink);
            
            // Navigation - Previous Month Link
            prevMonthLink = new HyperLink();
            prevMonthLink.NavigateUrl = "javascript: DatePicker_PrevMonth(\"" + this.ClientID + "\")";
            prevMonthLink.TabIndex = -1;
            prevMonthLink.Font.Underline = false;
            prevMonthLink.Text = "&lt;";

            prevMonthCell = new TableCell();
            prevMonthCell.HorizontalAlign = HorizontalAlign.Center;
            prevMonthCell.VerticalAlign = VerticalAlign.Middle;
            prevMonthCell.Controls.Add(prevMonthLink);

            // Navigation - Month Label/Title
            titleCell = new TableCell();
            titleCell.ID = this.ID + "_MonthCell";
            titleCell.HorizontalAlign = HorizontalAlign.Center;
            titleCell.VerticalAlign = VerticalAlign.Middle;

            // Navigation - Next Month Link
            nextMonthLink = new HyperLink();
            nextMonthLink.NavigateUrl = "javascript: DatePicker_NextMonth(\"" + this.ClientID + "\")";
            nextMonthLink.TabIndex = -1;
            nextMonthLink.Font.Underline = false;
            nextMonthLink.Text = "&gt;";

            nextMonthCell = new TableCell();
            nextMonthCell.HorizontalAlign = HorizontalAlign.Center;
            nextMonthCell.VerticalAlign = VerticalAlign.Middle;
            nextMonthCell.Controls.Add(nextMonthLink);

            // Navigation - Next Year Link
            nextYearLink = new HyperLink();
            nextYearLink.NavigateUrl = "javascript: DatePicker_NextYear(\"" + this.ClientID + "\")";
            nextYearLink.TabIndex = -1;
            nextYearLink.Font.Underline = false;
            nextYearLink.Text = "&gt;&gt;";

            nextYearCell = new TableCell();
            nextYearCell.HorizontalAlign = HorizontalAlign.Center;
            nextYearCell.VerticalAlign = VerticalAlign.Middle;
            nextYearCell.Controls.Add(nextYearLink);

            // Navigation - Header Row (navigation controls)
            paneHeadRow = new TableRow();
            paneHeadRow.TableSection = TableRowSection.TableHeader;
            paneHeadRow.Cells.Add(prevYearCell);
            paneHeadRow.Cells.Add(prevMonthCell);
            paneHeadRow.Cells.Add(titleCell);
            paneHeadRow.Cells.Add(nextMonthCell);
            paneHeadRow.Cells.Add(nextYearCell);

            paneBodyCell = new TableCell();
            paneBodyCell.ColumnSpan = 5;

            // Navigation - Body Row (calendar container)
            paneBodyRow = new TableRow();
            paneBodyRow.TableSection = TableRowSection.TableBody;
            paneBodyRow.Cells.Add(paneBodyCell);
            paneBodyCell.Controls.Add(calendarTable);

            // Navigation - Table
            paneTable = new Table();
            paneTable.CellPadding = 0;
            paneTable.CellSpacing = 0;
            paneTable.Width = new Unit("100%");
            paneTable.HorizontalAlign = HorizontalAlign.Center;
            paneTable.Font.Name = "Arial";
            paneTable.Font.Size = new FontUnit("10pt");
            paneTable.Rows.Add(paneHeadRow);
            paneTable.Rows.Add(paneBodyRow);

            /*** Initialize Hidden Values ***/
            todayHidden = new HiddenField();
            todayHidden.ID = this.ID + "_TodayValue";

            yearHidden = new HiddenField();
            yearHidden.ID = this.ID + "_YearValue";

            monthHidden = new HiddenField();
            monthHidden.ID = this.ID + "_MonthValue";
            
            alternateMonthStyleHidden = new HiddenField();
            alternateMonthStyleHidden.ID = this.ID + "_AlternateMonthStyle";
            alternateMonthLinkStyleHidden = new HiddenField();
            alternateMonthLinkStyleHidden.ID = this.ID + "_AlternateMonthLinkStyle";
            
            monthStyleHidden = new HiddenField();
            monthStyleHidden.ID = this.ID + "_MonthStyle";
            monthLinkStyleHidden = new HiddenField();
            monthLinkStyleHidden.ID = this.ID + "_MonthLinkStyle";
            
            todayStyleHidden = new HiddenField();
            todayStyleHidden.ID = this.ID + "_TodayStyle";
            todayLinkStyleHidden = new HiddenField();
            todayLinkStyleHidden.ID = this.ID + "_TodayLinkStyle";
            
            selectedStyleHidden = new HiddenField();
            selectedStyleHidden.ID = this.ID + "_SelectedStyle";
            selectedLinkStyleHidden = new HiddenField();
            selectedLinkStyleHidden.ID = this.ID + "_SelectedLinkStyle";
            
            dateFormatHidden = new HiddenField();
            dateFormatHidden.ID = this.ID + "_DateFormat";
            
            firstDayOfWeekHidden = new HiddenField();
            firstDayOfWeekHidden.ID = this.ID + "_FirstDayOfWeek";
            
            // Floating Panel
            floatingPanel = new Panel();
            floatingPanel.ID = this.ID + "_CalendarPanel";
            floatingPanel.Style.Add("visibility", "hidden");
            floatingPanel.Style.Add("position", "absolute");
            floatingPanel.Attributes["class"] = this.CssClass;
            floatingPanel.Controls.Add(paneTable);
            floatingPanel.Controls.Add(todayHidden);
            floatingPanel.Controls.Add(yearHidden);
            floatingPanel.Controls.Add(monthHidden);
            floatingPanel.Controls.Add(alternateMonthStyleHidden);
            floatingPanel.Controls.Add(alternateMonthLinkStyleHidden);
            floatingPanel.Controls.Add(monthStyleHidden);
            floatingPanel.Controls.Add(monthLinkStyleHidden);
            floatingPanel.Controls.Add(todayStyleHidden);
            floatingPanel.Controls.Add(todayLinkStyleHidden);
            floatingPanel.Controls.Add(selectedStyleHidden);
            floatingPanel.Controls.Add(selectedLinkStyleHidden);
            floatingPanel.Controls.Add(dateFormatHidden);
            floatingPanel.Controls.Add(firstDayOfWeekHidden);
            
            // Base control
            this.Controls.Add(containerTable);
            this.Controls.Add(floatingPanel);
            this.Style["vertical-align"] = "bottom"; //Fixes line alignment issue with span display style of inline-block.
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            // Generate Calendar Day Header Cells
            for (int i = 0; i < 7; i++)
            {
                TableCell calendarTableHeadCell = new TableCell();
                calendarTableHeadCell.HorizontalAlign = HorizontalAlign.Center;
                calendarTableHeadCell.VerticalAlign = VerticalAlign.Middle;
                Literal dayLiteral = new Literal();
                calendarTableHeadCell.Controls.Add(dayLiteral);
                calendarTableHeadRow.Controls.Add(calendarTableHeadCell);
            }

            // Generate Calendar Day Cells (7 days by 6 weeks)
            for (int row = 0; row <= 5; row++)
            {
                TableRow calendarTableBodyRow = new TableRow();
                calendarTableBodyRow.TableSection = TableRowSection.TableBody;
                calendarTable.Rows.Add(calendarTableBodyRow);
                for (int col = 0; col <= 6; col++)
                {
                    TableCell calendarTableBodyCell = new TableCell();
                    calendarTableBodyCell.HorizontalAlign = HorizontalAlign.Center;
                    calendarTableBodyCell.VerticalAlign = VerticalAlign.Middle;
                    calendarTableBodyRow.Cells.Add(calendarTableBodyCell);
                    HtmlAnchor cellLink = new HtmlAnchor();
                    cellLink.Style.Add("text-decoration", "none");
                    cellLink.Attributes.Add("tabIndex", "-1");
                    calendarTableBodyCell.Controls.Add(cellLink);
                }
            }

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            //When viewstate false, clear post data from textbox
            if (this.EnableViewState)
            {
                textBox.Text = SelectedDate != DateTime.MinValue ? SelectedDate.ToString(Culture.DateTimeFormat.ShortDatePattern) : string.Empty;
            }
            else
            {
                textBox.Text = string.Empty;
            }

            /*** Apply Styles ***/
            calendarTableHeadRow.ApplyStyle(DayHeaderStyle);
            calendarTable.ApplyStyle(CalendarTableStyle);
            prevYearLink.ApplyStyle(NextPrevYearStyle);
            prevMonthLink.ApplyStyle(NextPrevMonthStyle);
            titleCell.ApplyStyle(TitleStyle);
            nextMonthLink.ApplyStyle(NextPrevMonthStyle);
            nextYearLink.ApplyStyle(NextPrevYearStyle);
            paneHeadRow.ApplyStyle(PaneHeaderStyle);
            paneTable.ApplyStyle(PaneTableStyle);
            floatingPanel.Width = PaneWidth;

            /*** Set Hidden Field Values ***/
            todayHidden.Value = this.TodaysDate.ToString("yyyy/MM/dd");
            yearHidden.Value = this.TodaysDate.Year.ToString();
            monthHidden.Value = Convert.ToString(TodaysDate.Month - 1);
            alternateMonthStyleHidden.Value = GetStyleString(AlternateMonthStyle.GetStyleAttributes(this));
            alternateMonthLinkStyleHidden.Value = GetLinkStyleString(AlternateMonthStyle.GetStyleAttributes(this));
            monthStyleHidden.Value = GetStyleString(MonthStyle.GetStyleAttributes(this));
            monthLinkStyleHidden.Value = GetLinkStyleString(MonthStyle.GetStyleAttributes(this));
            todayStyleHidden.Value = GetStyleString(TodayStyle.GetStyleAttributes(this));
            todayLinkStyleHidden.Value = GetLinkStyleString(TodayStyle.GetStyleAttributes(this));
            selectedStyleHidden.Value = GetStyleString(SelectedStyle.GetStyleAttributes(this));
            selectedLinkStyleHidden.Value = GetLinkStyleString(SelectedStyle.GetStyleAttributes(this));
            dateFormatHidden.Value = Culture.DateTimeFormat.ShortDatePattern;
            firstDayOfWeekHidden.Value = ((int)FirstDayOfWeek).ToString();
            
            base.OnPreRender(e);        
        }
        #endregion

        #region Properties
        /*** Value Properties ***/
        [Category("Behaviour"),
        DefaultValue(false),
        Description("When set to true, postback will occur when selected date is change.")]
        public bool AutoPostBack
        {
            get { return _autoPostBack; }
            set { _autoPostBack = value; }
        }

        [Category("Misc"),
        Description("Set the current date selected."),
        Bindable(true)]
        public DateTime SelectedDate
        {
            get
            {
                object o = ViewState["SelectedDate"];
                return (o != null) ? (DateTime)o : DateTime.MinValue;
            }
            set { ViewState["SelectedDate"] = value; }
        }

        [Category("Misc"),
        Description("Set the date to appear as today's date."),
        Bindable(true)]
        public DateTime TodaysDate
        {
            get 
            {
                object o = ViewState["TodaysDate"];
                return o != null ? (DateTime)o : DateTime.Today; 
            }
            set { ViewState["TodaysDate"] = value; }
        }

        public enum Day { Sunday = 0, Monday = 1, Tuesday = 2, Wednesday = 3
            , Thursday = 4, Friday = 5, Saturday = 6 };
        [Category("Misc"),
        DefaultValue(Day.Sunday),
        Description("Set the first day of week.")]
        public Day FirstDayOfWeek
        {
            get 
            {
                object o = ViewState["FirstDayOfWeek"];
                return (o != null)? (Day)o : Day.Sunday; 
            }
            set { ViewState["FirstDayOfWeek"] = value; }
        }

        [Category("Behaviour"),
        Description("Set the culture for parsing the date values.")]
        public CultureInfo Culture
        {
            get
            {
                object o = ViewState["Culture"];
                return (o != null) ? (CultureInfo)o : CultureInfo.CurrentCulture;
            }
            set { ViewState["Culture"] = value; }
        }


        /*** Appearance And Inner Style Properties ***/
        [Category("Layout"),
        Description("Set the unit width of the floating calendar panel.")]
        public Unit PaneWidth
        {
            get
            {
                if (_paneWidth == null)
                {
                    _paneWidth = new Unit("150px");
                }
                return _paneWidth;
            }
            set { _paneWidth = value; }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableStyle PaneTableStyle
        {
            get 
            {
                if (_paneTableStyle == null)
                {
                    // Set Default Style
                    _paneTableStyle = new TableStyle();
                    _paneTableStyle.BorderStyle = BorderStyle.Solid;
                    _paneTableStyle.BorderColor = System.Drawing.ColorTranslator.FromHtml("gray");
                    _paneTableStyle.BorderWidth = new Unit("1px");
                }
                return _paneTableStyle; 
            }
            set { PaneTableStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle PaneHeaderStyle
        {
            get 
            {
                if (_paneHeaderStyle == null)
                {
                    // Set Default Style
                    _paneHeaderStyle = new TableItemStyle();
                    _paneHeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#DDDDDD");
                }
                return _paneHeaderStyle; 
            }
            set { PaneHeaderStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle TitleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    // Set Default Style
                    _titleStyle = new TableItemStyle();
                }
                return _titleStyle;
            }
            set { TitleStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle NextPrevMonthStyle
        {
            get 
            {
                if (_nextPrevMonthStyle == null)
                {
                    // Set Default Style
                    _nextPrevMonthStyle = new TableItemStyle();
                    _nextPrevMonthStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("blue");
                }
                return _nextPrevMonthStyle; 
            }
            set { NextPrevMonthStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle NextPrevYearStyle
        {
            get 
            {
                if (_nextPrevYearStyle == null)
                {
                    // Set Default Style
                    _nextPrevYearStyle = new TableItemStyle();
                    _nextPrevYearStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("darkblue");
                } 
                return _nextPrevYearStyle;
            }
            set { NextPrevYearStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableStyle CalendarTableStyle
        {
            get 
            {
                if (_calenderTableStyle == null)
                {
                    // Set Default Style
                    _calenderTableStyle = new TableStyle();
                }
                return _calenderTableStyle; 
            }
            set { CalendarTableStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle DayHeaderStyle
        {
            get 
            {
                if (_dayHeaderStyle == null)
                {
                    // Set Default Style
                    _dayHeaderStyle = new TableItemStyle();
                    _dayHeaderStyle.BorderColor = System.Drawing.ColorTranslator.FromHtml("#AAAAAA");
                    _dayHeaderStyle.BorderStyle = BorderStyle.Solid;
                    _dayHeaderStyle.BorderWidth = new Unit("1px");
                    _dayHeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("silver");
                    _dayHeaderStyle.Font.Bold = true;
                }
                return _dayHeaderStyle; 
            }
            set { DayHeaderStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle AlternateMonthStyle
        {
            get 
            {
                if (_alternateMonthStyle == null)
                {
                    // Set Default Style
                    _alternateMonthStyle = new TableItemStyle();
                    _alternateMonthStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#DDDDDD");
                    _alternateMonthStyle.Font.Underline = false;
                }
                return _alternateMonthStyle;
            }
            set { AlternateMonthStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle MonthStyle
        {
            get 
            {
                if (_monthStyle == null)
                {
                    // Set Default Style
                    _monthStyle = new TableItemStyle();
                    _monthStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("white");
                    _monthStyle.Font.Underline = false;
                } 
                return _monthStyle;
            }
            set { MonthStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle TodayStyle
        {
            get 
            {
                if (_todayStyle == null)
                {
                    // Set Default Style
                    _todayStyle = new TableItemStyle();
                    _todayStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("lightgreen");
                    _todayStyle.BorderStyle = BorderStyle.Solid;
                    _todayStyle.BorderWidth = new Unit("1px");
                    _todayStyle.BorderColor = System.Drawing.ColorTranslator.FromHtml("green");
                    _todayStyle.Font.Underline = false;
                }
                return _todayStyle; 
            }
            set { TodayStyle.CopyFrom(value); }
        }

        [Category("Appearance"),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableItemStyle SelectedStyle
        {
            get 
            {
                if (_selectedStyle == null)
                {
                    // Set Default Style
                    _selectedStyle = new TableItemStyle();
                    _selectedStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("lightblue");
                    _selectedStyle.BorderStyle = BorderStyle.Solid;
                    _selectedStyle.BorderWidth = new Unit("1px");
                    _selectedStyle.BorderColor = System.Drawing.ColorTranslator.FromHtml("blue");
                    _selectedStyle.Font.Underline = false;
                } 
                return _selectedStyle;
            }
            set { SelectedStyle.CopyFrom(value); }
        }

        #endregion

        #region Methods
        // Used for creating style string from style collection.
        private string GetStyleString(CssStyleCollection styleCollection)
        {
            StringBuilder styleString = new StringBuilder();
            foreach (string key in styleCollection.Keys)
            {
                styleString.Append(key + ": " + styleCollection[key] + ";");
            }
            return styleString.ToString();
        }
        
        // Used for creating link (within calendar day) style string from style collection.
        private string GetLinkStyleString(CssStyleCollection styleCollection)
        {
            StringBuilder styleString = new StringBuilder();
            foreach (string key in styleCollection.Keys)
            {
                if (!key.ToUpper().StartsWith("BACK") &&
                    !key.ToUpper().StartsWith("BORDER"))
                {
                    styleString.Append(key + ": " + styleCollection[key] + ";");
                }
            }
            return styleString.ToString();
        }

        #endregion

        #region Events

        public event EventHandler SelectedDateChanged;

        // Monitor selected value change. Raise SelectedDateChanged event on change.
        protected virtual void OnSelectedDateChanged(object sender, EventArgs e)
        {
            //If date can't be parsed throw format exception. For other exceptions throw system exception.
            if (textBox.Text != string.Empty)
            {
                try
                {
                    this.SelectedDate = DateTime.Parse(textBox.Text, Culture);
                    if (SelectedDateChanged != null)
                    {
                        SelectedDateChanged(this, e);
                    }
                }
                catch (FormatException ex)
                {
                    throw new FormatException("Invalid selected date format. Could not parse date.", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unknown exception.", ex);
                }
            }
        }

        #endregion

    }
}
