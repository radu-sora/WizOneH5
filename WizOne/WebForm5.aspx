<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm5.aspx.cs" Inherits="WizOne.WebForm5" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

    <dx:ASPxScheduler ID="ASPxScheduler1" runat="server" Width="100%" ActiveViewType="Day" GroupType="Resource" ClientInstanceName="ASPxClientScheduler1">
        <Views>
            <DayView ResourcesPerPage="2">
                <WorkTime Start="07:00:00" End="20:00:00" />
            </DayView>
            <WorkWeekView Enabled="false" />
            <WeekView Enabled="false" />
            <MonthView Enabled="false" />
            <TimelineView Enabled="false">

            </TimelineView>
            <AgendaView Enabled="false" />
        </Views>
        <OptionsBehavior ShowViewSelector="false" />
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
    </dx:ASPxScheduler>


        </div>
    </form>
</body>
</html>
