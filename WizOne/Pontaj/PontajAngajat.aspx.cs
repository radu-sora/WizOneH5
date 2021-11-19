using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajAngajat : Page
    {
        public class ActionViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class ScheduleItemViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Remarks { get; set; }
            public int LabelId { get; set; }
            public int StatusId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        [Serializable]
        public class ScheduleItemColor
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
        }
        
        public List<ActionViewModel> GetActions()
        {
            return General.RunSqlQuery<ActionViewModel>("SELECT [F06204] AS [Id], [F06205] AS [Name] FROM [F062]");
        }

        public List<ScheduleItemViewModel> GetScheduleItems()
        {
            return General.RunSqlQuery<ScheduleItemViewModel>("SELECT * FROM [Schedule] WHERE [Type] = 2 AND [EmployeeId] = @1", HttpContext.Current.Session["User_Marca"]);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var currentScheduleItem = General.RunSqlSingle<ScheduleItemViewModel>("SELECT * FROM [Schedule] WHERE [Type] = 2 AND [StatusId] = 2 AND [EmployeeId] = @1", Session["User_Marca"]);

                Scheduler.FloatingActionButton.InitialActionContext = currentScheduleItem == null ? "start" : "stop";
                ViewState["Labels"] = General.RunSqlQuery<ScheduleItemColor>("SELECT 0 AS [Id], 'N/A' AS [Name], '#FF9F9F9F' AS [Color]");
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

        protected void Scheduler_AppointmentInserting(object sender, PersistentObjectCancelEventArgs e)
        {
            var appointment = (e.Object as Appointment);
            var result = 0;

            if ((int)appointment.StatusKey == 2)
            {
                result = General.RunSqlScalar<int>("INSERT INTO [Ptj_CC]([F10003], [Ziua], [F06204], [De], [IdStare], [USER_NO]) VALUES (@1, @2, @3, @4, @5, @6)", null,
                    Session["User_Marca"], appointment.Start.Date, appointment.LabelKey, appointment.Start, appointment.StatusKey, 999);
            }
            else
            {
                var startDate = General.RunSqlScalar<DateTime>("SELECT [De] FROM [Ptj_CC] WHERE [F10003] = @1 AND [IdStare] = @2", null, 
                    Session["User_Marca"], 2);

                if (startDate > DateTime.MinValue && startDate < DateTime.Now)
                {
                    //TODO: De esalonat pe zile in cazul in care End - Start > o zi! 
                    result = General.RunSqlScalar<int>("UPDATE [Ptj_CC] SET [La] = @1, [NrOre1] = @2, [IdStare] = @3 WHERE [F10003] = @4 AND [IdStare] = @5", null,
                        appointment.Start, (int)(appointment.Start - startDate).TotalMinutes, appointment.StatusKey, Session["User_Marca"], 2);

                    if (result > 0)
                    {
                        //General.PontajInitGeneral(Convert.ToInt32(Session["UserId"]), appointment.Start.Year, appointment.Start.Month);

                    }
                }
            }

            if (result == 0)
                Scheduler.JSProperties["cpInsertError"] = Dami.TraduCuvant("Eroare la salvarea datelor!");

            e.Cancel = true;
        }
    }
}