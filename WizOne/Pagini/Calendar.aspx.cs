using DevExpress.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class Calendar : Page
    {
        public class ScheduleItemViewModel
        {
            public string Id { get; set; }              
            public string Name { get; set; }
            public string Remarks { get; set; }
            public int LabelId { get; set; }
            public int StatusId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int Type { get; set; }
            public int InternalId { get; set; }
        }

        [Serializable]
        public class ScheduleItemColor
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
        }

        public List<ScheduleItemViewModel> GetScheduleItems()
        {
            return General.RunSqlQuery<ScheduleItemViewModel>("SELECT * FROM [Schedule] WHERE [EmployeeId] = @1", HttpContext.Current.Session["User_Marca"]);
        }        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)            
            {
                ViewState["Labels"] = General.RunSqlQuery<ScheduleItemColor>("SELECT [Id], [Denumire] AS [Name], [Culoare] AS [Color] FROM [Ptj_tblAbsente] UNION SELECT 0 AS [Id], 'N/A' AS [Name], '#FF9F9F9F' AS [Color]");
                ViewState["Statuses"] = General.RunSqlQuery<ScheduleItemColor>("SELECT [Id], [Denumire] AS [Name], [Culoare] AS [Color] FROM [Ptj_tblStari]");
            }

            if (ViewState["Labels"] != null)
            {
                Scheduler.Storage.Appointments.Labels.Clear();
                Scheduler.Storage.Appointments.Labels.AddRange((ViewState["Labels"] as List<ScheduleItemColor>).
                    Select(s => Scheduler.Storage.Appointments.Labels.CreateNewLabel(s.Id, s.Name, s.Name, ColorTranslator.FromHtml(s.Color))).ToList());
            }

            if (ViewState["Statuses"] != null)
            {
                Scheduler.Storage.Appointments.Statuses.Clear();
                Scheduler.Storage.Appointments.Statuses.AddRange((ViewState["Statuses"] as List<ScheduleItemColor>).
                    Select(s => Scheduler.Storage.Appointments.Statuses.CreateNewStatus(s.Id, s.Name, s.Name, ColorTranslator.FromHtml(s.Color))).ToList());
            }           
        }       

        protected void Scheduler_AppointmentDeleting(object sender, PersistentObjectCancelEventArgs e)
        {
            var scheduleItem = General.RunSqlSingle<ScheduleItemViewModel>("SELECT * FROM [Schedule] WHERE [Id] = @1", (e.Object as Appointment).Id);

            if (scheduleItem.Type == 1) // 1 - Request leave, 2 - Log time
            {
                var listaAbsente = new Absente.Lista();

                Scheduler.JSProperties["cpDeleteError"] = listaAbsente.AnulareCerere(scheduleItem.InternalId);
            }
            else
                Scheduler.JSProperties["cpDeleteError"] = Dami.TraduCuvant("Aceasta nu este o cerere de absentă!");

            e.Cancel = true;
        }               

        protected void Scheduler_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu.MenuId == SchedulerMenuItemId.AppointmentMenu)
            {
                foreach (MenuItem item in e.Menu.Items)
                {
                    switch ((SchedulerMenuItemId)Enum.Parse(typeof(SchedulerMenuItemId), item.Name))
                    {
                        case SchedulerMenuItemId.DeleteAppointment:
                            item.Text = Dami.TraduCuvant("Anulare cerere absentă");   
                            break;                        
                    }
                }                
            }
            else if (e.Menu.MenuId == SchedulerMenuItemId.DefaultMenu)
            {
                string itemName;

                foreach (MenuItem item in e.Menu.Items)
                {
                    itemName = item.Name.
                        Replace("!", "sTo").
                        Replace("01:00:00", "60MinutesSlot").
                        Replace("00:30:00", "30MinutesSlot").
                        Replace("00:15:00", "15MinutesSlot").
                        Replace("00:10:00", "10MinutesSlot").
                        Replace("00:06:00", "6MinutesSlot").
                        Replace("00:05:00", "5MinutesSlot");

                    switch ((SchedulerMenuItemId)Enum.Parse(typeof(SchedulerMenuItemId), itemName))
                    {
                        case SchedulerMenuItemId.NewAppointment:
                            item.Text = Dami.TraduCuvant("Adăugare absentă");
                            break;
                        case SchedulerMenuItemId.GotoToday:
                            item.Text = Dami.TraduCuvant("Arată data curentă");
                            break;
                        case SchedulerMenuItemId.GotoDate:
                            item.Text = Dami.TraduCuvant("Arată data ...");
                            break;
                        case SchedulerMenuItemId.SwitchViewMenu:
                            item.Text = Dami.TraduCuvant("Vizualizare");
                            break;
                        case SchedulerMenuItemId.SwitchTimeScalesTo60MinutesSlot:
                            item.Text = Dami.TraduCuvant("60 Minute");
                            break;
                        case SchedulerMenuItemId.SwitchTimeScalesTo30MinutesSlot:
                            item.Text = Dami.TraduCuvant("30 Minute");
                            break;
                        case SchedulerMenuItemId.SwitchTimeScalesTo15MinutesSlot:
                            item.Text = Dami.TraduCuvant("15 Minute");
                            break;
                        case SchedulerMenuItemId.SwitchTimeScalesTo10MinutesSlot:
                            item.Text = Dami.TraduCuvant("10 Minute");
                            break;
                        case SchedulerMenuItemId.SwitchTimeScalesTo5MinutesSlot:
                            item.Text = Dami.TraduCuvant("5 Minute");
                            break;
                        default:
                            item.Visible = false;
                            break;
                    }
                }
            }
        }
    }
}