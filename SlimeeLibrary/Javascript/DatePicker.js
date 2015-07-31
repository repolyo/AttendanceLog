/***
===============================================================================
Slimee Control Library - Copyright © 2009
[DatePicker Control Javascript]

Author: Daniel Oliver
Website: http://www.slimee.com

NOTICE:
This file is part of Slimee Control Library.

Slimee Control Library is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Slimee Control Library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

HISTORY:
1.0.0 First Release

===============================================================================
***/

/*** Function to display floating pane ***/
function DatePicker_ShowCalendar(clientID) {
    // Get Calendar DIV object. If hidden, show. If visible, hide.    
    var calendarPanel = document.getElementById(clientID + "_CalendarPanel");
    if (calendarPanel.style.visibility == "hidden")
    {
        // Show floating pane.
        calendarPanel.style.visibility = "visible";
        calendarPanel.style.overflow = "visible";

        // Set position of calendar.
        var dateTextBox = document.getElementById(clientID + "_TextBox");
        calendarPanel.style.left =
            DatePicker_FindPos(dateTextBox)[0] + "px";
        calendarPanel.style.top =
            (DatePicker_FindPos(dateTextBox)[1] +
            document.getElementById(clientID + "_TextBox").offsetHeight) + "px";
               
        // If date already selectged, change current calendar position to month of selected date.       
        var dateTextBox = document.getElementById(clientID + "_TextBox").value;
        var dateFormat = document.getElementById(clientID + "_DateFormat").value;
        var selectedDate = null;
        if (dateTextBox.length > 0)
        {
            try
            {
                //Parse selected date using server current culture short date format
                selectedDate = new Date(DatePicker_getDateFromFormat(dateTextBox,dateFormat));
                document.getElementById(clientID + "_MonthValue").value = selectedDate.getMonth();
                document.getElementById(clientID + "_YearValue").value = selectedDate.getFullYear();            
            } catch (e){}
        }
               
        // Generate calendar.
        DatePicker_CreateCalendar(clientID);
    }
    else
    {
        // Hide floating pane.
        calendarPanel.style.visibility = "hidden";
        calendarPanel.style.overflow = "hidden";
    }
    
    return false;
}

/*** Function to set and create calendar grid. ***/
function DatePicker_CreateCalendar(clientID) {

    var currentMonth = new Number(document.getElementById(clientID + "_MonthValue").value);
    var currentYear = new Number(document.getElementById(clientID + "_YearValue").value);
        
    // Set start date of calendar to first day of month.
    var startDate = new Date();
    startDate.setFullYear(currentYear, currentMonth, 1);
   
    // Identify first day of week. Set start date to begin calendar.
    var firstDayOfWeek = new Number(document.getElementById(clientID + "_FirstDayOfWeek").value);
    var diff = firstDayOfWeek - startDate.getDay();
    if (diff > 0) { diff = diff - 7; }
    startDate.setDate(startDate.getDate() + diff);
	
	// Get value set for Today.   
    var todayValue = document.getElementById(clientID + "_TodayValue").value;
    var today = new Date(todayValue);
    
    // Check textbox for selected value.
    var dateTextBox = document.getElementById(clientID + "_TextBox").value;
    // Get date format.
    var dateFormat = document.getElementById(clientID + "_DateFormat").value;
    var selectedDate = null;
    // If selected value found. Parse selected date.
    if (dateTextBox.length > 0)
    {
        try
        {
            // Parse selected date using server current culture short date format
            selectedDate = new Date(DatePicker_getDateFromFormat(dateTextBox,dateFormat));
        } catch (e){}
    }

    
    // Create Day Headers
    var headTable = document.getElementById(clientID + "_CalendarTable").getElementsByTagName("thead")[0];
    var days = new Array("Su","Mo","Tu","We","Th","Fr","Sa");
    var dow = firstDayOfWeek;
    for (i = 0; i < 7; i++)
    {
        var cell = headTable.rows[0].cells[i];
        cell.innerHTML = days[dow];
        dow += 1;
        if (dow > 6) { dow = 0 }
    }
    
    // Edit Weeks Rows
    var bodyTable = document.getElementById(clientID + "_CalendarTable").getElementsByTagName("tbody")[0];
    for (i = 0; i <= 5; i++)
    {
        // Create Day Cells
        for (x = 0; x <= 6; x++ )
        {
			var cell = bodyTable.rows[i].cells[x];
            
            // Create date link value using server culture date format.
            var selectDate = DatePicker_formatDate(startDate,dateFormat);
			var cellLink = cell.getElementsByTagName("a")[0];
            cellLink.href = "javascript: DatePicker_SetDate('"+clientID+"'"+",'" + selectDate + "'); DatePicker_PostBack();";
            cellLink.innerHTML = startDate.getDate();
                        
            // Set style of cell, depending if current month, today, selected date, or prev/next month.
            if (startDate.getMonth() != currentMonth) {
                cell.style.cssText = document.getElementById(clientID + "_AlternateMonthStyle").value;
                cellLink.style.cssText = document.getElementById(clientID + "_AlternateMonthLinkStyle").value;
            }
            else if (startDate.toDateString() == today.toDateString()) {
                cell.style.cssText = document.getElementById(clientID + "_TodayStyle").value;
                cellLink.style.cssText = document.getElementById(clientID + "_TodayLinkStyle").value;
            } 
            else if (selectedDate != null && 
                startDate.toDateString() == selectedDate.toDateString()) {
                cell.style.cssText = document.getElementById(clientID + "_SelectedStyle").value;
                cellLink.style.cssText = document.getElementById(clientID + "_SelectedLinkStyle").value;
            }
            else {
                cell.style.cssText = document.getElementById(clientID + "_MonthStyle").value;
                cellLink.style.cssText = document.getElementById(clientID + "_MonthLinkStyle").value;
            }
            
            startDate.setDate(startDate.getDate() + 1);
        }
    }
   
    // Create month array
	var months =
		new Array("Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec");
    
    // Set calendar title.
    document.getElementById(clientID + "_MonthCell").innerHTML =
		months[currentMonth] + ", " +
        currentYear;
}

