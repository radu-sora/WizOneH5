<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="WizOne.Pagini.Calendar" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Modal dialogs -->    

    <!-- Page content -->
    <div class="page-content">        
        <div class="page-content-data invisible">
            <dx:ASPxScheduler ID="Scheduler" ClientInstanceName="scheduler" runat="server" Width="100%" CssClass="dx-scheduler-adaptive"
                AppointmentDataSourceID="ScheduleItemsDataSource" OnAppointmentDeleting="Scheduler_AppointmentDeleting" OnPopupMenuShowing="Scheduler_PopupMenuShowing">
                <OptionsView VerticalScrollBarMode="Auto" />
                <OptionsAdaptivity Enabled="true" />
                <OptionsCustomization AllowAppointmentEdit="None" />
                <OptionsToolTips AppointmentToolTipMode="Auto" ShowAppointmentToolTip="false" />                
                <ViewVisibleInterval>
                    <OptionsCalendar AppointmentDatesHighlightMode="Labels">
                        <SettingsAdaptivity SwitchToSingleViewAtWindowInnerWidth="768" />
                    </OptionsCalendar>
                </ViewVisibleInterval>
                <Views>
                    <DayView Enabled="true" DisplayName="Zi" ShortDisplayName="Zi" MenuCaption="Zi" />
                    <WorkWeekView Enabled="true" DisplayName="Săptămână" ShortDisplayName="Săptămână" MenuCaption="Săptămână" />
                    <WeekView Enabled="false" />
                    <MonthView Enabled="true" DisplayName="Lună" ShortDisplayName="Lună" MenuCaption="Lună" />
                    <TimelineView Enabled="false" />                        
                    <AgendaView Enabled="true" DayCount="14" DisplayName="Listă" ShortDisplayName="Listă" MenuCaption="Listă" />
                </Views>
                <FloatingActionButton>
                    <Items>
                        <dx:FABCreateAppointmentAction Text="Adăugare absentă" />
                        <dx:FABEditAppointmentActionGroup>
                            <Items>                                
                                <dx:FABDeleteAppointmentActionItem Text="Anulare cerere absentă" />
                            </Items>
                        </dx:FABEditAppointmentActionGroup>
                    </Items>                    
                </FloatingActionButton>
                <Storage EnableReminders="false">
                    <Appointments>
                        <Mappings AppointmentId="Id" Subject="Name" Description="Remarks" Label="LabelId" Status="StatusId" Start="StartDate" End="EndDate" />                        
                    </Appointments>                    
                </Storage>
                <ClientSideEvents 
                    MenuItemClicked="function(s, e) { pageControl.onSchedulerMenuItemClicked(e); }" 
                    EndCallback="function(s, e) { pageControl.onSchedulerEndCallback(e); }" />
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
                ASPxClientControl.GetControlCollection().ControlsInitialized.AddHandler(function (s, e) {
                    self.onControlsInitialized(e);
                });
            },
            /* Events */
            onControlsInitialized: function (e) {
                if (!e.isCallback) { // Validate document ready
                    this.pageContent.find('> div[class*="invisible"]').removeClass('invisible'); // To hide DX controls UI init issues.                
                }
            },
            onSchedulerMenuItemClicked: function(e) {
                e.handled = true;
                
                switch (e.itemName) {
                    case ASPx.SchedulerMenuItemId.NewAppointment:
                        var interval = scheduler.GetSelectedInterval();
                        var startDate = interval.GetStart();                            
                        var endDate = interval.GetEnd();

                        startDate = startDate.getFullYear() + '-' + (startDate.getMonth() + 1) + '-' + startDate.getDate();
                        endDate = endDate.getFullYear() + '-' + (endDate.getMonth() + 1) + '-' + endDate.getDate();

                        location.href = '../Absente/Cereri?start=' + startDate + '&end=' + endDate;
                        break;                    
                    default:
                        e.handled = false;
                }            
            },
            onSchedulerEndCallback: function (e) {
                if (scheduler.cpDeleteError) {
                    swal({
                        title: 'Atentie',
                        text: scheduler.cpDeleteError,
                        type: 'warning'
                    });

                    delete scheduler.cpDeleteError;
                }
            }
        };

        pageControl.init();
    </script>
</asp:Content>
