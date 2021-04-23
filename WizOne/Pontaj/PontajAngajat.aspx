<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajAngajat.aspx.cs" Inherits="WizOne.Pontaj.PontajAngajat" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Modal dialogs -->
    <div class="dx-popup-adaptive-fullscreen">
        <dx:ASPxPopupControl ID="ActionsPopup" ClientInstanceName="actionsPopup" runat="server" Width="400px" Height="300px"
            CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
            HeaderText="Activitati" ShowFooter="true" FooterText="Selectati o activitate">            
            <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchAtWindowInnerWidth="768" MinWidth="100%" MinHeight="100%" />
            <ContentStyle>
                <Paddings Padding="0" />
            </ContentStyle>
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxGridView ID="ActionsGridView" ClientInstanceName="actionsGridView" runat="server" Width="100%" CssClass="dx-grid-adaptive"
                        DataSourceID="ActionsDataSource" AutoGenerateColumns="False" KeyFieldName="Id">
                        <Settings ShowColumnHeaders="false" GridLines="Horizontal" VerticalScrollBarMode="Auto" />                        
                        <SettingsSearchPanel Visible="true" />
                        <SettingsBehavior AllowSelectByRowClick="true" AllowSelectSingleRowOnly="true" AllowFocusedRow="true" />
                        <SettingsPager Mode="EndlessPaging" /> 
                        <SettingsLoadingPanel Text="Încărcare..." />
                        <SettingsText EmptyDataRow="Nu sunt date de afișat" />                    
                        <Columns>
                            <dx:GridViewCommandColumn ShowSelectCheckbox="true" Width="50px" />                        
                            <dx:GridViewDataTextColumn FieldName="Name">
                                <CellStyle HorizontalAlign="Left" />
                            </dx:GridViewDataTextColumn>
                        </Columns>
                        <ClientSideEvents RowClick="function(s, e) { pageControl.onActionsGridViewRowClick(e); }" />
                    </dx:ASPxGridView>
                </dx:PopupControlContentControl>
            </ContentCollection>
        </dx:ASPxPopupControl>
    </div>    

    <!-- Page content -->
    <div class="page-content">        
        <div class="page-content-data invisible">
            <dx:ASPxScheduler ID="Scheduler" ClientInstanceName="scheduler" runat="server" ActiveViewType="Agenda" Width="100%" CssClass="dx-scheduler-adaptive"
                AppointmentDataSourceID="ScheduleItemsDataSource" OnAppointmentInserting="Scheduler_AppointmentInserting">
                <OptionsBehavior ShowViewSelector="false" />
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
                    <DayView Enabled="false" />
                    <WorkWeekView Enabled="false" />
                    <WeekView Enabled="false" />
                    <MonthView Enabled="false" />
                    <TimelineView Enabled="false" />
                    <AgendaView Enabled="true" DayCount="14" />
                </Views>
                <FloatingActionButton>
                    <Items>                                                
                        <dx:FABAction ActionName="start" ContextName="start" Text="Start activitate">
                            <Image Url="../Fisiere/Imagini/Icoane/start.svg" />
                        </dx:FABAction>
                        <dx:FABAction ActionName="stop" ContextName="stop" Text="Stop activitate">
                            <Image Url="../Fisiere/Imagini/Icoane/stop.svg" />
                        </dx:FABAction>
                    </Items>
                    <ClientSideEvents ActionItemClick="function(s, e) { pageControl.onSchedulerFabActionItemClick(e); }" />
                </FloatingActionButton>
                <Storage EnableReminders="false">
                    <Appointments>
                        <Mappings AppointmentId="Id" Subject="Name" Description="Remarks" Label="LabelId" Status="StatusId" Start="StartDate" End="EndDate" />                        
                    </Appointments>                    
                </Storage>
                <ClientSideEvents EndCallback="function(s, e) { pageControl.onSchedulerEndCallback(e); }" />
            </dx:ASPxScheduler>
        </div>        
    </div>

    <asp:ObjectDataSource ID="ActionsDataSource" runat="server" TypeName="WizOne.Pontaj.PontajAngajat" DataObjectTypeName="WizOne.Pontaj.PontajAngajat+ActionItemViewModel"
        SelectMethod="GetActions">        
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ScheduleItemsDataSource" runat="server" TypeName="WizOne.Pontaj.PontajAngajat" DataObjectTypeName="WizOne.Pontaj.PontajAngajat+ScheduleItemViewModel"
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
            start: function (actionId) {
                var newAppointment = new ASPxClientAppointment();

                newAppointment.SetSubject('Start action');
                newAppointment.SetLabelId(actionId);
                newAppointment.SetStatusId(2);
                newAppointment.SetStart(new Date());
                newAppointment.AddResource(null);

                scheduler.InsertAppointment(newAppointment);
            },
            stop: function () {
                var newAppointment = new ASPxClientAppointment();

                newAppointment.SetSubject('Stop action');
                newAppointment.SetStatusId(3);
                newAppointment.SetStart(new Date());
                newAppointment.AddResource(null);

                scheduler.InsertAppointment(newAppointment);
            },
            /* Events */
            onControlsInitialized: function (e) {
                if (!e.isCallback) { // Validate document ready
                    this.pageContent.find('> div[class*="invisible"]').removeClass('invisible'); // To hide DX controls UI init issues.
                }                
            },
            onActionsGridViewRowClick: function (e) {
                this.start(actionsGridView.GetRowKey(e.visibleIndex));                
                actionsPopup.Hide();
            },
            onSchedulerFabActionItemClick: function (e) {
                if (e.actionName === 'start') {
                    actionsPopup.Show();
                    actionsGridView.Refresh();
                } else if (e.actionName === 'stop') {
                    this.stop();
                }                          
            },
            onSchedulerEndCallback: function (e) {
                if (e.command === ASPx.SchedulerClasses.CommandConsts.INSERT_APPOINTMENT) {
                    if (scheduler.cpInsertError) {
                        swal({
                            title: 'Atentie',
                            text: scheduler.cpInsertError,
                            type: 'error'
                        });

                        delete scheduler.cpInsertError;
                    } else {                        
                        scheduler.GetFloatingActionButton().SetActionContext(scheduler.GetFloatingActionButton().GetActionContext() === 'start' ? 'stop' : 'start');
                        scheduler.Refresh();
                    }
                }
                else if (e.command === 'REFRESH|') {
                    var intervals = scheduler.GetVisibleIntervals();

                    if (intervals.length) {
                        var start = intervals[0].GetStart();
                        var end = intervals[intervals.length - 1].GetEnd();
                        var current = new Date();

                        if (current.getTime() < start.getTime() || current.getTime() > end.getTime()) {
                            setTimeout(function () {
                                scheduler.GotoToday();
                            }, 500);
                        }                        
                    }
                }                
            }
        };

        pageControl.init();
    </script>
</asp:Content>