/*** Function to populate selected date to TextBox. ***/
function DatePicker_SetDate(clientID, selectDate)
{
    document.getElementById(clientID + "_TextBox").value = selectDate;
    // Hide calendar and postback.
    DatePicker_ShowCalendar(clientID);
    DatePicker_PostBack(clientID,'');
}
   
/*** Function change to next month. ***/
function DatePicker_NextMonth(clientID) {
	var month = new Number(document.getElementById(clientID+"_MonthValue").value) + 1;
	var year = new Number(document.getElementById(clientID+"_YearValue").value);
    if (month > 11) {
        month = 0;
        year += 1;
    }

    document.getElementById(clientID+"_MonthValue").value = month;
    document.getElementById(clientID+"_YearValue").value = year;
    DatePicker_CreateCalendar(clientID);
}

/*** Function change to next year. ***/
function DatePicker_NextYear(clientID) {
	var year = new Number(document.getElementById(clientID+"_YearValue").value) + 1;
    document.getElementById(clientID+"_YearValue").value = year;
    DatePicker_CreateCalendar(clientID);
}

/*** Function change to previous month. ***/
function DatePicker_PrevMonth(clientID) {
	var month = new Number(document.getElementById(clientID+"_MonthValue").value) - 1;
	var year = new Number(document.getElementById(clientID+"_YearValue").value);
    if (month < 0) {
        month = 11;
        year -= 1;
    }
    
    document.getElementById(clientID+"_MonthValue").value = month;
    document.getElementById(clientID+"_YearValue").value = year;
    DatePicker_CreateCalendar(clientID);
}

/*** Function change to previous year. ***/
function DatePicker_PrevYear(clientID) {
	var year = new Number(document.getElementById(clientID+"_YearValue").value) - 1;
    document.getElementById(clientID+"_YearValue").value = year;
    DatePicker_CreateCalendar(clientID);
}

/*** Function to find offset position of an element. 
     Used to calculate position of floating panel. ***/
function DatePicker_FindPos(obj) {
    var curLeft = curTop = 0;
    if (obj.offsetParent) {
        do {
            curLeft += obj.offsetLeft;
            curTop += obj.offsetTop;
        } while (obj = obj.offsetParent);
    }
    return [curLeft, curTop];
}