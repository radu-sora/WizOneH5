<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm5.aspx.cs" Inherits="WizOne.WebForm5" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

    <dx:ASPxScheduler ID="ASPxScheduler1" runat="server" Width="100%" ActiveViewType="Timeline"
        GroupType="Resource" ClientInstanceName="ASPxClientScheduler1"  Start="2019/8/27">
        <Views>
            <DayView ResourcesPerPage="2">
                <DayViewStyles ScrollAreaHeight="600px" />
            </DayView>
            <WorkWeekView ResourcesPerPage="2">
                <WorkWeekViewStyles ScrollAreaHeight="600px" />
            </WorkWeekView>
            <WeekView Enabled="False" />
            <FullWeekView Enabled="false" />
            <MonthView ResourcesPerPage="2" Enabled="False">
                <MonthViewStyles>
                    <DateCellBody Height="100px" />
                </MonthViewStyles>
            </MonthView>
            <TimelineView ResourcesPerPage="4" IntervalCount="19">
                <TimelineViewStyles>
                    <TimelineCellBody Height="120px" />
                </TimelineViewStyles>

            </TimelineView>
            <AgendaView Enabled="false" />
        </Views>
        <Storage enablereminders="false">
            <Appointments AutoRetrieveId="true" /> 
        </Storage>
    </dx:ASPxScheduler>
        </div>
    </form>
</body>
</html>
