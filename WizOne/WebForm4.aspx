<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm4.aspx.cs" Inherits="WizOne.WebForm4" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:aspxscheduler ID="ASPxScheduler1" runat="server" ActiveViewType="WorkWeek">
                <OptionsCustomization AllowAppointmentEdit="None" AllowAppointmentDelete="None"/>  
                <Views>
                    <FullWeekView Enabled="true"/>
                    <WeekView Enabled="false"/>
                </Views>
                <Storage EnableReminders="false">
                    <Appointments AutoRetrieveId="true">
                        <Mappings 
                            AllDay="AllDay" 
                            AppointmentId="ID" 
                            Description="Description" 
                            End="EndTime" 
                            Label="Label" 
                            Location="Location" 
                            RecurrenceInfo="RecurrenceInfo" 
                            ReminderInfo="ReminderInfo" 
                            ResourceId="CarId"
                            Start="StartTime" 
                            Status="Status" 
                            Subject="Subject" 
                            Type="EventType" />
                    </Appointments>
                </Storage>
            </dx:aspxscheduler>
        </div>
    </form>
</body>
</html>
