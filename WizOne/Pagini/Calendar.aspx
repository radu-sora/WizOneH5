<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="WizOne.Pagini.Calendar" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <div class="page-content">        
        <div class="page-content-data invisible">
            <dx:ASPxScheduler ID="Scheduler" ClientInstanceName="scheduler" runat="server" Width="100%" CssClass="dx-scheduler-adaptive"
                AppointmentDataSourceID="ScheduleItemsDataSource">
                <OptionsView VerticalScrollBarMode="Auto" />
                <OptionsAdaptivity Enabled="true" />
                <OptionsToolTips AppointmentToolTipMode="Auto" ShowAppointmentToolTip="false" />
                <ViewVisibleInterval>
                    <OptionsCalendar AppointmentDatesHighlightMode="Labels">
                        <SettingsAdaptivity SwitchToSingleViewAtWindowInnerWidth="768" />
                    </OptionsCalendar>
                </ViewVisibleInterval>
                <Views>
                    <DayView Enabled="true" />
                    <WorkWeekView Enabled="true" />
                    <WeekView Enabled="false" />
                    <MonthView Enabled="true" />
                    <TimelineView Enabled="false" />                        
                    <AgendaView Enabled="true" DayCount="4" />
                </Views>                
                <Storage EnableReminders="false">
                    <Appointments>
                        <Mappings AppointmentId="Id" Subject="Name" Description="Remarks" Label="LabelId" Status="StatusId" Start="StartDate" End="EndDate" />
                    </Appointments>                    
                </Storage>
            </dx:ASPxScheduler>
        </div>        
    </div>

    <asp:ObjectDataSource ID="ScheduleItemsDataSource" runat="server" TypeName="WizOne.Pagini.Calendar" DataObjectTypeName="WizOne.Pagini.Calendar+ScheduleItemViewModel"
        SelectMethod="GetScheduleItems">        
    </asp:ObjectDataSource>

    <script>               
        /* Page control */
        var pageControl = {
            /* Data */
            pageContent: null,
            /* Interface */
            init: function () {
                var self = this;

                self.pageContent = $('.page-content');
                ASPxClientControl.GetControlCollection().ControlsInitialized.AddHandler(function () {
                    self.onControlsInitialized(self);
                });
            },
            /* Events */
            onControlsInitialized: function (pageControl) {
                pageControl.pageContent.find('> div[class*="invisible"]').removeClass('invisible'); // To hide DX controls UI init issues.
            }            
        };

        pageControl.init();
    </script>
</asp:Content>
